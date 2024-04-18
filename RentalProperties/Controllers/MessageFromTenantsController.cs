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
    [Authorize]
    public class MessageFromTenantsController : Controller
    {
        private readonly RentalPropertiesDBContext _context;

        public MessageFromTenantsController(RentalPropertiesDBContext context)
        {
            _context = context;
        }

        // GET: MessageFromTenants/Index/1
        [HttpGet("MessageFromTenants/Index/{conversationId}")]
        public async Task<IActionResult> Index(int conversationId)
        {
            if (conversationId == null)
            {
                return NotFound();
            }

            var messages = _context.MessagesFromTenants.Where(m=>m.ConversationId == conversationId).AsQueryable();
            
            ConversationWithMessages conversationToOpen = new ConversationWithMessages();
            conversationToOpen.AllMessages = await messages.ToListAsync();
            var newMessage = new MessageFromTenant();
            newMessage.ConversationId = conversationId;
            conversationToOpen.newMessage = newMessage;

            var apartId = _context.Conversations.Where(c => c.ConversationId == conversationId).First().ApartmentId;
            var apartNumber = _context.Apartments.First(a => a.ApartmentId == apartId).ApartmentNumber;
            var propertyName = _context.Apartments.Include(a=>a.Property).First(a => a.ApartmentId == apartId).Property.PropertyName;

            ViewData["apNb"] = apartNumber;
            ViewData["PropName"] = propertyName;
            return View(conversationToOpen);
        }

        //GET : MessageFromTenants/StartConversation/1
        public async Task<IActionResult> StartConversation(int apartmentId)
        {
            if (await RentalWebsite.UserHasPolicy(HttpContext, "MustBeTenant"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);


                var pastMessages = _context.Conversations.FirstOrDefault(c => c.TenantId == userId && c.ApartmentId == apartmentId);
                if (pastMessages == null)
                {
                    Conversation newConversation = new Conversation();
                    newConversation.TenantId = userId;
                    newConversation.ApartmentId = apartmentId;
                    _context.Add(newConversation);
                    await _context.SaveChangesAsync();
                }

                int convId = _context.Conversations.FirstOrDefault(c => c.TenantId == userId && c.ApartmentId == apartmentId).ConversationId;
                return RedirectToAction("Index", "MessageFromTenants", new { conversationId = convId });
            }
            else
            {
                return NotFound();
            }
        }

            // GET: MessageFromTenants/Create
            [HttpGet("MessageFromTenants/Create")]
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MessageId,ConversationId,MessageSent")] MessageFromTenant newMessage)
        {
            if (ModelState.IsValid)
            {
                //Settin Message
                newMessage.DateSent = DateTime.Now;

                //setting Author
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                var user = _context.UserAccounts.FirstOrDefault(u => u.UserId == userId);
                newMessage.AuthorName = user.FullName;
                newMessage.AuthorType = user.UserType;

                _context.Add(newMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","MessageFromTenants",new { conversationId = newMessage.ConversationId});
            }

            return View(newMessage);
        }

        // GET: MessageFromTenants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messageFromTenant = _context.MessagesFromTenants
                .FirstOrDefault(m=>m.MessageId==id);
            if (messageFromTenant == null)
            {
                return NotFound();
            }
            //if (! await MessageFromTenantOrForManager(messageFromTenant))
            //{
            //    return RedirectToAction("AccessDenied", "Home");
            //}
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

        //private async Task<bool> MessageFromTenantOrForManager(MessageFromTenant message)
        //{
        //    var currentUser = HttpContext.User;
        //    int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);

        //    if (message.TenantId == userId || 
        //        message.Apartment.Property.ManagerId == userId ||
        //        await RentalWebsite.UserHasPolicy(HttpContext,"MustBeOwnerOrAdministrator"))
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }
}
