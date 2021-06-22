using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Chat_and_Mail_Services.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the Chat_and_Mail_ServicesUser class
    public class Chat_and_Mail_ServicesUser : IdentityUser
    {
        public byte[] ProfilePhoto { get; set; }
    }
}
