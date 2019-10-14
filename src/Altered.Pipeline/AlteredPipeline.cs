using Altered.Pipeline.Pipelines;
using Altered.Shared;
using Altered.Shared.Extensions;
using Newtonsoft.Json.Linq;
using Polly;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Altered.Pipeline
{
    public interface IAlteredPipeline<TRequest, TResponse>
    {
        Task<TResponse> Execute(TRequest request);
    }

    public class AlteredPipeline<TRequest, TResponse> : IAlteredPipeline<TRequest, TResponse>
    {
        readonly Func<TRequest, Task<TResponse>> operation;

        public AlteredPipeline(IAlteredPipeline<TRequest, TResponse> AlteredPipeline) =>
            operation = AlteredPipeline.Execute;
        public AlteredPipeline(Func<TRequest, Task<TResponse>> f) =>
            operation =
                request => f(request);
        public AlteredPipeline(Func<TRequest, CancellationToken, Task<TResponse>> f) : this(r => f(r, default(CancellationToken)))
        { }

        public Task<TResponse> Execute(TRequest request) => operation(request);
        public TResponse ExecuteSync(TRequest request) => operation(request).Result;
    }

    public interface IAlteredPipelineEx<TRequest, TResponse>
    {
        IObservable<TResponse> Execute(IObservable<TRequest> requests);
    }

    public class AlteredPipelineEx<TRequest, TResponse> : IAlteredPipelineEx<TRequest, TResponse>, IAlteredPipeline<TRequest, TResponse>
    {
        readonly Func<IObservable<TRequest>, IObservable<TResponse>> operation;

        public AlteredPipelineEx(IAlteredPipeline<TRequest, TResponse> AlteredPipeline) =>
            operation = requests => from request in requests
                                    from response in AlteredPipeline.Execute(request)
                                    select response;
        public AlteredPipelineEx(Func<IObservable<TRequest>, IObservable<TResponse>> p) =>
            operation = p;

        public virtual Task<TResponse> Execute(TRequest request) => operation(Observable.Return(request)).ToTask();

        public IObservable<TResponse> Execute(IObservable<TRequest> requests) => operation(requests);
    }

    public static class AlteredPipelineExtensions
    {
        public static Func<TRequest, Task<TResponse>> AsFunc<TRequest, TResponse>(this IAlteredPipeline<TRequest, TResponse> p) =>
    p.Execute;
        public static Task<TResponse> Execute<TRequest, TResponse>(this IAlteredPipeline<IEnumerable<TRequest>, TResponse> p, TRequest single) => p
            .Execute(new TRequest[] { single });
        public static IEnumerable<T> Use<T>(this T t) where T : IDisposable
        {
            try
            {
                yield return t;
            }
            finally
            {
                t?.Dispose();
            }
        }

        public static Func<TRequest, Task<TResponse>> WithAlteredPipeline<TRequest, TResponse>(this Func<TRequest, Task<TResponse>> operation, [CallerMemberName] string name = null,
            Func<PolicyBuilder<TResponse>, string, AsyncPolicy<TResponse>> retryPolicy = null,
            Action<TRequest> requestLog = null,
            Action<TResponse> responseLog = null)
            where TRequest : IAlteredRequest
            where TResponse : IAlteredResponse, new() => operation
            //.WithXRaySegment(name)
            .WithUnhandledException(name)
            .WithRetry(name, retryPolicy)
            .WithCopyRequestId()
            .WithLogRequest(requestLog ?? (request =>
            {
                AlteredLog.Information(new
                {
                    Name = name,
                    request.RequestId,
                    Request = request
                });
            }))
            .WithCreateRequestId()
            .WithMeasureRequestDuration()
            .WithLogResponse(responseLog ?? (response =>
            {
                AlteredLog.Information(new
                {
                    Name = name,
                    response.RequestId,
                    Response = response
                });
            }));        
    }
}
