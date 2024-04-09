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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RentalProperties.Controllers
{
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
            var currentUser = HttpContext.User;

            if (await UserHasPolicy("MustBeOwnerOrAdministrator"))
            {
                var listOfApartments = _context.Apartments.Include(a => a.Property);
                return View(await listOfApartments.ToListAsync());
            }
            else if (await UserHasPolicy("MustBeManager"))
            {
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                var listOfApartments = _context.Apartments.Include(a => a.Property).Where(m => m.Property.ManagerId == userId);
                return View(await listOfApartments.ToListAsync());
            }
            else
            {
                /////////////////////////PAREI AQUI --> SEARCHING APARTMENTS
                var listOfApartments = _context.Apartments.Include(a => a.Property);
                return View(await listOfApartments.ToListAsync());
            }
        }

        // GET: Apartments/Details/5
        [Authorize(Policy = "CantBeTenant")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartments
                .Include(a => a.Property).Include(a => a.Rentals).ThenInclude(r => r.Tenant)
                .FirstOrDefaultAsync(m => m.ApartmentId == id);
            if (apartment == null)
            {
                return NotFound();
            }

            if (!CurrentUserIsAllowedToManageProperty(apartment))
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            return View(apartment);
        }

        // GET: Apartments/Create
        [Authorize(Policy = "CantBeTenant")]
        [HttpGet("Apartments/Create")]
        public async Task<IActionResult> Create()
        {
            var propertiesFromDatabase = _context.Properties;

            ViewData["PropertyId"] = await ListOfPropertiesDependingOnUser();
            return View();
        }

        // GET: Apartments/Create/4
        [Authorize(Policy = "CantBeTenant")]
        [HttpGet("Apartments/Create/{propertyId}")]
        public async Task<IActionResult> Create(int propertyId)
        {
            ViewData["PropertyId"] = await GetListOfProperties(propertyId);
            return View();
        }

        // POST: Apartments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "CantBeTenant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApartmentId,PropertyId,ApartmentNumber,NbOfBeds,NbOfBaths,NbOfParkingSpots,PriceAnnounced,AnimalsAccepted")] Apartment apartment, bool confirmationStatus)
        {
            if (ModelState.IsValid)
            {
                bool dataOk = true;
                List<string> errors = new List<string>();
                if (ApartmentNumberExistsInProperty(apartment,false))
                {
                    errors.Add("This Property already has an apartment with that number. Please use an unique apartment number.");
                    dataOk = false;
                }
                if (apartment.PriceAnnounced == 0 && confirmationStatus == false)
                {
                    ViewData["ShowConfirmation"] = true;
                    ViewBag.ConfirmationMessage = "You are defining the price as ZERO. Do you wish to continue?";
                    ViewData["PropertyId"] = await GetListOfProperties(apartment.PropertyId);
                    return View(apartment);
                }
                if (!dataOk)
                {
                    ViewData["PropertyId"] = await GetListOfProperties(apartment.PropertyId);
                    ViewData["ErrorMessage"] = errors;
                    return View(apartment);
                }

                _context.Add(apartment);
                await _context.SaveChangesAsync();
                if (EndsWithANumber())
                {
                    return RedirectToAction("Index", "Properties");
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PropertyId"] = GetListOfProperties(apartment.PropertyId);
            return View(apartment);
        }

        // GET: Apartments/Edit/5
        [Authorize(Policy = "CantBeTenant")]
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

            if (!CurrentUserIsAllowedToManageProperty(apartment))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            ViewData["PropertyId"] = await GetListOfProperties(apartment.PropertyId);
            return View(apartment);
        }

        // POST: Apartments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "CantBeTenant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApartmentId,PropertyId,ApartmentNumber,NbOfBeds,NbOfBaths,NbOfParkingSpots,PriceAnnounced,AnimalsAccepted")] Apartment apartment, bool confirmationStatus)
        {
            if (id != apartment.ApartmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                bool dataOk = true;
                List<string> errors = new List<string>();
                if (ApartmentNumberExistsInProperty(apartment,true))
                {
                    errors.Add("This Property already has an apartment with that number. Please use an unique apartment number.");
                    dataOk = false;
                }
                if (!dataOk)
                {
                    ViewData["PropertyId"] = await GetListOfProperties(apartment.PropertyId);
                    ViewData["ErrorMessage"] = errors;
                    return View(apartment);
                }
                if (apartment.PriceAnnounced == 0 && confirmationStatus == false)
                {
                    ViewData["ShowConfirmation"] = true;
                    ViewBag.ConfirmationMessage = "You are defining the price as ZERO. Do you wish to continue?";
                    ViewData["PropertyId"] = await GetListOfProperties(apartment.PropertyId);
                    return View(apartment);
                }

                try
                {
                    Apartment updateThisApartment = _context.Apartments.FirstOrDefault(a=>
                        a.ApartmentId == id);

                    if (updateThisApartment != null)
                    {
                        updateThisApartment.PropertyId = apartment.PropertyId;
                        updateThisApartment.ApartmentNumber = apartment.ApartmentNumber;
                        updateThisApartment.NbOfBeds = apartment.NbOfBeds;
                        updateThisApartment.NbOfBaths = apartment.NbOfBaths;
                        updateThisApartment.NbOfParkingSpots = apartment.NbOfParkingSpots;
                        updateThisApartment.PriceAnnounced = apartment.PriceAnnounced;
                        updateThisApartment.AnimalsAccepted = apartment.AnimalsAccepted;
                    }

                    _context.Update(updateThisApartment);
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
        [Authorize(Policy = "CantBeTenant")]
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
            if (!CurrentUserIsAllowedToManageProperty(apartment))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View(apartment);
        }

        // POST: Apartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CantBeTenant")]
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

        private bool ApartmentNumberExistsInProperty(Apartment apartment, bool isEdition)
        {
            if (isEdition)
            {
                return false;
            }
            return _context.Apartments.Where(a =>
                    a.ApartmentNumber == apartment.ApartmentNumber &&
                    a.PropertyId == apartment.PropertyId).Any();
        }

        private bool EndsWithANumber()
        {
            var referer = Request.Headers["Referer"].ToString();
            bool endsWithNumber = char.IsDigit(referer[referer.Length - 1]);
            return endsWithNumber;
        }

        private async Task<SelectList> GetListOfProperties(int propertyId)
        {
            if (EndsWithANumber())
                return await ListOfPropertiesDependingOnUserAndPropertyId(propertyId);
            else return await ListOfPropertiesDependingOnUser();
        }

        private bool CurrentUserIsAllowedToManageProperty(Apartment apartment)
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c =>
                c.Type == "Type" &&
                (
                    c.Value == "Manager"
                )))
            {
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);

                Apartment apartmentFound = _context.Apartments
                    .Where(a => a.ApartmentId == apartment.ApartmentId).Include(a => a.Property).First();

                if (apartment.Property.ManagerId != userId)
                {
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> UserHasPolicy(string policyName)
        {
            var currentUser = HttpContext.User;
            var authorizationService = HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorizationResult = await authorizationService.AuthorizeAsync(currentUser, null, policyName);
            return authorizationResult.Succeeded;
        }

        private async Task<SelectList> ListOfPropertiesDependingOnUser()
        {
            var propertiesFromDatabase = _context.Properties.AsQueryable();

            if (!await UserHasPolicy("MustBeOwnerOrAdministrator"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                propertiesFromDatabase = propertiesFromDatabase.Where(u => u.ManagerId == userId);
            }

            SelectList listOfProperties = new SelectList(
                propertiesFromDatabase,
                "PropertyId",
                "PropertyName");

            return listOfProperties;
        }

        private async Task<SelectList> ListOfPropertiesDependingOnUserAndPropertyId(int propertyId)
        {
            var propertiesFromDatabase = _context.Properties.Where(p=> p.PropertyId == propertyId);

            if (!await UserHasPolicy("MustBeOwnerOrAdministrator"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                propertiesFromDatabase = propertiesFromDatabase.Where(u => u.ManagerId == userId);
            }

            SelectList listOfProperties = new SelectList(
                propertiesFromDatabase,
                "PropertyId",
                "PropertyName");

            return listOfProperties;
        }
    }
}
