using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Altered.Shared.Extensions
{
    public static class HttpStatusCodeExtensions
    {
        public static bool ShouldRetry(this HttpStatusCode httpStatusCode)
        {
            return httpStatusCode.Is5XX();
        }

        public static HttpStatusCode Highest(this IEnumerable<HttpStatusCode> httpStatusCodes) => httpStatusCodes
            .OrderByDescending(httpStatusCode => (int)httpStatusCode).FirstOrDefault();

        public static bool Is2XX(this HttpStatusCode httpStatusCode)
        {
            int code = (int)httpStatusCode;
            return code >= 200 && code < 300;
        }
        public static bool Is3XX(this HttpStatusCode httpStatusCode)
        {
            int code = (int)httpStatusCode;
            return code >= 300 && code < 400;
        }
        public static bool Is4XX(this HttpStatusCode httpStatusCode)
        {
            int code = (int)httpStatusCode;
            return code >= 400 && code < 500;
        }
        public static bool Is5XX(this HttpStatusCode httpStatusCode)
        {
            int code = (int)httpStatusCode;
            return code >= 500 && code < 600;
        }
    }
}
