using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Altered.Aws;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altered.Shared;
using System.Diagnostics;
using Altered.Pipeline;

namespace Altered.Aws
{
    public interface ILambda : IAlteredPipeline<ApplicationLoadBalancerRequest, ApplicationLoadBalancerResponse> { }

    public class Lambda : AlteredPipeline<ApplicationLoadBalancerRequest, ApplicationLoadBalancerResponse>, ILambda
    {
        public Lambda(IAlteredPipeline<AlteredApiRequest, AlteredApiResponse> pipeline) : base(async (request) =>
        {
            var body = request.IsBase64Encoded ?
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Body)) :
                    request.Body;
            var mvcRequest = new AlteredApiRequest
            {
                Path = request.Path,
                HttpMethod = request.HttpMethod,
                Headers = request.Headers.ToDictionary(kvp => kvp.Key, kvp => new StringValues(kvp.Value)),
                QueryStringParameters = request.QueryStringParameters.ToDictionary(kvp => kvp.Key, kvp => new StringValues(kvp.Value)),
                Body = body
            };

            try
            {
                var mvcResponse = await pipeline.Execute(mvcRequest);
                var response = new ApplicationLoadBalancerResponse
                {
                    StatusCode = mvcResponse.StatusCode,
                    Headers = mvcResponse.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()),
                    Body = mvcResponse.Body
                };
                return response;
            }
            catch (Exception e)
            {
                // lambda will write error + request if thrown to
                Console.Error.WriteLine(AlteredJson.SerializeObject(new
                {
                    e.Message,
                    e.Source,
                    StackTrace = new StackTrace(e).ToString(),
                    e.Data
                }));
                Console.Out.WriteLine(AlteredJson.SerializeObject(mvcRequest));
                throw e;
            }
        })
        { }
    }

    public static class LambdaExtensions
    {
        // translate MvcRequest/Response into ApplicationLoadBalancerRequest/Response
        public static ILambda WithLambda(this IAlteredPipeline<AlteredApiRequest, AlteredApiResponse> pipeline) =>
            new Lambda(pipeline);
    }
}
