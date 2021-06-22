using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Chat_and_Mail_Services.Data;
using Chat_and_Mail_Services.Models;
using System.Net.WebSockets;
using Chat_and_Mail_Services.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace Chat_and_Mail_Services.Controllers
{
    public class GlobalChatsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public GlobalChatMiddleware _socketController;
        public GlobalChatsController(ApplicationDbContext context)
        {
            _context = context;
        }



        // GET: GlobalChats
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

    }
}
