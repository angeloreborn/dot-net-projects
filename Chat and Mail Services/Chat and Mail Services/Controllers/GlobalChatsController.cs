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
        public async Task<IActionResult> Index()
        {
            return View();
        }





        // GET: GlobalChats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var globalChat = await _context.GlobalChat
                .FirstOrDefaultAsync(m => m.Id == id);
            if (globalChat == null)
            {
                return NotFound();
            }

            return View(globalChat);
        }

        // GET: GlobalChats/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GlobalChats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,MessageContent,TimeStamp")] GlobalChat globalChat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(globalChat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(globalChat);
        }

        // GET: GlobalChats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var globalChat = await _context.GlobalChat.FindAsync(id);
            if (globalChat == null)
            {
                return NotFound();
            }
            return View(globalChat);
        }

        // POST: GlobalChats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,MessageContent,TimeStamp")] GlobalChat globalChat)
        {
            if (id != globalChat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(globalChat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GlobalChatExists(globalChat.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(globalChat);
        }

        // GET: GlobalChats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var globalChat = await _context.GlobalChat
                .FirstOrDefaultAsync(m => m.Id == id);
            if (globalChat == null)
            {
                return NotFound();
            }

            return View(globalChat);
        }

        // POST: GlobalChats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var globalChat = await _context.GlobalChat.FindAsync(id);
            _context.GlobalChat.Remove(globalChat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GlobalChatExists(int id)
        {
            return _context.GlobalChat.Any(e => e.Id == id);
        }
    }
}
