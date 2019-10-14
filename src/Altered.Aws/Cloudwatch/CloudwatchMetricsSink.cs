using Altered.Shared;
using Altered.Shared.Interfaces;
using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Altered.Aws.Cloudwatch
{
    public sealed class CloudwatchMetricsSink : IDisposable
    {
        readonly Subject<PutMetricDataRequest> metrics = new Subject<PutMetricDataRequest>();
        readonly IDisposable metricsSubscription;

        public CloudwatchMetricsSink(IAmazonCloudWatch cloudwatch)
        {
            metricsSubscription =
                (from putMetricDataRequest in metrics
                     .SubscribeOn(NewThreadScheduler.Default)
                 from putMetricDataResponse in cloudwatch.PutMetricDataAsync(putMetricDataRequest)
                     // switching to sync mode is useful for debugging
                     //let putMetricDataResponse = cloudwatch.PutMetricDataAsync(putMetricDataRequest).Result
                 select putMetricDataResponse)
                 .Subscribe();
        }

        public void PutMetricData(PutMetricDataRequest putMetricDataRequest)
        {
            metrics.OnNext(putMetricDataRequest);
        }

        public void Dispose()
        {
            metricsSubscription?.Dispose();
        }
    }
}
