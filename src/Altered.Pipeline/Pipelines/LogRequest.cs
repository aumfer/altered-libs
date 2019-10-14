using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace Altered.Pipeline.Pipelines
{
    public static class LogRequestExtensions
    {
        public static Func<TRequest, Task<TResponse>> WithLogRequest<TRequest, TResponse>(this Func<TRequest, Task<TResponse>> func,
            Action<TRequest> logMessage = null)
        {
            return async (request) =>
            {
                logMessage?.Invoke(request);
                var response = await func(request);
                return response;
            };
        }

    }
}
