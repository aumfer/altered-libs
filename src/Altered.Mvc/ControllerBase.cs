using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net;

namespace Altered.Mvc
{
    // unfortunately you need to use "this." to access these extension methods from a controller
    // probably worth to switch to a base class just for that (just don't put any state in it)
    public static class ControllerBaseExtensions
    {
        public static void AddHeader(this ControllerBase controller, string name, string value)
        {
            controller.Response.Headers.Add(name, new StringValues(value));
        }
        public static IActionResult StatusCode(this ControllerBase controller, HttpStatusCode statusCode, object result = null) =>
            controller.StatusCode((int)statusCode, result);

        public static void AddETag(this HttpResponse response, string tag, bool isWeak = false) =>
            response.GetTypedHeaders().ETag = new EntityTagHeaderValue($"\"{tag?.Trim('"')}\"", isWeak);
        public static void AddETag(this HttpContext context, string tag, bool isWeak = false) =>
            AddETag(context.Response, tag, isWeak);
        public static void AddETag(this ControllerBase controller, string tag, bool isWeak = false) =>
            AddETag(controller.Response, tag, isWeak);

        public static string GetETag(this HttpRequest request) =>
            request.GetTypedHeaders().IfMatch?.FirstOrDefault()?.Tag.Value?.Trim('"');
        public static string GetETag(this HttpContext context) =>
            GetETag(context.Request);
        public static string GetETag(this ControllerBase controller) =>
            GetETag(controller.HttpContext);
    }
}
