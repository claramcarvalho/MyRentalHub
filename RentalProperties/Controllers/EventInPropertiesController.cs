using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalProperties.DATA;
using RentalProperties.Models;

namespace RentalProperties.Controllers
{
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
            var rentalPropertiesDBContext = _context.EventsInProperties.Include(e => e.Apartment).Include(e => e.Property);
            return View(await rentalPropertiesDBContext.ToListAsync());
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

            return View(eventInProperty);
        }

        // GET: EventInProperties/Create
        public IActionResult Create()
        {
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId");
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "PropertyId");
            return View();
        }

        // POST: EventInProperties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,PropertyId,ApartmentId,EventTitle,EventDescription,ReportDate")] EventInProperty eventInProperty)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventInProperty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", eventInProperty.ApartmentId);
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "PropertyId", eventInProperty.PropertyId);
            return View(eventInProperty);
        }

        // GET: EventInProperties/Edit/5
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
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", eventInProperty.ApartmentId);
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "PropertyId", eventInProperty.PropertyId);
            return View(eventInProperty);
        }

        // POST: EventInProperties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,PropertyId,ApartmentId,EventTitle,EventDescription,ReportDate")] EventInProperty eventInProperty)
        {
            if (id != eventInProperty.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
    }
}
