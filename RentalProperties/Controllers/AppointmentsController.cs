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
    public class AppointmentsController : Controller
    {
        private readonly RentalPropertiesDBContext _context;

        public AppointmentsController(RentalPropertiesDBContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var rentalPropertiesDBContext = _context.Appointments.Include(a => a.Apartment).ThenInclude(a=>a.Property).Include(a => a.Tenant).OrderBy(a=>a.VisitDate).ToList();

            var currentUser = HttpContext.User;
            int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (await RentalWebsite.UserHasPolicy(HttpContext,"MustBeTenant"))
            {
                rentalPropertiesDBContext = rentalPropertiesDBContext.Where(a => a.TenantId == userId).ToList();
            } else if (await RentalWebsite.UserHasPolicy(HttpContext,"MustBeManager"))
            {
                rentalPropertiesDBContext = rentalPropertiesDBContext.Where(a => a.Apartment.Property.ManagerId == userId).ToList();
            }

            return View(rentalPropertiesDBContext);
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Apartment).ThenInclude(a=>a.Property)
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }
            if (!await AppointmentForTenantOrManager(appointment))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        [HttpGet("Appointments/Create")]
        public async Task<IActionResult> Create()
        {
            ViewData["TenantId"] = await CreateSelectListOfTenants();
            ViewData["ApartmentId"] = await CreateSelectListOfApartments();
            return View();
        }

        // GET: Appointments/Create/2
        [HttpGet("Appointments/Create/{apartmentId}")]
        public async Task<IActionResult> Create(int apartmentId)
        {
            ViewData["TenantId"] = await CreateSelectListOfTenants();
            ViewData["ApartmentId"] = await CreateSelectListOfApartments(apartmentId);
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,TenantId,ApartmentId,VisitDate")] Appointment appointment, bool confirmationStatus)
        {
            if (ModelState.IsValid)
            {
                bool needConfirmation = false;
                if (appointment.VisitDate<DateOnly.FromDateTime(DateTime.Now) && confirmationStatus == false)
                {
                    ViewData["ShowConfirmation"] = true;
                    ViewBag.ConfirmationMessage = "You are scheduling a visit for the past. Do you wish to continue?";
                    needConfirmation = true;
                }
                if (needConfirmation)
                {
                    ViewData["ApartmentId"] = await CreateSelectListOfApartments();
                    ViewData["TenantId"] = await CreateSelectListOfTenants();
                    return View(appointment);
                }
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApartmentId"] = await CreateSelectListOfApartments();
            ViewData["TenantId"] = await CreateSelectListOfTenants();
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Apartment).ThenInclude(a => a.Property)
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }
            if (!await AppointmentForTenantOrManager(appointment))
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            ViewData["ApartmentId"] = await CreateSelectListOfApartments();
            ViewData["TenantId"] = await CreateSelectListOfTenants();
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,TenantId,ApartmentId,VisitDate")] Appointment appointment, bool confirmationStatus)
        {
            if (id != appointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                bool needConfirmation = false;
                if (appointment.VisitDate < DateOnly.FromDateTime(DateTime.Now) && confirmationStatus == false)
                {
                    ViewData["ShowConfirmation"] = true;
                    ViewBag.ConfirmationMessage = "You are scheduling a visit for the past. Do you wish to continue?";
                    needConfirmation = true;
                }
                if (needConfirmation)
                {
                    ViewData["ApartmentId"] = await CreateSelectListOfApartments();
                    ViewData["TenantId"] = await CreateSelectListOfTenants();
                    return View(appointment);
                }
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentId))
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
            ViewData["ApartmentId"] = new SelectList(_context.Apartments, "ApartmentId", "ApartmentId", appointment.ApartmentId);
            ViewData["TenantId"] = new SelectList(_context.UserAccounts, "UserId", "UserId", appointment.TenantId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Apartment).ThenInclude(a => a.Property)
                .Include(a => a.Tenant)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }
            if (!await AppointmentForTenantOrManager(appointment))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }

        private async Task<SelectList> CreateSelectListOfApartments()
        {
            var selectListApartments = _context.Apartments.Include(a => a.Property).ToList();
            if (await RentalWebsite.UserHasPolicy(HttpContext,"MustBeManager"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                selectListApartments = selectListApartments.Where(a => a.Property.ManagerId == userId).ToList();
            }

            //Creatting list of Apartments
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in selectListApartments)
            {
                string text = item.ApartmentId.ToString() + " - " + item.Property.PropertyName.ToString() + " - Apt " + item.ApartmentNumber.ToString();
                SelectListItem selectListItem = new SelectListItem(text, item.ApartmentId.ToString());
                list.Add(selectListItem);
            }
            SelectList listToReturn = new SelectList(list, "Value", "Text");

            return listToReturn;
        }

        private async Task<SelectList> CreateSelectListOfApartments(int apId)
        {
            var selectListApartments = _context.Apartments.Include(a => a.Property).Where(a=>a.ApartmentId==apId).ToList();
            if (await RentalWebsite.UserHasPolicy(HttpContext,"MustBeManager"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                selectListApartments = selectListApartments.Where(a => a.Property.ManagerId == userId).ToList();
            }

            //Creatting list of Apartments
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in selectListApartments)
            {
                string text = item.ApartmentId.ToString() + " - " + item.Property.PropertyName.ToString() + " - Apt " + item.ApartmentNumber.ToString();
                SelectListItem selectListItem = new SelectListItem(text, item.ApartmentId.ToString());
                list.Add(selectListItem);
            }
            SelectList listToReturn = new SelectList(list, "Value", "Text");

            return listToReturn;
        }

        private async Task<SelectList> CreateSelectListOfTenants()
        {           
            var selectListTenants = _context.UserAccounts.Where(u => u.UserType == UserType.Tenant).ToList();
            if (await RentalWebsite.UserHasPolicy(HttpContext,"MustBeTenant"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                selectListTenants = selectListTenants.Where(u => u.UserId == userId).ToList();
            }

            //Creatting list of Apartments
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in selectListTenants)
            {
                SelectListItem selectListItem = new SelectListItem(item.FullName, item.UserId.ToString());
                list.Add(selectListItem);
            }
            SelectList listToReturn = new SelectList(list, "Value", "Text");

            return listToReturn;
        }

        private async Task<bool> AppointmentForTenantOrManager(Appointment appointment)
        {
            var currentUser = HttpContext.User;
            int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (appointment.TenantId == userId || 
                appointment.Apartment.Property.ManagerId == userId ||
                await RentalWebsite.UserHasPolicy(HttpContext,"MustBeOwnerOrAdministrator"))
            {
                return true;
            }
            return false;
        }
    }
}
