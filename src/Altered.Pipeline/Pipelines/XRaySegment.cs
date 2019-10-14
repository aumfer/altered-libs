using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Altered.Shared.Interfaces;
using Amazon.XRay.Recorder.Core;

namespace Altered.Pipeline.Pipelines
{
    public static class XRaySegmentExtensions
    {
        // xray kinda sucks
        public static Func<TRequest, Task<TResponse>> WithXRaySegment<TRequest, TResponse>(this Func<TRequest, Task<TResponse>> func, string name)
            where TRequest : IRequestId =>
            async (request) =>
            {
                AWSXRayRecorder.Instance.BeginSegment(name);
                AWSXRayRecorder.Instance.AddAnnotation(nameof(request.RequestId), request.RequestId);

                try
                {
                    var response = await func(request);
                    return response;
                }
                catch (Exception e)
                {
                    AWSXRayRecorder.Instance.AddException(e);
                    throw;
                }
                finally
                {
                    AWSXRayRecorder.Instance.EndSegment();
                }
            };
    }
}
