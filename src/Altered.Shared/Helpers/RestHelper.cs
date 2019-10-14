using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Altered.Shared.Helpers
{
    public static class RestHelper
    {
        public static Func<TRequest, Task<TResponse>> CreateRestClient<TRequest, TResponse>(
            Func<TRequest, Uri, HttpRequestMessage> mapRequest,
            Func<HttpResponseMessage, Task<TResponse>> mapResponse,
            Uri baseUri,
            HttpClient httpClient = null)
        {
            httpClient = httpClient ?? new HttpClient();
            return async (request) =>
            {
                var requestMessage = mapRequest(request, baseUri);
                var responseMessage = await httpClient.SendAsync(requestMessage);
                var response = await mapResponse(responseMessage);
                return response;
            };
        }
    }
}
