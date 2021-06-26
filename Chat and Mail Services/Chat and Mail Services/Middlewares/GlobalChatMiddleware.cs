using Chat_and_Mail_Services.Areas.Identity.Data;
using Chat_and_Mail_Services.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chat_and_Mail_Services.Middlewares
{
    public class GlobalChatMiddleware : Controller
    {
        public readonly GlobalChatOptions _options;
        private readonly RequestDelegate _next;
       // private readonly UserManager<ServicesUser> _userManager;
        public List<WebSocket> sockets = new();
        
        public GlobalChatMiddleware(RequestDelegate next, GlobalChatOptions options)
        {
            _options = options;
            _next = next;
        }

        // Invoke Socket Middleware
        public async Task InvokeAsync(HttpContext httpContext, UserManager<ServicesUser> userManager)
        {
            if (_options.AccessRoutes.Contains(httpContext.Request.Path))
            {
                if (httpContext.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
                    sockets.Add(webSocket);
                    await EchoAsync(httpContext, webSocket, userManager);
                }
            }
            await _next(httpContext);
        }
        private static class BufferSizes
        {
            public static int DefaultBufferSize = 1024 * 8;
        }
        private async Task EchoAsync(HttpContext context, WebSocket webSocket, UserManager<ServicesUser> userManager)
        {
            var buffer = new byte[BufferSizes.DefaultBufferSize];

            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            
            while (!result.CloseStatus.HasValue)
            {
                foreach (WebSocket _webSocket in sockets)
                {
                    ServicesUser user = await userManager.FindByNameAsync(context.User.Identity.Name);

                    Message message = new()
                    {
                        Author = new()
                        {
                            Email = context.User.Identity.Name,
                            ProfileImageUrl = user.ProfilePhotoUid
                        },
                        Content = new()
                        {
                            Data = Encoding.UTF8.GetString(buffer).TrimEnd('\0')
                        },
                        SendDate = DateTime.Now
                    };

                    byte[] messageBuffer = ObjectToByteArray(message);
                
                    if (_webSocket.State == WebSocketState.Open)
                    {
                        await _webSocket.SendAsync(new ArraySegment<byte>(messageBuffer, 0, messageBuffer.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    }
                }
                buffer = new byte[BufferSizes.DefaultBufferSize];
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        public class Data
        {
            string DataLink { get; set; }
        }
        public class Message
        {
            public Author Author { get; set; }
            public MessageContent Content { get; set; }

            public DateTime SendDate { get; set; }
        }

        public class MessageContent
        {
            public string Data { get; set; }
        }
        public class Author
        {
            public string Email { get; set; }
            public Guid ProfileImageUrl { get; set; }
        }

        public static class SpecialReservedChars
        {
            public static string VERTICAL_BAR = "|";
        }
        private static byte[] ObjectToByteArray(Object objectToConvert)
        {
            if (objectToConvert == null) return null;
            string objectJson = JsonConvert.SerializeObject(objectToConvert);
            byte[] objectBytes = Encoding.UTF8.GetBytes(objectJson);
            string objectBase64 = SpecialReservedChars.VERTICAL_BAR + Convert.ToBase64String(objectBytes);
            return Encoding.ASCII.GetBytes(objectBase64);
        }


        public class GlobalChatOptions
        { 
            public List<string> AccessRoutes { get; set; }
        }
    }
}
