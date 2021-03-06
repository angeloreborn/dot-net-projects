using System;
using Chat_and_Mail_Services.Areas.Identity.Data;
using Chat_and_Mail_Services.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Chat_and_Mail_Services.Areas.Identity.IdentityHostingStartup))]
namespace Chat_and_Mail_Services.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<Identity_Context>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("Identity_ContextConnection")));

                services.AddDefaultIdentity<ServicesUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<Identity_Context>();
            });
        }
    }
}