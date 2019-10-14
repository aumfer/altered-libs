using Altered.Shared.Interfaces;
using System;
using System.Threading.Tasks;

namespace Altered.Pipeline.Pipelines
{
    public static class CreateRequestIdExtensions
    {
        public static Func<TRequest, Task<TResponse>> WithCreateRequestId<TRequest, TResponse>(this Func<TRequest, Task<TResponse>> func)
            where TRequest : IRequestId =>
            (request) =>
            {
                request.RequestId = request.RequestId ?? Guid.NewGuid().ToString();
                return func(request);
            };
    }
}
