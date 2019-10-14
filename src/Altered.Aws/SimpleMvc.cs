using Altered.Pipeline;
using Altered.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Aws
{
    // a simple api pipeline to pass requests serialized into json body
    // useful when you have a 3rd party dto that you just want to send over the wire
    // without translating it into ApiRequest/Response
    public sealed class SimpleMvc<TRequest, TResponse> : AlteredPipeline<AlteredApiRequest, AlteredApiResponse>
    {
        public SimpleMvc(IAlteredPipeline<TRequest, TResponse> pipeline) : base(async (request) =>
        {
            var innerRequest = JsonConvert.DeserializeObject<TRequest>(request.Body, AlteredJson.DefaultJsonSerializerSettings);
            var innerResponse = await pipeline.Execute(innerRequest);
            var body = AlteredJson.SerializeObject(innerResponse);
            return new AlteredApiResponse
            {
                StatusCode = 200,
                Body = body
            };
        })
        { }
    }

    public static class SimpleMvcExtensions
    {
        public static IAlteredPipeline<AlteredApiRequest, AlteredApiResponse> WithSimpleMvc<TRequest, TResponse>(this IAlteredPipeline<TRequest, TResponse> pipeline) =>
            new SimpleMvc<TRequest, TResponse>(pipeline);
    }
}
