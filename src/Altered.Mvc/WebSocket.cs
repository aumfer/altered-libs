using Altered.Pipeline;
using Altered.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altered.Mvc
{
    public static class WebSocketExtensions
    {
        // translate MvcRequest/Response into aspnet RequestDelegate
        public static RequestDelegate ToWebSocket(this IAlteredPipeline<AlteredApiRequest, AlteredApiResponse> pipeline) =>
            async (httpContext) =>
            {
                var request = httpContext.Request;

                if (httpContext.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
                    var buffer = new byte[16384];
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), default(CancellationToken));
                    while (!webSocket.CloseStatus.HasValue)
                    {
                        var message = await webSocket.ReceiveMessage();
                        var apiRequest = JsonConvert.DeserializeObject<AlteredApiRequest>(message, AlteredJson.DefaultJsonSerializerSettings);
                        var apiResponse = await pipeline.Execute(apiRequest);
                        // ignore
                    }
                }
                else
                {
                    // fallback
                    await pipeline.ToAspNet()(httpContext);
                }
            };

        public static async Task<string> ReceiveMessage(this WebSocket webSocket, int chunkSize = 16384)
        {
            var buffer = new byte[16384];
            var receive = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), default(CancellationToken));
            var sb = new StringBuilder();
            while (!receive.CloseStatus.HasValue && !receive.EndOfMessage)
            {
                var msg = Encoding.UTF8.GetString(buffer, 0, receive.Count);
                sb.Append(msg);

                if (receive.EndOfMessage)
                {
                    return sb.ToString();
                }
            }
            throw new Exception($"WebSocket closed during {nameof(ReceiveMessage)} after reading {sb.Length} bytes with status {receive.CloseStatus}: {receive.CloseStatusDescription}");
        }
    }
}
