using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalProperties.DATA;
using RentalProperties.Models;

namespace RentalProperties.Controllers
{
    public class MessageFromTenantsController : Controller
    {
        private readonly RentalPropertiesDBContext _context;

        public MessageFromTenantsController(RentalPropertiesDBContext context)
        {
            _context = context;
        }

        // GET: MessageFromTenants
        public async Task<IActionResult> Index()
        {
            var rentalPropertiesDBContext = _context.MessagesFromTenants.Include(m => m.Apartment).ThenInclude(a=>a.Property).Include(m => m.Tenant);
            return View(await rentalPropertiesDBContext.ToListAsync());
        }

        /*
        // GET: MessageFromTenants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageFromTenant = await _context.MessagesFromTenants
                .Include(m => m.Apartment)
                .Include(m => m.Tenant)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (messageFromTenant == null)
            {
                return NotFound();
            }

            return View(messageFromTenant);
        } */

        // GET: MessageFromTenants/Create
        [HttpGet("MessageFromTenants/Create")]
        [Authorize(Policy = "MustBeTenant")]
        public IActionResult Create(int apartmentId)
        {
            ViewData["ApartmentId"] = new SelectList(_context.Apartments.Where(a => a.ApartmentId == apartmentId), "ApartmentId", "ApartmentNumber");

            var currentUser = HttpContext.User;
            int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
            ViewData["TenantId"] = new SelectList(_context.UserAccounts.Where(u => u.UserId == userId), "UserId", "FullName");
            return View();
        }

        // POST: MessageFromTenants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Policy = "MustBeTenant")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageId,TenantId,ApartmentId,MessageSent,AnswerFromManager")] MessageFromTenant messageFromTenant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(messageFromTenant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments.Where(a => a.ApartmentId == messageFromTenant.ApartmentId), "ApartmentId", "ApartmentNumber");
            var currentUser = HttpContext.User;
            int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
            ViewData["TenantId"] = new SelectList(_context.UserAccounts.Where(u => u.UserId == userId), "UserId", "FullName"); return View(messageFromTenant);
        }

        // GET: MessageFromTenants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageFromTenant = _context.MessagesFromTenants
                .Include(m=>m.Tenant)
                .Include(m=>m.Apartment).ThenInclude(a=>a.Property)
                .FirstOrDefault(m=>m.MessageId==id);
            if (messageFromTenant == null)
            {
                return NotFound();
            }
            if (!MessageFromTenantOrForManager(messageFromTenant))
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            if (messageFromTenant.AnswerFromManager != null)
            {
                ViewData["ErrorMessage"] = "You can only edit messages that were not replied.";
                ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentNumber", messageFromTenant.ApartmentId);
                ViewData["TenantId"] = new SelectList(_context.UserAccounts, "UserId", "FullName", messageFromTenant.TenantId);
                return View(messageFromTenant);
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentNumber", messageFromTenant.ApartmentId);
            ViewData["TenantId"] = new SelectList(_context.UserAccounts, "UserId", "FullName", messageFromTenant.TenantId);
            return View(messageFromTenant);
        }

        // POST: MessageFromTenants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MessageId,TenantId,ApartmentId,MessageSent,AnswerFromManager")] MessageFromTenant messageFromTenant)
        {
            if (id != messageFromTenant.MessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(messageFromTenant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageFromTenantExists(messageFromTenant.MessageId))
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
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentNumber", messageFromTenant.ApartmentId);
            ViewData["TenantId"] = new SelectList(_context.UserAccounts, "UserId", "FullName", messageFromTenant.TenantId);
            return View(messageFromTenant);
        }

        [Authorize(Policy = "MustBeTenant")]
        // GET: MessageFromTenants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageFromTenant = await _context.MessagesFromTenants
                .Include(m => m.Apartment)
                .Include(m => m.Tenant)
                .FirstOrDefaultAsync(m => m.MessageId == id);
            if (messageFromTenant == null)
            {
                return NotFound();
            }

            return View(messageFromTenant);
        }

        // POST: MessageFromTenants/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "MustBeTenant")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var messageFromTenant = await _context.MessagesFromTenants.FindAsync(id);
            if (messageFromTenant != null)
            {
                _context.MessagesFromTenants.Remove(messageFromTenant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageFromTenantExists(int id)
        {
            return _context.MessagesFromTenants.Any(e => e.MessageId == id);
        }

        private bool MessageFromTenantOrForManager(MessageFromTenant message)
        {
            var currentUser = HttpContext.User;
            int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (message.TenantId == userId || message.Apartment.Property.ManagerId == userId)
            {
                return true;
            }
            return false;
        }
    }
}
