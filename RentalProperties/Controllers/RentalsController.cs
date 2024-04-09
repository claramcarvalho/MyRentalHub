using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
            ViewData["Properties"] = await GetListOfPropertiesDependingOnPolicy();
            var listOfRentals = await GetRentalsDependingOnPolicy();

            return View(listOfRentals);
        }

        // GET: Rentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = (Rental)await GetRentalDependingOnPolicy((int)id);
            if (rental == null)
            {
                return NotFound();
            }
            if (! await CurrentUserIsAllowedToManageProperty(rental))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View(rental);
        }

        // GET: Rentals/Create
        [HttpGet("Rentals/Create")]
        public async Task<IActionResult> Create()
        {
            //Creatting list of Apartments
            ViewData["ApartmentsId"] = await GetApartments();

            //Creating list of Tenants
            ViewData["TenantId"] = GetTenants();

            ViewData["ShowConfirmation"] = false;

            return View();
        }

        // GET: Rentals/Create/1
        [HttpGet("Rentals/Create/{apartmentId}")]
        public async Task<IActionResult> Create(int apartmentId)
        {
            //Creatting list of Apartments
            ViewData["ApartmentsId"] = await GetApartments(apartmentId);

            //Creating list of Tenants
            ViewData["TenantId"] = GetTenants();

            ViewData["ShowConfirmation"] = false;

            return View();
        }

        // POST: Rentals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentalId,TenantId,ApartmentId,FirstDayRental,LastDayRental,PriceRent,RentalStatus")] Rental rental, bool confirmationStatus)
        {
            if (ModelState.IsValid)
            {
                bool dataOk = true;
                List<string> errors = new List<string>();
                if (rental.ApartmentId == -1)
                {
                    errors.Add("Please select an apartment!");
                    dataOk = false;
                }
                if (rental.LastDayRental<rental.FirstDayRental)
                {
                    errors.Add("Last Day of Rental can't be before First Day of Rental.");
                    dataOk = false;
                }
                if (RentalInApartmentWithSameFirstDay(rental,false))
                {
                    errors.Add("This apartment has already a rental that begins in the same day! Choose another day!");
                    dataOk = false;
                }
                if (RentalInApartmentWithOverlappingDates(rental,false))
                {
                    errors.Add("This apartment has already a rental which dates overlap the chosen dates! Please verify before adding.");
                    dataOk = false;
                }
                if (rental.PriceRent == 0 && confirmationStatus == false)
                {
                    ViewData["ShowConfirmation"] = true;
                    ViewBag.ConfirmationMessage = "You are defining the price as ZERO. Do you wish to continue?";
                    ViewData["ApartmentsId"] = await GetListOfApartments(rental);
                    ViewData["TenantId"] = GetTenants();
                    return View(rental);
                }
                if (TenantWithSameFirstDayOfRental(rental) && confirmationStatus==false)
                {
                    ViewData["ShowConfirmation"] = true;
                    ViewBag.ConfirmationMessage = "This tenant has already a rental that starts in the same day. Do you wish to continue?";
                    ViewData["ApartmentsId"] = await GetListOfApartments(rental);
                    ViewData["TenantId"] = GetTenants();
                    return View(rental);
                }
                if (!dataOk)
                {
                    ViewData["ErrorMessage"] = errors;
                    ViewData["ApartmentsId"] = await GetListOfApartments(rental);
                    ViewData["TenantId"] = GetTenants();
                    return View(rental);
                }
                _context.Add(rental);
                await _context.SaveChangesAsync();
                if (EndsWithANumber())
                {
                    return RedirectToAction("Index", "Apartments");
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApartmentsId"] = GetListOfApartments(rental);
            ViewData["TenantId"] = GetTenants();
            return View(rental);
        }

        // GET: Rentals/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await GetRental((int)id);
            if (rental == null)
            {
                return NotFound();
            }
            if (!await CurrentUserIsAllowedToManageProperty(rental))
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            ViewData["ApartmentsId"] = await GetApartments(rental.ApartmentId);
            ViewData["TenantId"] = GetTenants();
            return View(rental);
        }

        // POST: Rentals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RentalId,TenantId,ApartmentId,FirstDayRental,LastDayRental,PriceRent,RentalStatus")] Rental rental, bool confirmationStatus)
        {
            if (id != rental.RentalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                bool dataOk = true;
                List<string> errors = new List<string>();
                if (rental.LastDayRental < rental.FirstDayRental)
                {
                    errors.Add("Last Day of Rental can't be before First Day of Rental.");
                    dataOk = false;
                }
                if (RentalInApartmentWithSameFirstDay(rental,true))
                {
                    errors.Add("This apartment has already a rental that begins in the same day! Choose another day!");
                    dataOk = false;
                }
                if (RentalInApartmentWithOverlappingDates(rental,true))
                {
                    errors.Add("This apartment has already a rental which dates overlap the chosen dates! Please verify before adding.");
                    dataOk = false;
                }
                if (!dataOk)
                {
                    ViewData["ErrorMessage"] = errors;
                    ViewData["ApartmentsId"] = GetListOfApartments(rental);
                    ViewData["TenantId"] = GetTenants();
                    return View(rental);
                }
                bool needConfirmation = false;
                string message = "";
                if (TenantWithSameFirstDayOfRental(rental) && confirmationStatus == false)
                {
                    ViewData["ShowConfirmationDates"] = true;
                    message = "This tenant has already a rental that starts in the same day.";
                    needConfirmation = true;
                }
                if (rental.PriceRent == 0 && confirmationStatus == false)
                {
                    ViewData["ShowConfirmationPrice"] = true;
                    if (message != "")
                    {
                        message = message + "<br><br>You are defining the price as ZERO.";
                    }
                    else
                    {
                        message = "You are defining the price as ZERO.";
                    }
                    needConfirmation = true;
                }
                string messageFinal = message;
                messageFinal = messageFinal + "<br><br>Do you wish to continue?";
                ViewBag.ConfirmationMessage = messageFinal;
                if (needConfirmation)
                {
                    ViewData["ApartmentsId"] = await GetListOfApartments(rental);
                    ViewData["TenantId"] = GetTenants();
                    return View(rental);
                }
                try
                {
                    Rental updateThisRental = _context.Rentals.FirstOrDefault(r =>
                    r.RentalId == id);

                    if (updateThisRental != null)
                    {
                        updateThisRental.TenantId = rental.TenantId;
                        updateThisRental.ApartmentId = rental.ApartmentId;
                        updateThisRental.FirstDayRental = rental.FirstDayRental;
                        updateThisRental.LastDayRental = rental.LastDayRental;
                        updateThisRental.PriceRent = rental.PriceRent;
                    }      
                    _context.Update(updateThisRental);
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
            ViewData["ApartmentId"] = await GetApartments(id);
            ViewData["TenantId"] = GetTenants();
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
                .Include(r => r.Apartment).ThenInclude(a=> a.Property)
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

            var listProperties = _context.Properties;
            ViewData["Properties"] = new SelectList(listProperties, "PropertyId", "PropertyName");
            return View("Index", filteredStatus);
        }

        private async Task<SelectList> GetApartments()
        {
            //Creatting list of Apartments
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in _context.Apartments
                .Include(a => a.Property))
            {
                if (await UserHasPolicy("MustBeOwnerOrAdministrator") || CurrentUserIsAllowedToManageProperty(item))
                {
                    string text = item.ApartmentId.ToString() + " - " + item.Property.PropertyName.ToString() + " - Apt " + item.ApartmentNumber.ToString();
                    SelectListItem selectListItem = new SelectListItem(text, item.ApartmentId.ToString());
                    list.Add(selectListItem);
                }
                
            }
            SelectList listOfApartments = new SelectList(list, "Value", "Text");

            return listOfApartments;
        }

        private async Task<SelectList> GetApartments(int apartmentId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in _context.Apartments
                .Include(a => a.Property)
                .Where(a => a.ApartmentId == apartmentId))
            {
                if (await UserHasPolicy("MustBeOwnerOrAdministrator") || CurrentUserIsAllowedToManageProperty(item))
                {
                    string text = item.ApartmentId.ToString() + " - " + item.Property.PropertyName.ToString() + " - Apt " + item.ApartmentNumber.ToString();
                    SelectListItem selectListItem = new SelectListItem(text, item.ApartmentId.ToString());
                    list.Add(selectListItem);
                }
            }
            SelectList listOfApartments = new SelectList(list, "Value", "Text");

            return listOfApartments;
        }

        private async Task<SelectList> GetListOfApartments(Rental rental)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            SelectList listOfApartments = new SelectList(list, "Value", "Text");
            if (EndsWithANumber())
                listOfApartments = await GetApartments(rental.ApartmentId);
            else listOfApartments = await GetApartments();

            return listOfApartments;
        }

        private SelectList GetTenants()
        {
            return new SelectList(_context.UserAccounts.Where(u => u.UserType == UserType.Tenant), "UserId", "FullName");
        }

        private bool EndsWithANumber()
        {
            var referer = Request.Headers["Referer"].ToString();
            bool endsWithNumber = char.IsDigit(referer[referer.Length - 1]);
            return endsWithNumber;
        }

        private bool RentalInApartmentWithSameFirstDay(Rental newRental, bool isEdition)
        {
            if (isEdition)
            {
                return false;
            }
            if (_context.Rentals.Where(r => 
                r.ApartmentId == newRental.ApartmentId && 
                r.FirstDayRental == newRental.FirstDayRental)
                .Any())
            {
                return true;
            }
            return false;
        }

        private bool RentalInApartmentWithOverlappingDates(Rental newRental, bool isEdition)
        {
            if (isEdition)
            {
                var oldRental = _context.Rentals.Where(r => r.RentalId == newRental.RentalId).First();
                if (newRental.FirstDayRental >= oldRental.FirstDayRental &&
                    newRental.LastDayRental <= oldRental.LastDayRental)
                {
                    return false;
                } 
            }
            var overlaps = _context.Rentals
            .FirstOrDefault(r =>
                ((newRental.FirstDayRental <= r.LastDayRental && newRental.FirstDayRental >= r.FirstDayRental) ||
                (newRental.LastDayRental >= r.FirstDayRental && newRental.LastDayRental <= r.LastDayRental)) &&
                newRental.ApartmentId == r.ApartmentId);
            
            if (overlaps!=null)
            {
                return true;
            }
            return false;
        }

        private bool TenantWithSameFirstDayOfRental(Rental newRental)
        {
            if (_context.Rentals
                .Any(r => 
                    r.TenantId == newRental.TenantId &&
                    r.FirstDayRental == newRental.FirstDayRental))
            {
                return true;
            }
            return false;
        }

        private async Task<SelectList> GetListOfPropertiesDependingOnPolicy()
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

        private async Task<bool> UserHasPolicy(string policyName)
        {
            var currentUser = HttpContext.User;
            var authorizationService = HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorizationResult = await authorizationService.AuthorizeAsync(currentUser, null, policyName);
            return authorizationResult.Succeeded;
        }

        private async Task<IQueryable<Rental>> GetRentalsDependingOnPolicy()
        {
            var rentalsFromDatabase = _context.Rentals
                .Include(r => r.Apartment).ThenInclude(a => a.Property)
                .Include(r => r.Tenant)
                .Where(t => t.Tenant.UserType == UserType.Tenant)
                .OrderBy(r => r.FirstDayRental).AsQueryable().AsNoTracking();

            if (!await UserHasPolicy("MustBeOwnerOrAdministrator"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                rentalsFromDatabase = rentalsFromDatabase.Where(u => u.Apartment.Property.ManagerId == userId);
            }

            return rentalsFromDatabase;
        }

        private async Task<Rental> GetRentalDependingOnPolicy(int id)
        {
            var rentalFromDatabase = _context.Rentals
                .Include(r => r.Apartment).ThenInclude(a => a.Property)
                .Include(r => r.Tenant)
                .Where(r => r.RentalId == id).First();
            if (!await UserHasPolicy("MustBeOwnerOrAdministrator"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (rentalFromDatabase.Apartment.Property.ManagerId != userId)
                {
                    rentalFromDatabase = null;
                }               
            }

            return rentalFromDatabase;
        }

        private async Task<Rental> GetRental(int id)
        {
            var rentalFromDatabase = _context.Rentals
                .Include(r => r.Apartment).ThenInclude(a => a.Property)
                .Include(r => r.Tenant)
                .Where(r => r.RentalId == id).First();
            
            return rentalFromDatabase;
        }

        private async Task<bool> CurrentUserIsAllowedToManageProperty(Rental rental)
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c =>
                c.Type == "Type" &&
                (
                    c.Value == "Manager"
                )))
            {
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (rental.Apartment.Property.ManagerId != userId)
                {
                    return false;
                }
            }
            return true;
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

                if (apartment.Property.ManagerId != userId)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
