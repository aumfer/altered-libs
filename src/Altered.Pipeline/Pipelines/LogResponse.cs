using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace Altered.Pipeline.Pipelines
{
    public static class LogResponseExtensions
    {
        public static Func<TRequest, Task<TResponse>> WithLogResponse<TRequest, TResponse>(this Func<TRequest, Task<TResponse>> func,
            Action<TResponse> logMessage = null) => async (request) =>
            {
                var response = await func(request);
                logMessage?.Invoke(response);
                return response;
            };
    }
}
