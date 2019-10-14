using Altered.Pipeline.Serilog;
using Amazon;
using Amazon.CloudWatchLogs;
using Destructurama;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.AwsCloudWatch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Altered.Pipeline
{
    public static class LoggerConfigurationExtensions
    {
        public static T EnsureAwsRegion<T>(this T t, string region = null)
        {
            if (AWSConfigs.AWSRegion == null)
            {
                region = region ?? Environment.GetEnvironmentVariable("AWS_REGION") ?? Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION")  ?? DefaultAwsRegion;
                AWSConfigs.AWSRegion = region;
            }
            else if (region != null)
            {
                Debug.Assert(AWSConfigs.AWSRegion == region);
            }
            return t;
        }
        // warning: using this function directly does not enable Destructured.JsonNetTypes
        // (for situations where jsonnet isn't available / or max perf is needed)
        // prefer AlteredLogExtensions.AddAlteredLog
        public static LoggerConfiguration WithAlteredDefault(this LoggerConfiguration lc, string cloudwatchLogGroup = null) => lc
            .MinimumLevel.Information()
#if DEBUG
            .WriteTo.Console()
#else
            .WriteTo.Console(LogEventLevel.Warning)
#endif
            .EnsureAwsRegion()
            .WriteTo.AmazonCloudWatch(new CloudWatchSinkOptions
            {
                LogGroupName = cloudwatchLogGroup ?? DefaultLogGroupName,
                TextFormatter = new CompactJsonFormatter()
            }, new AmazonCloudWatchLogsClient(RegionEndpoint.USEast1))

            .Enrich.WithEnvironmentVariable("repo", "TF_VAR_repo_name")
            .Enrich.WithEnvironmentVariable("env", "TF_VAR_branch_name")
            .Enrich.WithEnvironmentVariable("sha", "TF_VAR_source_rev");

        public static readonly string DefaultLogGroupName = "acl";
        public static readonly string DefaultAwsRegion = RegionEndpoint.USEast1.SystemName;
    }
}
