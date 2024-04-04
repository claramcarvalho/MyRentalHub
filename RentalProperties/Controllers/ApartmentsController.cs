using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalProperties.DATA;
using RentalProperties.Models;

namespace RentalProperties.Controllers
{
    [Authorize(Policy = "CantBeTenant")]
    public class ApartmentsController : Controller
    {
        private readonly RentalPropertiesDBContext _context;

        public ApartmentsController(RentalPropertiesDBContext context)
        {
            _context = context;
        }

        // GET: Apartments
        public async Task<IActionResult> Index()
        {
            var rentalPropertiesDBContext = _context.Apartments.Include(a => a.Property);
            return View(await rentalPropertiesDBContext.ToListAsync());
        }

        // GET: Apartments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments
                .Include(a => a.Property).Include(a=> a.Rentals).ThenInclude(r=>r.Tenant)
                .FirstOrDefaultAsync(m => m.ApartmentId == id);
            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        // GET: Apartments/Create
        [HttpGet("Apartments/Create")]
        public IActionResult Create()
        {
            var propertiesFromDatabase = _context.Properties;

            ViewData["PropertyId"] = new SelectList(
                propertiesFromDatabase, 
                "PropertyId", 
                "PropertyName");
            return View();
        }

        // GET: Apartments/Create/4
        [HttpGet("Apartments/Create/{propertyId}")]
        public IActionResult Create(int propertyId)
        {
            var propertiesFromDatabase = _context.Properties.Where(p => p.PropertyId == propertyId);

            ViewData["PropertyId"] = new SelectList(
                propertiesFromDatabase,
                "PropertyId",
                "PropertyName");
            return View();
        }

        // POST: Apartments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApartmentId,PropertyId,ApartmentNumber,NbOfBeds,NbOfBaths,NbOfParkingSpots,PriceAnnounced,AnimalsAccepted")] Apartment apartment)
        {
            if (ModelState.IsValid)
            {
                bool apartmentExists = _context.Apartments.Where(a => 
                    a.ApartmentNumber == apartment.ApartmentNumber && 
                    a.PropertyId == apartment.PropertyId).Any();

                //checking if ap nb is unique
                if (!apartmentExists)
                {
                    _context.Add(apartment);
                    await _context.SaveChangesAsync();

                    var referer = Request.Headers["Referer"].ToString();
                    bool endsWithNumber = char.IsDigit(referer[referer.Length - 1]);

                    if (endsWithNumber)
                    {
                        return RedirectToAction("Index", "Properties");
                    }
                    return RedirectToAction(nameof(Index));
                }
                ViewData["ErrorMessage"] = "This Property already has an apartment with that number. Please use an unique apartment number.";

                var propertiesFromDatabase = _context.Properties;
                ViewData["PropertyId"] = new SelectList(
                    propertiesFromDatabase,
                    "PropertyId",
                    "PropertyName");

                return View(apartment);
            }
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "PropertyId", apartment.PropertyId);
            return View(apartment);
        }

        // GET: Apartments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "PropertyName", apartment.PropertyId);
            return View(apartment);
        }

        // POST: Apartments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApartmentId,PropertyId,ApartmentNumber,NbOfBeds,NbOfBaths,NbOfParkingSpots,PriceAnnounced,AnimalsAccepted")] Apartment apartment)
        {
            if (id != apartment.ApartmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(apartment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApartmentExists(apartment.ApartmentId))
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
            ViewData["PropertyId"] = new SelectList(_context.Properties, "PropertyId", "PropertyId", apartment.PropertyId);
            return View(apartment);
        }

        // GET: Apartments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments
                .Include(a => a.Property)
                .FirstOrDefaultAsync(m => m.ApartmentId == id);
            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        // POST: Apartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment != null)
            {
                _context.Apartments.Remove(apartment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApartmentExists(int id)
        {
            return _context.Apartments.Any(e => e.ApartmentId == id);
        }

        [HttpGet]
        public IActionResult GetApartmentPrice(int apartmentId)
        {
            // Lógica para recuperar o preço do aluguel do apartamento com o ID especificado
            var price = _context.Apartments.Where(a => a.ApartmentId == apartmentId).Select(a => a.PriceAnnounced).FirstOrDefault();
            return Json(price);
        }
    }
}
