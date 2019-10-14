using Altered.Shared.Interfaces;
using System;
using System.Threading.Tasks;

namespace Altered.Pipeline.Pipelines
{
    public static class CopyRequestIdExtensions
    {
        public static Func<TRequest, Task<TResponse>> WithCopyRequestId<TRequest, TResponse>(this Func<TRequest, Task<TResponse>> func)
            where TRequest : IRequestId
            where TResponse : IRequestId =>
            async (request) =>
            {
                var response = await func(request);
                response.RequestId = request.RequestId;
                return response;
            };
    }
}
