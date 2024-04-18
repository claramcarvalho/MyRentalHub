using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalProperties.DATA;
using RentalProperties.Models;

namespace RentalProperties.Controllers
{
    public class ConversationsController : Controller
    {
        private readonly RentalPropertiesDBContext _context;

        public ConversationsController(RentalPropertiesDBContext context)
        {
            _context = context;
        }

        // GET: Conversations
        public async Task<IActionResult> Index()
        {
            var rentalPropertiesDBContext = _context.Conversations.Include(c => c.Apartment).ThenInclude(a=>a.Property).Include(c => c.Tenant).AsQueryable();

            if (await RentalWebsite.UserHasPolicy(HttpContext,"MustBeTenant"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                rentalPropertiesDBContext = rentalPropertiesDBContext.Where(c => c.TenantId == userId);
            } else if (await RentalWebsite.UserHasPolicy(HttpContext, "MustBeManager"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                rentalPropertiesDBContext = rentalPropertiesDBContext.Where(c => c.Apartment.Property.ManagerId == userId);
            }

            return View(await rentalPropertiesDBContext.ToListAsync());
        }

        // GET: Conversations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversation = await _context.Conversations
                .Include(c => c.Apartment)
                .Include(c => c.Tenant)
                .FirstOrDefaultAsync(m => m.ConversationId == id);
            if (conversation == null)
            {
                return NotFound();
            }

            return View(conversation);
        }

        // GET: Conversations/Create
        public IActionResult Create()
        {
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId");
            ViewData["TenantId"] = new SelectList(_context.UserAccounts, "UserId", "UserName");
            return View();
        }

        // POST: Conversations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConversationId,TenantId,ApartmentId")] Conversation conversation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(conversation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", conversation.ApartmentId);
            ViewData["TenantId"] = new SelectList(_context.UserAccounts, "UserId", "UserName", conversation.TenantId);
            return View(conversation);
        }

        // GET: Conversations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversation = await _context.Conversations
                .Include(c => c.Apartment)
                .Include(c => c.Tenant)
                .FirstOrDefaultAsync(m => m.ConversationId == id);
            if (conversation == null)
            {
                return NotFound();
            }

            return View(conversation);
        }

        // POST: Conversations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var conversation = await _context.Conversations.FindAsync(id);
            if (conversation != null)
            {
                _context.Conversations.Remove(conversation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConversationExists(int id)
        {
            return _context.Conversations.Any(e => e.ConversationId == id);
        }
    }
}
