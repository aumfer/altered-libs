using Polly;
using Altered.Shared.Extensions;
using Altered.Shared.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Altered.Shared;
using System.Collections;

namespace Altered.Pipeline.Pipelines
{
    public static class RetryExtensions
    {
        public static Func<TRequest, Task<TResponse>> WithRetry<TRequest, TResponse>(this Func<TRequest, Task<TResponse>> func, string name,
            Func<PolicyBuilder<TResponse>, string, AsyncPolicy<TResponse>> retryPolicy = null)
            where TResponse : IAlteredResponse
        {
            retryPolicy = retryPolicy ?? AlteredRetryPolicy;
            return (request) =>
            Policy
                .HandleResult<TResponse>(r =>
                    r.StatusCode.ShouldRetry())
                .AlteredRetryPolicy(name)
                .ExecuteAsync(() =>
                    func(request));
        }
            
        public static AsyncPolicy<T> AlteredRetryPolicy<T>(this PolicyBuilder<T> p, string name) where T : IAlteredResponse
            => Policy.WrapAsync(
            p.WaitAndRetryAsync(
                DecorrelatedJitter(MaxRetries, MinDelay, MaxDelay),
                (response, timeSpan, numRetries, context) =>
                {
                    AlteredLog.Warning(new
                    {
                        Name = name,
                        response.Result.RequestId,
                        Response = new
                        {
                            StatusCode = (int)RetryStatusCode,
                            response.Result.RequestDuration,
                            RetryAfter = timeSpan.TotalMilliseconds,
                            NumRetries = numRetries,
                            Context = context.ToDictionary()
                            // response.Exception? already logged
                        }
                    });
                }),
            p.CircuitBreakerAsync(MaxCircuitFails, CircuitBreakDuration,
                (response, state, timeSpan, context) =>
                {
                    AlteredLog.Error(new
                    {
                        Name = name,
                        response.Result.RequestId,
                        Response = new
                        {
                            StatusCode = (int)CircuitBreakStatusCode,
                            response.Result.RequestDuration,
                            RetryAfter = timeSpan.TotalMilliseconds,
                            CircuitState = state,
                            Context = context.ToDictionary()
                        }
                    });
                },
                (context) =>
                {
                    AlteredLog.Error(new
                    {
                        Name = name,
                        Response = new
                        {
                            StatusCode = (int)CircuitResetStatusCode,
                            Context = context.ToDictionary()
                        }
                    });
                },
                () =>
                {
                    // half-open
                }));

        // https://github.com/App-vNext/Polly/wiki/Retry-with-jitter
        public static IEnumerable<TimeSpan> DecorrelatedJitter(int maxRetries, TimeSpan seedDelay, TimeSpan maxDelay)
        {
            Random jitterer = new Random();
            int retries = 0;

            double seed = seedDelay.TotalMilliseconds;
            double max = maxDelay.TotalMilliseconds;
            double current = seed;

            while (++retries <= maxRetries)
            {
                current = Math.Min(max, Math.Max(seed, current * 3 * jitterer.NextDouble())); // adopting the 'Decorrelated Jitter' formula from https://www.awsarchitectureblog.com/2015/03/backoff.html.  Can be between seed and previous * 3.  Mustn't exceed max.
                yield return TimeSpan.FromMilliseconds(current);
            }
        }

        static readonly int MaxRetries = 3;
        static readonly int MaxCircuitFails = 20;
        static readonly TimeSpan CircuitBreakDuration = TimeSpan.FromSeconds(10.0);

        static readonly TimeSpan MinDelay = TimeSpan.FromMilliseconds(100);
        static readonly TimeSpan MaxDelay = TimeSpan.FromSeconds(2);

        static readonly StatusCode RetryStatusCode = 420;
        static readonly StatusCode CircuitBreakStatusCode = 418;
        static readonly StatusCode CircuitResetStatusCode = 100;
    }
}
