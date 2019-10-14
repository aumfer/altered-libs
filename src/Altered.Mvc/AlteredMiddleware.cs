using Altered.Shared;
using Altered.Shared.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Altered.Pipeline;

// this doesn't quite work yet (because of how aspnetcore2 handles state/responses)
// waiting for aspnetcore3, see AlteredLogMiddlewareExtensions.AlteredLogMiddleware for now
namespace Altered.Mvc
{
    public sealed class AlteredMvcRequest : IAlteredRequest
    {
        public string RequestId
        {
            get => Context.Request.Headers[AlteredHeaderNames.CorrelationId];
            set => Context.Request.Headers[AlteredHeaderNames.CorrelationId] = value;
        }

        public HttpContext Context { get; set; }
    }

    public sealed class AlteredMvcResponse : IAlteredResponse
    {
        public string RequestId
        {
            get => Context.Response.Headers[AlteredHeaderNames.CorrelationId];
            set => Context.Response.Headers[AlteredHeaderNames.CorrelationId] = value;
        }
        public StatusCode StatusCode
        {
            get => Context.Response.StatusCode;
            set => Context.Response.StatusCode = value;
        }
        public double RequestDuration
        {
            get => DoubleHelper.TryParse(Context.Response.Headers[AlteredHeaderNames.Duration]) ?? 0;
            set => Context.Response.Headers[AlteredHeaderNames.Duration] = $"{value}";
        }

        public HttpContext Context { get; set; }
    }

    public static class AlteredMiddlewareExtensions
    {
        public static RequestDelegate AlteredMiddleware(RequestDelegate mvcPipeline) => mvcPipeline
            .ToAlteredMiddleware()
            // this pipeline has everything. probably don't want to retry our own incoming http requests
            .WithAlteredPipeline()
            .ToMvcMiddleware();

        // this is more or less the same thing as offering an extension method "overload" for say elasticsearchclient
        // that wraps its parameters in IAlteredRequest/Response shapes
        // we're just doing it on the functions themselves (mvc middlewares)
        public static Func<AlteredMvcRequest, Task<AlteredMvcResponse>> ToAlteredMiddleware(this RequestDelegate mvcPipeline) =>
            async (request) =>
            {
                await mvcPipeline(request.Context);
                return new AlteredMvcResponse
                {
                    Context = request.Context
                };
            };

        public static RequestDelegate ToMvcMiddleware(this Func<AlteredMvcRequest, Task<AlteredMvcResponse>> func) =>
            async (mvcRequest) =>
            {
                var request = new AlteredMvcRequest
                {
                    Context = mvcRequest
                };
                var response = await func(request);

                // nothing to return, mvc is just Task not Task<>
                // all state is in shared HttpContext for mvc

                // ms... u know the ability to put a type in there could have been super useful
                // could have just used Task<HttpContext> for now
                // and at least kept the future opportunity to get rid of httpcontext
                // now you're stuck with the "bag of mutable state" pattern forever
            };
    }
}
