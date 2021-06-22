using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Chat_and_Mail_Services.Middlewares
{
    public class GlobalChatMiddleware
    {
        public readonly GlobalChatOptions _options;
        private readonly RequestDelegate _next;
        public List<WebSocket> sockets = new();
        
        public GlobalChatMiddleware(RequestDelegate next, GlobalChatOptions options)
        {
            _options = options;
            _next = next;
        }

        // Invoke Socket Middleware
        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (_options.AccessRoutes.Contains(httpContext.Request.Path))
            {
                if (httpContext.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
                    sockets.Add(webSocket);
                    await EchoAsync(httpContext, webSocket);
                }
            }
            await _next(httpContext);
        }
        private async Task EchoAsync(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {

                foreach (WebSocket _webSocket in sockets)
                {
                    if (_webSocket.State == WebSocketState.Open)
                    {
                        await _webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    }

                }
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        public class GlobalChatOptions
        { 
            public List<string> AccessRoutes { get; set; }
        }
    }
}
