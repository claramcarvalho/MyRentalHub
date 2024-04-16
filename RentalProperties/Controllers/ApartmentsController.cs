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
using Microsoft.IdentityModel.Tokens;
using RentalProperties.DATA;
using RentalProperties.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static NuGet.Client.ManagedCodeConventions;

namespace RentalProperties.Controllers
{
    [Authorize]
    public class ApartmentsController : Controller
    {
        private readonly RentalPropertiesDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public ApartmentsController(RentalPropertiesDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Apartments
        public async Task<IActionResult> Index()
        {
            var currentUser = HttpContext.User;

            if (await RentalWebsite.UserHasPolicy(HttpContext,"MustBeOwnerOrAdministrator"))
            {
                var listOfApartments = _context.Apartments.Include(a => a.Property);
                return View(await listOfApartments.ToListAsync());
            }
            else if (await RentalWebsite.UserHasPolicy(HttpContext,"MustBeManager"))
            {
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                var listOfApartments = _context.Apartments.Include(a => a.Property).Where(m => m.Property.ManagerId == userId);
                return View(await listOfApartments.ToListAsync());
            }
            else
            {
                var listProperties = _context.Properties;
                ViewData["Properties"] = new SelectList(listProperties, "PropertyId", "PropertyName");
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
        public async Task<IActionResult> Create([Bind("ApartmentId,PropertyId,ApartmentNumber,NbOfBeds,NbOfBaths,NbOfParkingSpots,PriceAnnounced,AnimalsAccepted")] Apartment apartment, bool confirmationStatus, IFormFile fileInput)
        {
            ModelState.Remove("fileInput");
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

                if (fileInput!=null)
                    await SavePhotoApartmentInWWWRoot(apartment, fileInput);

                if (EndsWithANumber())
                {
                    return RedirectToAction("Index", "Properties");
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PropertyId"] = await GetListOfProperties(apartment.PropertyId);
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
        public async Task<IActionResult> Edit(int id, [Bind("ApartmentId,PropertyId,ApartmentNumber,NbOfBeds,NbOfBaths,NbOfParkingSpots,PriceAnnounced,AnimalsAccepted")] Apartment apartment, bool confirmationStatus, IFormFile fileInput)
        {
            if (id != apartment.ApartmentId)
            {
                return NotFound();
            }

            ModelState.Remove("fileInput");
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

                    if (fileInput != null)
                        await SavePhotoApartmentInWWWRoot(updateThisApartment, fileInput);
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

        // GET: Apartments/Timeline/5
        [Authorize(Policy = "CantBeTenant")]
        public IActionResult Timeline(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = _context.Apartments.Include(a => a.Property).Include(a=> a.Rentals).ThenInclude(r=>r.Tenant).FirstOrDefault(m => m.ApartmentId == id);
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

        private async Task<SelectList> ListOfPropertiesDependingOnUser()
        {
            var propertiesFromDatabase = _context.Properties.AsQueryable();

            if (!await RentalWebsite.UserHasPolicy(HttpContext,"MustBeOwnerOrAdministrator"))
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

            if (!await RentalWebsite.UserHasPolicy(HttpContext,"MustBeOwnerOrAdministrator"))
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

        public async Task<IActionResult> Search(
                string queryMDate,
                string queryProperty,
                string queryNbBed,
                string queryNbBath,
                string queryNbParking,
                string queryMinPrice,
                string queryMaxPrice,
                string queryAnimals
                )
        {
            string filtersApplied = "Filters Applied:";
            var apartmentsToFilter = _context.Apartments
                .Include(a=>a.Property)
                .Include(a=>a.Rentals)
                .ToList();
            if (await RentalWebsite.UserHasPolicy(HttpContext,"MustBeManager"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                apartmentsToFilter = apartmentsToFilter.Where(a => a.Property.ManagerId == userId).ToList();
            }
            //filtering moving date
            if (!queryMDate.IsNullOrEmpty())
            {
                apartmentsToFilter = FilterAvailableApartmentsInMovingDate(queryMDate, apartmentsToFilter);
                filtersApplied = filtersApplied + " (Moving Date: " + DateOnly.FromDateTime(DateTime.Parse(queryMDate)) + ")";
            }
            if (queryProperty.ToString() != "All")
            {
                apartmentsToFilter = apartmentsToFilter.Where(u => u.Property.PropertyId == int.Parse(queryProperty)).ToList();
                string propertyName = _context.Properties.FirstOrDefault(p => p.PropertyId == int.Parse(queryProperty)).PropertyName;
                filtersApplied = filtersApplied + " (Property: " + propertyName +")";
            }

            if (queryNbBed.ToString() != "All")
            {
                apartmentsToFilter = FilterNbOfBeds(queryNbBed,apartmentsToFilter);
                filtersApplied = filtersApplied + " (Number of Bedrooms: "+ queryNbBed + ")";
            }
            if (queryNbBath.ToString() != "All")
            {
                apartmentsToFilter = FilterNbOfBaths(queryNbBath, apartmentsToFilter);
                filtersApplied = filtersApplied + " (Number of Bathrooms: " + queryNbBath + ")";
            }
            if (queryNbParking.ToString() != "All")
            {
                apartmentsToFilter = FilterNbOfParking(queryNbParking, apartmentsToFilter);
                filtersApplied = filtersApplied + " (Number of Parking Spots: " + queryNbParking + ")";
            }
            if (!queryMinPrice.IsNullOrEmpty())
            {
                apartmentsToFilter = apartmentsToFilter.Where(u => u.PriceAnnounced >= decimal.Parse(queryMinPrice)).ToList();
                filtersApplied = filtersApplied + " (Min Price: " + decimal.Parse(queryMinPrice) + ")";
            }
            if (!queryMaxPrice.IsNullOrEmpty())
            {
                apartmentsToFilter = apartmentsToFilter.Where(u => u.PriceAnnounced <= decimal.Parse(queryMaxPrice)).ToList();
                filtersApplied = filtersApplied + " (Max Price: " + decimal.Parse(queryMaxPrice) + ")";
            }
            if (queryAnimals == "true")
            {
                apartmentsToFilter = apartmentsToFilter.Where(u => u.AnimalsAccepted).ToList();
                filtersApplied = filtersApplied + " (Animals Accepted: " + queryAnimals + ")";
            }
            else if (queryAnimals == "false")
            {
                apartmentsToFilter = apartmentsToFilter.Where(u => !u.AnimalsAccepted).ToList();
                filtersApplied = filtersApplied + " (Animals Accepted: " + queryAnimals + ")";
            }

            var listProperties = _context.Properties;
            ViewData["Filters"] = filtersApplied.ToString();
            ViewData["Properties"] = new SelectList(listProperties, "PropertyId", "PropertyName");
            return View("Index", apartmentsToFilter);
        }

        public List<Apartment> FilterNbOfBeds(string query, List<Apartment> apartments)
        {
            switch(query)
            {
                case "0":
                    apartments = apartments.Where(a => a.NbOfBeds == 0).ToList();
                    break;
                case "0+":
                    apartments = apartments.Where(a => a.NbOfBeds >= 0).ToList();
                    break;
                case "1":
                    apartments = apartments.Where(a => a.NbOfBeds == 1).ToList();
                    break;
                case "1+":
                    apartments = apartments.Where(a => a.NbOfBeds >= 1).ToList();
                    break;
                case "2":
                    apartments = apartments.Where(a => a.NbOfBeds == 2).ToList();
                    break;
                case "2+":
                    apartments = apartments.Where(a => a.NbOfBeds >= 2).ToList();
                    break;
                case "3":
                    apartments = apartments.Where(a => a.NbOfBeds == 3).ToList();
                    break;
                case "3+":
                    apartments = apartments.Where(a => a.NbOfBeds >= 3).ToList();
                    break;

            }

            return apartments;
        }

        public List<Apartment> FilterNbOfBaths(string query, List<Apartment> apartments)
        {
            switch (query)
            {
                case "1":
                    apartments = apartments.Where(a => a.NbOfBaths == 1).ToList();
                    break;
                case "1+":
                    apartments = apartments.Where(a => a.NbOfBaths >= 1).ToList();
                    break;
                case "2":
                    apartments = apartments.Where(a => a.NbOfBaths == 2).ToList();
                    break;
                case "2+":
                    apartments = apartments.Where(a => a.NbOfBaths >= 2).ToList();
                    break;
            }

            return apartments;
        }

        public List<Apartment> FilterNbOfParking(string query, List<Apartment> apartments)
        {
            switch (query)
            {
                case "1":
                    apartments = apartments.Where(a => a.NbOfParkingSpots == 1).ToList();
                    break;
                case "1+":
                    apartments = apartments.Where(a => a.NbOfParkingSpots >= 1).ToList();
                    break;
                case "2":
                    apartments = apartments.Where(a => a.NbOfParkingSpots == 2).ToList();
                    break;
                case "2+":
                    apartments = apartments.Where(a => a.NbOfParkingSpots >= 2).ToList();
                    break;
            }

            return apartments;
        }

        public List<Apartment> FilterAvailableApartmentsInMovingDate(string movingD, List<Apartment> apartments)
        {
            List<Apartment> newListOfApartments = new List<Apartment>();
            DateOnly movingDate = DateOnly.FromDateTime(DateTime.Parse(movingD));
            foreach (var apartment in apartments)
            {
                var rentalsInApt = _context.Rentals.Where(r => r.ApartmentId == apartment.ApartmentId);
                bool rentalAvailable = true;
                foreach (var rental in rentalsInApt)
                {
                    if (movingDate > rental.FirstDayRental && movingDate < rental.LastDayRental && rental.RentalStatus != StatusOfRental.Terminated)
                    {
                        rentalAvailable = false; 
                        break;
                    }
                }
                if (rentalAvailable)
                {
                    newListOfApartments.Add(apartment);
                }        
            }

            return newListOfApartments;
        }

        private async Task SavePhotoApartmentInWWWRoot(Apartment apartment, IFormFile photo)
        {
            //Creating new folder
            string newDirectory = Path.Combine(
                _environment.WebRootPath,
                "images",
                apartment.PropertyId.ToString(),
                apartment.ApartmentId.ToString());

            if (!Directory.Exists(newDirectory))
            {
                Directory.CreateDirectory(newDirectory);
            }
            string fileName = $"1{Path.GetExtension(photo.FileName)}";
            string filePath = Path.Combine(newDirectory, fileName);

            using (var stream = new FileStream(filePath,FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }
        }
    }
}
