using CsvHelper.Configuration.Attributes;
using System;

namespace Altered.Aws.Elb
{
    // https://docs.aws.amazon.com/elasticloadbalancing/latest/classic/access-log-collection.html
    public sealed class ElbLogEntry
    {
        [Index(0)]
        public DateTime Timestamp { get; set; }

        [Index(1)]
        public string Elb { get; set; }

        [Index(2)]
        public string ClientIpPort { get; set; }

        [Index(3)]
        public string BackendIpPort { get; set; }

        [Index(4)]
        public double RequestProcessingTime { get; set; }

        [Index(5)]
        public double BackendProcessingTime { get; set; }

        [Index(6)]
        public double ResponseProcessingTime { get; set; }

        [Index(7)]
        public int ElbStatusCode { get; set; }

        [Index(8)]
        public int BackendStatusCode { get; set; }

        [Index(9)]
        public int ReceivedBytes { get; set; }

        [Index(10)]
        public int SendBytes { get; set; }

        [Index(11)]
        public string Request { get; set; }

        [Index(12)]
        public string UserAgent { get; set; }

        [Index(13)]
        public string SslCipher { get; set; }

        [Index(14)]
        public string SslProtocol { get; set; }
    }
}
