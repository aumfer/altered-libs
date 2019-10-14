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
    public sealed class CloudwatchLogsSink : IDisposable
    {
        readonly Subject<JObject> logs = new Subject<JObject>();
        readonly BehaviorSubject<string> lastSequenceToken = new BehaviorSubject<string>(null);
        readonly IDisposable subscription;

        static readonly TimeSpan LogRate = TimeSpan.FromMilliseconds(100);
        static readonly int MaxLogs = 10000;

        public CloudwatchLogsSink(IAmazonCloudWatchLogs cwl, string logGroupName = null, string logStreamName = null)
        {
            logGroupName = logGroupName ?? "acl";
            logStreamName = logStreamName ?? $"{AlteredEnvironment.App}/{AlteredEnvironment.Env}/{AlteredEnvironment.Sha}/{Guid.NewGuid()}";

            var createLogRequest = new CreateLogStreamRequest
            {
                LogGroupName = logGroupName,
                LogStreamName = logStreamName
            };

            subscription =
                (from createLogResponse in cwl.CreateLogStreamAsync(createLogRequest).ToObservable()
                 from logBatch in logs
                     .SubscribeOn(NewThreadScheduler.Default)
                     .Buffer(LogRate, MaxLogs)
                 where logBatch.Count > 0
                 let logEvents = (from log in logBatch
                                  select new InputLogEvent
                                  {
                                      Timestamp = DateTime.UtcNow,
                                      Message = $"{log}"
                                  }).ToList()
                 let putLogsRequest = new PutLogEventsRequest
                 {
                     LogGroupName = logGroupName,
                     LogStreamName = logStreamName,
                     SequenceToken = lastSequenceToken.Value,
                     LogEvents = logEvents
                 }
                 from putLogsResponse in cwl.PutLogEventsAsync(putLogsRequest)
                     //let putLogsResponse = cwl.PutLogEventsAsync(putLogsRequest).Result
                 select putLogsResponse.NextSequenceToken)
                .Subscribe(lastSequenceToken);
        }

        public JObject Log(object log)
        {
            var message = JObject.FromObject(log, AlteredJson.DefaultJsonSerializer);
            logs.OnNext(message);
            return message;
        }

        public void Dispose()
        {
            logs?.Dispose();
            lastSequenceToken?.Dispose();
            subscription?.Dispose();
        }
    }
}
