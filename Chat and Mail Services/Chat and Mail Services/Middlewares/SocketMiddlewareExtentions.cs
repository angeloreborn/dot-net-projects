using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Chat_and_Mail_Services.Middlewares.GlobalChatMiddleware;

namespace Chat_and_Mail_Services.Middlewares
{
    public static class SocketMiddlewareExtentions
    {
        public static IApplicationBuilder UseGlobalChat(
            this IApplicationBuilder builder, GlobalChatOptions options)
        {
            return builder.UseMiddleware<GlobalChatMiddleware>(options);
        }

        public static IApplicationBuilder Use(
        this IApplicationBuilder builder, GlobalChatOptions options)
        {
            return builder.UseMiddleware<GlobalChatMiddleware>(options);
        }

    }
}
