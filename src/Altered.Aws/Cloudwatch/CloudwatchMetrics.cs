using Altered.Pipeline;
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
    public class CloudwatchMetrics<TRequest, TResponse> : AlteredPipeline<TRequest, TResponse>
        where TResponse : IStatusCode, IRequestDuration
    {
        public CloudwatchMetrics(IAlteredPipeline<TRequest, TResponse> operation, CloudwatchMetricsSink metrics, string name) : base(async (request) =>
        {
            var response = await operation.Execute(request);

            var dimensions = new List<Dimension>
                {
                    new Dimension
                    {
                        Name = nameof(AlteredEnvironment.App),
                        Value =  AlteredEnvironment.App ?? "-"
                    },
                    new Dimension
                    {
                        Name = nameof(AlteredEnvironment.Env),
                        Value = AlteredEnvironment.Env ?? "-"
                    },
                    //new Dimension
                    //{
                    //    Name = nameof(AlteredEnvironment.Sha),
                    //    Value = AlteredEnvironment.Sha ?? "-"
                    //},
                    new Dimension
                    {
                        Name = "Name",
                        Value = name ?? "-"
                    }
                };

            var metricData = new List<MetricDatum>
                {
                    new MetricDatum
                    {
                        MetricName = $"{name} Count",
                        TimestampUtc = DateTime.UtcNow,
                        Unit = StandardUnit.None,
                        Dimensions = dimensions
                    },
                    new MetricDatum
                    {
                        MetricName = $"{name} {nameof(response.RequestDuration)}",
                        TimestampUtc = DateTime.UtcNow,
                        Unit = StandardUnit.Milliseconds,
                        Dimensions = dimensions,
                        Value = response.RequestDuration
                    },

                    new MetricDatum
                    {
                        MetricName = $"{name} {response.StatusCode/100}XX Count",
                        TimestampUtc = DateTime.UtcNow,
                        Unit = StandardUnit.None,
                        Dimensions = dimensions
                    },
                    new MetricDatum
                    {
                        MetricName = $"{name} {response.StatusCode/100}XX {nameof(response.RequestDuration)}",
                        TimestampUtc = DateTime.UtcNow,
                        Unit = StandardUnit.Milliseconds,
                        Dimensions = dimensions,
                        Value = response.RequestDuration
                    }
                };

            var putMetricDataRequest = new PutMetricDataRequest
            {
                Namespace = AlteredEnvironment.Name,
                MetricData = metricData
            };

            metrics.PutMetricData(putMetricDataRequest);

            return response;
        })
        { }
    }

    public static class CloudwatchMetricsExtensions
    {
        public static IServiceCollection AddCloudwatchMetrics(this IServiceCollection services) => services
            .AddAlteredAws()
            .AddSingleton<CloudwatchMetricsSink>();

        public static IAlteredPipeline<TRequest, TResponse> WithCloudwatchMetrics<TRequest, TResponse>(this IAlteredPipeline<TRequest, TResponse> operation, CloudwatchMetricsSink metrics, [CallerMemberName] string name = null)
            where TResponse : IStatusCode, IRequestDuration =>
            new CloudwatchMetrics<TRequest, TResponse>(operation, metrics, name);
    }
}
