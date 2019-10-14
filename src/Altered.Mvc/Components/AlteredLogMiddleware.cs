using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Altered.Shared.Extensions.NonGeneric;
using Altered.Shared;
using Altered.Shared.Helpers;

namespace Altered.Mvc.Components
{
    // todo replace with AlteredMiddlewareExtensions.AlteredMiddleware
    public static class AlteredLogMiddlewareExtensions
    {
        public static RequestDelegate AlteredLogMiddleware(this RequestDelegate mvcPipeline) => async (context) =>
        {
            var clock = Stopwatch.StartNew();
            var request = context.Request;
            var name = $"{request.Method} {request.Path}";
            var response = context.Response;

            if (!request.Headers.TryGetValue(AlteredHeaderNames.CorrelationId, out StringValues correlationId))
            {
                correlationId = new StringValues(Guid.NewGuid().ToString());
                request.Headers.Add(AlteredHeaderNames.CorrelationId, correlationId);
                response.Headers.Add(AlteredHeaderNames.CorrelationId, correlationId);
            }

            AlteredLog.Information(new
            {
                Name = name,
                RequestId = $"{correlationId}",
                Request = new
                {
                    RemoteIpAddress = $"{request.HttpContext.Connection?.RemoteIpAddress}",
                    UserIdentityName = request.HttpContext.User?.Identity?.Name,
                    request.ContentType,
                    request.ContentLength,
                    request.Headers
                }
            });

            try
            {
                await mvcPipeline(context);
            }
            catch (Exception e)
            {
                StatusCode statusCode = 500;
                var duration = clock.Elapsed.TotalMilliseconds;

                response.StatusCode = statusCode;

                ExceptionHelper.DontThrow(() => // todo check this might throw?
                {
                    response.Headers.Add(AlteredHeaderNames.Duration, $"{duration}");
                });

                AlteredLog.Error(new
                {
                    Name = name,
                    RequestId = $"{correlationId}",
                    Response = new
                    {
                        StatusCode = (int)statusCode,
                        RequestDuration = duration,
                        RemoteIpAddress = $"{response.HttpContext.Connection?.RemoteIpAddress}",
                        UserIdentityName = request.HttpContext.User?.Identity?.Name,
                        Exception = new
                        {
                            //e.StackTrace,
                            StackTrace = new StackTrace(e).ToString(),
                            e.Source,
                            e.Message,
                            e.Data
                        }
                    }
                });
            }

            AlteredLog.Information(new
            {
                Name = name,
                RequestId = $"{correlationId}",
                Response = new
                {
                    StatusCode = (int)response.StatusCode,
                    RequestDuration = clock.Elapsed.TotalMilliseconds,
                    RemoteIpAddress = $"{response.HttpContext.Connection?.RemoteIpAddress}",
                    UserIdentityName = response.HttpContext.User?.Identity?.Name,
                    response.ContentType,
                    response.ContentLength,
                    response.Headers
                }
            });
        };
    }
}
