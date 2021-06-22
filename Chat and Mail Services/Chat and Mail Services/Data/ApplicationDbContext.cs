using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Chat_and_Mail_Services.Models;

namespace Chat_and_Mail_Services.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Chat_and_Mail_Services.Models.GlobalChat> GlobalChat { get; set; }
    }
}
