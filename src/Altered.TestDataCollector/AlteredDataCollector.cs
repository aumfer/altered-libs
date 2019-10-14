using Altered.Shared;
using Altered.Shared.Extensions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Serilog;
using System;
using Altered.Pipeline;
using Newtonsoft.Json;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Serilog.Core;
using System.Diagnostics;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Altered.Aws.Cloudwatch;
using Altered.Aws;

namespace Altered.TestDataCollector
{
    [DataCollectorFriendlyName("AlteredDataCollector")]
    [DataCollectorTypeUri("my://altered/datacollector")]
    public class AlteredDataCollector : DataCollector//, ITestExecutionEnvironmentSpecifier
    {
        //static readonly Dictionary<string, string> Environment = new Dictionary<string, string>
        //{
        //    { "AWS_REGION", "us-east-1" }
        //};

        static readonly string BuildArn = Environment.GetEnvironmentVariable("CODEBUILD_BUILD_ARN");

        Lazy<CloudwatchLogsSink> log;
        DataCollectionLogger log2;

        readonly ConcurrentDictionary<string, Stopwatch> clocks = new ConcurrentDictionary<string, Stopwatch>();

        public override void Initialize(
            System.Xml.XmlElement configurationElement,
            DataCollectionEvents events,
            DataCollectionSink dataSink,
            DataCollectionLogger logger,
            DataCollectionEnvironmentContext environmentContext)
        {
            log = new Lazy<CloudwatchLogsSink>(() =>
                    // have to wait as late as possible for DataCollector to bind the right assembly versions
                     new ServiceCollection()
                    .AddAlteredAws()
                    .AddCloudwatchLogs()
                    .BuildServiceProvider()
                    .GetService<CloudwatchLogsSink>());

            log2 = logger;

            events.TestHostLaunched += TestHostLaunched_Handler;
            events.SessionStart += SessionStarted_Handler;
            events.SessionEnd += SessionEnded_Handler;
            events.TestCaseStart += Events_TestCaseStart;
            events.TestCaseEnd += Events_TestCaseEnd;

            dataSink.SendFileCompleted += DataSink_SendFileCompleted;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (log.IsValueCreated)
                {
                    log.Value.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        //public IEnumerable<KeyValuePair<string, string>> GetTestExecutionEnvironmentVariables() => Environment;

        void TestHostLaunched_Handler(object sender, TestHostLaunchedEventArgs e)
        {
            log.Value.Log(new
            {
                Name = "TestHostLaunched",
                RequestId = BuildArn ?? $"{e.Context?.SessionId?.Id}",
            });
            log2.LogWarning(e.Context, "TestHostLaunched");
        }

        void SessionStarted_Handler(object sender, SessionStartEventArgs e)
        {
            clocks.AddOrUpdate($"{e.Context?.SessionId?.Id}", _ => Stopwatch.StartNew(), (_,s) => s);
            log.Value.Log(new
            {
                Name = "TestSessionStarted",
                RequestId = BuildArn ?? $"{e.Context?.SessionId?.Id}",
            });
            log2.LogWarning(e.Context, "SessionStarted");
        }

        void SessionEnded_Handler(object sender, SessionEndEventArgs e)
        {
            clocks.TryGetValue($"{e.Context?.SessionId?.Id}", out Stopwatch clock);
            log.Value.Log(new
            {
                Name = "TestSessionEnded",
                RequestId = BuildArn ?? $"{e.Context?.SessionId?.Id}",
                Response = new
                {
                    StatusCode = 200,
                    RequestDuration = clock.Elapsed.TotalMilliseconds
                }
            });
            log2.LogWarning(e.Context, "SessionEnded");
        }

        void Events_TestCaseStart(object sender, TestCaseStartEventArgs e)
        {
            clocks.AddOrUpdate($"{e.Context?.SessionId?.Id}:{e.TestCaseId}", _ => Stopwatch.StartNew(), (_, s) => s);
            log.Value.Log(new 
            {
                Name = "TestCaseStart",
                RequestId = BuildArn ?? $"{e.Context?.SessionId?.Id}",
                Request = new
                {
                    TestCaseId = $"{e.TestCaseId}",
                    e.TestCaseName
                    //e.TestElement
                }
            });
            log2.LogWarning(e.Context, e.TestCaseName);
        }

        void Events_TestCaseEnd(object sender, TestCaseEndEventArgs e)
        {
            clocks.TryGetValue($"{e.Context?.SessionId?.Id}:{e.TestCaseId}", out Stopwatch clock);
            log.Value.Log(new 
            {
                Name = "TestCaseEnd",
                RequestId = BuildArn ?? $"{e.Context?.SessionId?.Id}",
                Response = new
                {
                    RequestDuration = clock.Elapsed.TotalMilliseconds,
                    TestCaseId = $"{e.TestCaseId}",
                    e.TestCaseName,
                    e.TestOutcome
                }
            });
            log2.LogWarning(e.Context, e.TestCaseName);
        }

        void DataSink_SendFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
        }
    }
}
