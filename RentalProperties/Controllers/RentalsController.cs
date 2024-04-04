using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentalProperties.DATA;
using RentalProperties.Models;

namespace RentalProperties.Controllers
{
    [Authorize(Policy = "CantBeTenant")]
    public class RentalsController : Controller
    {
        private readonly RentalPropertiesDBContext _context;

        public RentalsController(RentalPropertiesDBContext context)
        {
            _context = context;
        }

        // GET: Rentals
        public async Task<IActionResult> Index()
        {
            var listProperties = _context.Properties;
            ViewData["Properties"] = new SelectList(listProperties, "PropertyId", "PropertyName");

            var rentalPropertiesDBContext = _context.Rentals
                .Include(r => r.Apartment).ThenInclude(a => a.Property)
                .Include(r => r.Tenant)
                .Where(t => t.Tenant.UserType == UserType.Tenant)
                .OrderBy(r => r.LastDayRental);

            return View(await rentalPropertiesDBContext.ToListAsync());
        }

        // GET: Rentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Apartment)
                .Include(r => r.Tenant)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // GET: Rentals/Create
        [HttpGet("Rentals/Create")]
        public IActionResult Create()
        {
            //Creatting list of Apartments
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in _context.Apartments
                .Include(a => a.Property))
            {
                string text = item.ApartmentId.ToString() + " - " + item.Property.PropertyName.ToString() + " - Apt " + item.ApartmentNumber.ToString();
                SelectListItem selectListItem = new SelectListItem(text, item.ApartmentId.ToString());
                list.Add(selectListItem);
            }
            SelectList listOfApartments = new SelectList(list, "Value", "Text");
            ViewData["ApartmentsId"] = listOfApartments;

            //Creating list of Tenants
            ViewData["TenantId"] = new SelectList(_context.UserAccounts.Where(u => u.UserType == UserType.Tenant), "UserId", "FullName");

            return View();
        }

        // GET: Rentals/Create/1
        [HttpGet("Rentals/Create/{apartmentId}")]
        public IActionResult Create(int apartmentId)
        {
            //Creatting list of Apartments
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in _context.Apartments
                .Include(a => a.Property)
                .Where(a => a.ApartmentId == apartmentId))
            {
                string text = item.ApartmentId.ToString() + " - " + item.Property.PropertyName.ToString() + " - Apt " + item.ApartmentNumber.ToString();
                SelectListItem selectListItem = new SelectListItem(text, item.ApartmentId.ToString());
                list.Add(selectListItem);
            }
            SelectList listOfApartments = new SelectList(list, "Value", "Text");
            ViewData["ApartmentsId"] = listOfApartments;

            //Creating list of Tenants
            ViewData["TenantId"] = new SelectList(_context.UserAccounts.Where(u => u.UserType == UserType.Tenant), "UserId", "FullName");

            return View();
        }

        // POST: Rentals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentalId,TenantId,ApartmentId,FirstDayRental,LastDayRental,PriceRent,RentalStatus")] Rental rental)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rental);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            List<SelectListItem> listOfApartments = new List<SelectListItem>();
            foreach (var item in _context.Apartments)
            {
                string text = item.ApartmentId.ToString() + " - " + item.ApartmentNumber.ToString() + item.Property.PropertyName.ToString();
                SelectListItem selectListItem = new SelectListItem(text, item.ApartmentId.ToString());
                listOfApartments.Add(selectListItem);
            }

            ViewData["ApartmentsId"] = listOfApartments;
            ViewData["TenantId"] = new SelectList(_context.UserAccounts, "UserId", "UserId", rental.TenantId);
            return View(rental);
        }

        // GET: Rentals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", rental.ApartmentId);
            ViewData["TenantId"] = new SelectList(_context.UserAccounts, "UserId", "UserName", rental.TenantId);
            return View(rental);
        }

        // POST: Rentals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RentalId,TenantId,ApartmentId,FirstDayRental,LastDayRental,PriceRent,RentalStatus")] Rental rental)
        {
            if (id != rental.RentalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rental);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.RentalId))
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
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", rental.ApartmentId);
            ViewData["TenantId"] = new SelectList(_context.UserAccounts, "UserId", "UserName", rental.TenantId);
            return View(rental);
        }

        // GET: Rentals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Apartment)
                .Include(r => r.Tenant)
                .FirstOrDefaultAsync(m => m.RentalId == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental != null)
            {
                _context.Rentals.Remove(rental);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.RentalId == id);
        }

        public IActionResult Search(
                string queryUserId,
                string queryUserName,
                string queryProperty,
                string queryApNb,
                string queryFDate,
                string queryLDate,
                string queryActualPrice,
                string queryAnnouncedPrice,
                string queryRentalStatus
                )
        {
            var filteredIds = _context.Rentals.Include(r => r.Apartment).ThenInclude(a => a.Property).Include(r => r.Tenant).Where(t => t.Tenant.UserType == UserType.Tenant).OrderBy(r => r.LastDayRental).ToList();
            if (!queryUserId.IsNullOrEmpty())
            {
                filteredIds = filteredIds.Where(u => u.TenantId == int.Parse(queryUserId)).ToList();
            }

            var filteredUserNames = filteredIds;
            if (!queryUserName.IsNullOrEmpty())
            {
                filteredUserNames = filteredUserNames.Where(u => u.Tenant.FullName.Contains(queryUserName)).ToList();
            }

            var filteredProperties = filteredUserNames;
            if (queryProperty.ToString() != "All")
            {
                filteredProperties = filteredProperties.Where(u => u.Apartment.Property.PropertyId == int.Parse(queryProperty)).ToList();
            }

            var filteredApNb = filteredProperties;
            if (!queryApNb.IsNullOrEmpty())
            {
                filteredApNb = filteredApNb.Where(u => u.Apartment.ApartmentNumber.Contains(queryApNb)).ToList();
            }

            var filteredFDate = filteredApNb;
            if (!queryFDate.IsNullOrEmpty())
            {
                filteredFDate = filteredFDate.Where(u => u.FirstDayRental == DateOnly.FromDateTime(DateTime.Parse(queryFDate).Date)).ToList();
            }

            var filteredLDate = filteredFDate;
            if (!queryLDate.IsNullOrEmpty())
            {
                filteredLDate = filteredLDate.Where(u => u.LastDayRental == DateOnly.FromDateTime(DateTime.Parse(queryLDate).Date)).ToList();
            }

            var filteredActualPrice = filteredLDate;
            if (!queryActualPrice.IsNullOrEmpty())
            {
                filteredActualPrice = filteredActualPrice.Where(u => u.PriceRent == decimal.Parse(queryActualPrice)).ToList();
            }

            var filteredAnnouncedPrice = filteredActualPrice;
            if (!queryAnnouncedPrice.IsNullOrEmpty())
            {
                filteredAnnouncedPrice = filteredAnnouncedPrice.Where(u => u.Apartment.PriceAnnounced == decimal.Parse(queryAnnouncedPrice)).ToList();
            }

            var filteredStatus = filteredAnnouncedPrice;
            if (queryRentalStatus.ToString() != "All")
            {
                filteredStatus = filteredStatus.Where(u => u.RentalStatus == (StatusOfRental)Enum.Parse(typeof(StatusOfRental), queryRentalStatus)).ToList();
            }
            return View("Index", filteredStatus);
        }
    }
}
