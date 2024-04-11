using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RentalProperties.DATA;
using RentalProperties.Models;
using static System.Net.Mime.MediaTypeNames;

namespace RentalProperties.Controllers
{
    [Authorize(Policy = "CantBeTenant")]
    public class EventInPropertiesController : Controller
    {
        private readonly RentalPropertiesDBContext _context;

        public EventInPropertiesController(RentalPropertiesDBContext context)
        {
            _context = context;
        }

        // GET: EventInProperties
        public async Task<IActionResult> Index()
        {
            var rentalPropertiesDBContext = await GetListOfEvents();

            return View(rentalPropertiesDBContext);
        }

        // GET: EventInProperties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventInProperty = await _context.EventsInProperties
                .Include(e => e.Apartment)
                .Include(e => e.Property)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (eventInProperty == null)
            {
                return NotFound();
            }
            if (!CurrentUserIsAllowedToManageProperty(eventInProperty))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View(eventInProperty);
        }

        // GET: EventInProperties/Create
        [HttpGet("EventInProperties/Create")]
        public async Task<IActionResult> Create()
        {
            ViewData["PropertyId"] = await CreateSelectListOfProperties();
            ViewData["ApartmentId"] = CreateEmptyListOfApartments();

            return View();
        }

        // GET: EventInProperties/Create
        [HttpGet("EventInProperties/Create/{propertyId}")]
        public async Task<IActionResult> Create(int propertyId)
        {
            ViewData["PropertyId"] = await CreateSelectListOfProperties();
            ViewData["ApartmentId"] = await CreateSelectListOfApartmentsByProperty(propertyId);

            return View();
        }

        // POST: EventInProperties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,PropertyId,ApartmentId,EventTitle,EventDescription")] EventInProperty eventInProperty)
        {
            if (ModelState.IsValid)
            {
                List<string> errors = new List<string>();
                if (eventInProperty.PropertyId == 0)
                {
                    errors.Add("You must select at least a Property.");
                    ViewData["ErrorMessage"] = errors;
                    ViewData["PropertyId"] = await CreateSelectListOfProperties();
                    ViewData["ApartmentId"] = CreateEmptyListOfApartments();

                    return View();
                }
                eventInProperty.ReportDate = DateOnly.FromDateTime(DateTime.Now);
                if (eventInProperty.ApartmentId == 0)
                    eventInProperty.ApartmentId = null;
                _context.Add(eventInProperty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PropertyId"] = await CreateSelectListOfProperties();   
            ViewData["ApartmentId"] = CreateEmptyListOfApartments(); 
            return View(eventInProperty);
        }

        // GET: EventInProperties/Edit/5
        [HttpGet("EventInProperties/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventInProperty = await _context.EventsInProperties.FindAsync(id);
            if (eventInProperty == null)
            {
                return NotFound();
            }
            if (!CurrentUserIsAllowedToManageProperty(eventInProperty))
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            ViewData["ApartmentId"] = await CreateSelectListOfApartmentsByProperty(eventInProperty.PropertyId);
            ViewData["PropertyId"] = await CreateSelectListOfProperties();
            return View(eventInProperty);
        }

        // GET: EventInProperties/Edit/5/2
        [HttpGet("EventInProperties/Edit/{id}/{propertyId}")]
        public async Task<IActionResult> Edit(int? id, int propertyId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventInProperty = await _context.EventsInProperties.FindAsync(id);
            if (eventInProperty == null)
            {
                return NotFound();
            }
            if (!CurrentUserIsAllowedToManageProperty(eventInProperty))
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            ViewData["ApartmentId"] = await CreateSelectListOfApartmentsByProperty(propertyId);
            ViewData["PropertyId"] = await CreateSelectListOfProperties();
            return View(eventInProperty);
        }

        // POST: EventInProperties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("EventInProperties/Edit/{id}")]
        [HttpPost("EventInProperties/Edit/{id}/{propertyId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int propertyId, [Bind("EventId,PropertyId,ApartmentId,EventTitle,EventDescription,ReportDate")] EventInProperty eventInProperty)
        {
            if (id != eventInProperty.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                List<string> errors = new List<string>();
                if (eventInProperty.PropertyId == 0)
                {
                    errors.Add("You must select at least a Property.");
                    ViewData["ErrorMessage"] = errors;
                    ViewData["PropertyId"] = await CreateSelectListOfProperties();
                    ViewData["ApartmentId"] = CreateEmptyListOfApartments();

                    return View(eventInProperty);
                }
                eventInProperty.ReportDate = DateOnly.FromDateTime(DateTime.Now);
                if (eventInProperty.ApartmentId == 0)
                    eventInProperty.ApartmentId = null;
                try
                {
                    _context.Update(eventInProperty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventInPropertyExists(eventInProperty.EventId))
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
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", eventInProperty.ApartmentId);
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "PropertyId", eventInProperty.PropertyId);
            return View(eventInProperty);
        }

        // GET: EventInProperties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var eventInProperty = await _context.EventsInProperties
                .Include(e => e.Apartment)
                .Include(e => e.Property)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (eventInProperty == null)
            {
                return NotFound();
            }
            if (!CurrentUserIsAllowedToManageProperty(eventInProperty))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View(eventInProperty);
        }

        // POST: EventInProperties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventInProperty = await _context.EventsInProperties.FindAsync(id);
            if (eventInProperty != null)
            {
                _context.EventsInProperties.Remove(eventInProperty);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventInPropertyExists(int id)
        {
            return _context.EventsInProperties.Any(e => e.EventId == id);
        }

        private async Task<SelectList> CreateSelectListOfProperties()
        {
            var selectListProperties = _context.Properties.ToList();
            if (await RentalWebsite.UserHasPolicy(HttpContext, "MustBeManager"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                selectListProperties = selectListProperties.Where(a => a.ManagerId == userId).ToList();
            }

            //Creatting list of Apartments
            List<SelectListItem> list = new List<SelectListItem>();

            SelectListItem itemSelectFirst = new SelectListItem("Select a Property", "0");
            list.Add(itemSelectFirst);

            foreach (var item in selectListProperties)
            {
                string text = item.PropertyName.ToString();
                SelectListItem selectListItem = new SelectListItem(text, item.PropertyId.ToString());
                list.Add(selectListItem);
            }
            SelectList listToReturn = new SelectList(list, "Value", "Text");

            return listToReturn;
        }

        private async Task<SelectList> CreateSelectListOfApartmentsByProperty(int propertyId)
        {
            var selectListApartments = _context.Apartments.Include(a => a.Property).Where(p=>p.PropertyId==propertyId).ToList();
            if (await RentalWebsite.UserHasPolicy(HttpContext, "MustBeManager"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                selectListApartments = selectListApartments.Where(a => a.Property.ManagerId == userId).ToList();
            }

            //Creatting list of Apartments
            List<SelectListItem> list = new List<SelectListItem>();
            
            SelectListItem itemAllApartments = new SelectListItem("All Apartments", "0");
            list.Add(itemAllApartments);

            foreach (var item in selectListApartments)
            {
                string text = item.ApartmentId.ToString() + " - " + item.Property.PropertyName.ToString() + " - Apt " + item.ApartmentNumber.ToString();
                SelectListItem selectListItem = new SelectListItem(text, item.ApartmentId.ToString());
                list.Add(selectListItem);
            }
            
            SelectList listToReturn = new SelectList(list, "Value", "Text");

            return listToReturn;
        }

        private SelectList CreateEmptyListOfApartments()
        {
            var item = new SelectListItem("Select a Property first", "0");

            var listItems = new List<SelectListItem> { item };
            var selectListApartment = new SelectList(listItems, "Value", "Text");
            
            return selectListApartment;
        }

        private async Task<List<EventInProperty>> GetListOfEvents()
        {
            var list = _context.EventsInProperties.Include(e => e.Apartment).Include(e => e.Property).ThenInclude(p=>p.Manager).ToList();

            if (await RentalWebsite.UserHasPolicy(HttpContext, "MustBeManager"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                list = list.Where(e=>e.Property.ManagerId == userId).ToList();
            }

            return list;
        }

        private bool CurrentUserIsAllowedToManageProperty(EventInProperty @event)
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c =>
                c.Type == "Type" &&
                (
                    c.Value == "Manager"
                )))
            {
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);


                if (@event.Property.ManagerId != userId)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
