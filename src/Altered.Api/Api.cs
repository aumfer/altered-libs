using Altered.Aws;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Api
{
    /**
     * these are http interfaces/shapes that we can get to from aspnet or lambda or w/e
     */
    public interface IApiRequest
    {
        string Path { get; set; }

        string HttpMethod { get; set; }

        IDictionary<string, StringValues> Headers { get; set; }

        IDictionary<string, StringValues> QueryStringParameters { get; set; }

        string Body { get; set; }
    }

    public interface IApiResposne
    {
        int StatusCode { get; set; }

        IDictionary<string, StringValues> Headers { get; set; }

        string Body { get; set; }
    }

    public class ApiRequest
    {
        public string Path { get; set; }

        public string HttpMethod { get; set; }

        public IDictionary<string, StringValues> Headers { get; set; } = new Dictionary<string, StringValues>();

        public IDictionary<string, StringValues> QueryStringParameters { get; set; } = new Dictionary<string, StringValues>();

        public string Body { get; set; }
    }

    public class ApiResponse
    {
        public int StatusCode { get; set; }

        public IDictionary<string, StringValues> Headers { get; set; } = new Dictionary<string, StringValues>();

        public string Body { get; set; }
    }
}
