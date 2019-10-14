using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Altered.Mvc
{
    public static class HttpResponseExtensions
    {
        public static HttpStatusCode HttpStatusCode(this HttpResponse httpResponse) =>
            (HttpStatusCode)httpResponse.StatusCode;
    }
}
