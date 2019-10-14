using Altered.Shared.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using Altered.Shared.Extensions;
using Altered.Shared;
using Altered.Shared.Extensions.NonGeneric;
using System.Diagnostics;

namespace Altered.Pipeline.Pipelines
{
    public static class UnhandledExceptionExtensions
    {
        public static Func<TRequest, Task<TResponse>> WithUnhandledException<TRequest, TResponse>(this Func<TRequest, Task<TResponse>> func, string name, Func<Exception, StatusCode> getErrorCode = null)
            where TRequest : IRequestId
            where TResponse : IStatusCode, new() => func.WithUnhandledException<TRequest, TResponse, Exception>(name);

        public static Func<TRequest, Task<TResponse>> WithUnhandledException<TRequest, TResponse, TException>(this Func<TRequest, Task<TResponse>> func, string name, Func<TException, StatusCode> getErrorCode = null)
            where TRequest : IRequestId
            where TResponse : IStatusCode, new()
            where TException : Exception =>
            async (request) =>
            {
                var clock = Stopwatch.StartNew();
                try
                {
                    var response = await func(request);
                    return response;
                }
                catch (TException e)
                {
                    var errorCode = getErrorCode?.Invoke(e) ?? HttpStatusCode.InternalServerError;
                    var response = new TResponse
                    {
                        StatusCode = errorCode
                    };
                    AlteredLog.Error(new
                    {
                        Name = name,
                        request.RequestId,
                        Response = new
                        {
                            StatusCode = (int)response.StatusCode,
                            RequestDuration = clock.Elapsed.TotalMilliseconds,
                            Exception = new
                            {
                                //e.StackTrace,
                                StackTrace = new StackTrace(e).ToString(),
                                e.Source,
                                e.Message,
                                Data = e.Data.ToDictionary().ToDictionary(kvp => $"{kvp.Key}", kvp => $"{kvp.Value}")
                            }
                        }
                    });
                    return response;
                }
            };
    }
}
