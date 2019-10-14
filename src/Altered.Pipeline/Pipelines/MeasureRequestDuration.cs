using Altered.Shared.Interfaces;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Altered.Pipeline.Pipelines
{
    public static class MeasureRequestDurationExtensions
    {
        public static Func<TRequest, Task<TResponse>> WithMeasureRequestDuration<TRequest, TResponse>(this Func<TRequest, Task<TResponse>> func)
            where TResponse : IRequestDuration =>
            async (TRequest request) =>
            {
                var clock = Stopwatch.StartNew();
                var response = await func(request);
                response.RequestDuration = clock.Elapsed.TotalMilliseconds;
                return response;
            };
    }
}
