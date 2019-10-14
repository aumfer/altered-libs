using Altered.Pipeline;
using Altered.Shared;
using Altered.Shared.Interfaces;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Text;

namespace Altered.Aws.Cloudwatch
{
    public class CloudwatchLogs<TRequest, TResponse> : AlteredPipeline<TRequest, TResponse>
        where TRequest : IRequestId
        where TResponse : IRequestId, IStatusCode, IRequestDuration, new()
    {
        public CloudwatchLogs(IAlteredPipeline<TRequest, TResponse> operation, CloudwatchLogsSink logs, string name) : base(async (request) =>
        {
            var requestLog = new
            {
                Time = DateTime.UtcNow,
                AlteredEnvironment.App,
                AlteredEnvironment.Env,
                AlteredEnvironment.Sha,
                Name = name,
                request.RequestId,
                Request = request
            };
            logs.Log(requestLog);

            var response = await operation.Execute(request);

            Debug.Assert(request.RequestId == response.RequestId);
            var responseLog = new
            {
                Time = DateTime.UtcNow,
                AlteredEnvironment.App,
                AlteredEnvironment.Env,
                AlteredEnvironment.Sha,
                Name = name,
                response.RequestId,
                Response = response
            };
            logs.Log(responseLog);

            return response;
        })
        { }
    }

    public static class CloudwatchLogsExtensions
    {
        public static IServiceCollection AddCloudwatchLogs(this IServiceCollection services) => services
            .AddAlteredAws()
            .AddSingleton<CloudwatchLogsSink>();

        public static IAlteredPipeline<TRequest, TResponse> WithCloudwatchLogs<TRequest, TResponse>(this IAlteredPipeline<TRequest, TResponse> operation, CloudwatchLogsSink logs, [CallerMemberName] string name = null)
            where TRequest : IRequestId
            where TResponse : IRequestId, IStatusCode, IRequestDuration, new() =>
            new CloudwatchLogs<TRequest, TResponse>(operation, logs, name);
    }
}
