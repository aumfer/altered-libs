using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Aws.Alb
{
    // https://docs.aws.amazon.com/elasticloadbalancing/latest/application/load-balancer-access-logs.html
    public sealed class AlbLogEntry
    {
        [Index(0)]
        public string Type { get; set; }

        [Index(1)]
        public DateTime Timestamp { get; set; }

        [Index(2)]
        public string Elb { get; set; }

        [Index(3)]
        public string ClientIpPort { get; set; }

        [Index(4)]
        public string TargetIpPort { get; set; }

        [Index(5)]
        public double RequestProcessingTime { get; set; }

        [Index(6)]
        public double TargetProcessingTime { get; set; }

        [Index(7)]
        public double ResponseProcessingTime { get; set; }

        [Index(8)]
        public int ElbStatusCode { get; set; }

        [Index(9)]
        public string TargetStatusCode { get; set; }

        [Index(10)]
        public int ReceivedBytes { get; set; }

        [Index(11)]
        public int SentBytes { get; set; }

        [Index(12)]
        public string Request { get; set; }

        [Index(13)]
        public string UserAgent { get; set; }

        [Index(14)]
        public string SslCipher { get; set; }

        [Index(15)]
        public string SslProtocol { get; set; }

        [Index(16)]
        public string TargetGroupArn { get; set; }

        [Index(17)]
        public string TraceId { get; set; }

        [Index(18)]
        public string DomainName { get; set; }

        [Index(19)]
        public string ChosenCertArn { get; set; }

        [Index(20)]
        public int MatchedRulePriority { get; set; }

        [Index(21)]
        public DateTime RequestCreationTime { get; set; }

        [Index(22)]
        public string ActionsExecuted { get; set; }

        [Index(23)]
        public string RedirectUrl { get; set; }

        [Index(24)]
        public string ErrorReason { get; set; }
    }
}
