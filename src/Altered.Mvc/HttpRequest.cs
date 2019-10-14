using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Mvc
{
    public static class HttpRequestExtensions
    {
        public static Uri GetUriAuthority(this HttpRequest httpRequest)
        {
            string host = httpRequest.Host.ToUriComponent();
            UriBuilder uriBuilder = new UriBuilder($"http://{host}");

            // If we are running on AWS behind an ELB get the scheme from the host header
            if (httpRequest.Headers.TryGetValue("X-Forwarded-Proto", out StringValues scheme))
            {
                // Clear the port number from the request by setting the port number to -1
                uriBuilder.Port = -1;
                uriBuilder.Scheme = scheme;
            }

            return uriBuilder.Uri;
        }
    }
}
