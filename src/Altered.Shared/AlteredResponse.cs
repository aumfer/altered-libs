using System;
using System.Net;

namespace Altered.Shared
{
    // use for 'empty' requests, inherit if you must
    public class AlteredResponse : IAlteredResponse
    {
        public string RequestId { get; set; }
        public StatusCode StatusCode { get; set; } = HttpStatusCode.NotImplemented;
        public double RequestDuration { get; set; }
    }
}
