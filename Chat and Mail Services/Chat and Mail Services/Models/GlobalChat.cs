using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat_and_Mail_Services.Models
{

    public class GlobalChat
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string MessageContent { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
