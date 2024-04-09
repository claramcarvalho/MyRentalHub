using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalProperties;
using RentalProperties.DATA;
using RentalProperties.Models;

namespace RentalProperties.Controllers
{
    [Authorize(Policy = "CantBeTenant")]
    public class PropertiesController : Controller
    {
        private readonly RentalPropertiesDBContext _context;

        public PropertiesController(RentalPropertiesDBContext context)
        {
            _context = context;
        }

        // GET: Properties
        public async Task<IActionResult> Index()
        {
            var currentUser = HttpContext.User;

            if (await UserHasPolicy("MustBeOwnerOrAdministrator")) 
            {
                var rentalPropertiesDBContext = _context.Properties.Include(m => m.Manager).Include(a => a.Apartments).ThenInclude(r => r.Rentals);
                return View(await rentalPropertiesDBContext.ToListAsync());
            }
            else
            {
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                var rentalPropertiesDBContext = _context.Properties.Include(m => m.Manager).Include(a => a.Apartments).ThenInclude(r => r.Rentals).Where(m=> m.ManagerId == userId);
                return View(await rentalPropertiesDBContext.ToListAsync());
            }

        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Properties
                .Include(m => m.Manager).Include(m => m.Apartments)
                .FirstOrDefaultAsync(m => m.PropertyId == id);
            if (property == null)
            {
                return NotFound();
            }


            if (!CurrentUserIsAllowedToManageProperty(property))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View(property);
        }

        // GET: Properties/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ManagerId"] = await ListOfManagersDependingOnPolicy();
            return View();
        }

        // POST: Properties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyId,PropertyName,AddressNumber,AddressStreet,PostalCode,City,Neighbourhood,ManagerId")] Property property)
        {
            if (ModelState.IsValid)
            {
                if(!PropertyNameAlreadyExists(property))
                {
                    property.PostalCode = property.PostalCode.ToUpper();
                    _context.Add(property);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                ViewData["ErrorMessage"] = "There is already a Property with that same name! Please choose a different name.";
                ViewData["ManagerId"] = await ListOfManagersDependingOnPolicy();
                return View(property);

            }
            ViewData["ManagerId"] = await ListOfManagersDependingOnPolicy();
            return View(property);
        }

        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            if (!CurrentUserIsAllowedToManageProperty(property))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            ViewData["ManagerId"] = await ListOfManagersDependingOnPolicy();

            return View(property);
        }

        // POST: Properties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PropertyId,PropertyName,AddressNumber,AddressStreet,PostalCode,City,Neighbourhood,ManagerId")] Property property)
        {
            if (id != property.PropertyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!PropertyNameAlreadyExists(property))
                {
                    try
                    {
                        _context.Update(property);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PropertyExists(property.PropertyId))
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
            }
            ViewData["ManagerId"] = ListOfManagersDependingOnPolicy();
            return View(property);
        }

        // GET: Properties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Properties
                .Include(m => m.Manager)
                .FirstOrDefaultAsync(m => m.PropertyId == id);
            if (property == null)
            {
                return NotFound();
            }
            if (!CurrentUserIsAllowedToManageProperty(property))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View(property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property != null)
            {
                _context.Properties.Remove(property);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.PropertyId == id);
        }

        private async Task<bool> UserHasPolicy(string policyName)
        {
            var currentUser = HttpContext.User;
            var authorizationService = HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            var authorizationResult = await authorizationService.AuthorizeAsync(currentUser, null, policyName);
            return authorizationResult.Succeeded;
        }

        public async Task<SelectList> ListOfManagersDependingOnPolicy()
        {
            var managersFromDatabase = _context.UserAccounts.Where(
                m => m.UserType == UserType.Manager
                );

            if (!await UserHasPolicy("MustBeOwnerOrAdministrator"))
            {
                var currentUser = HttpContext.User;
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                managersFromDatabase = managersFromDatabase.Where(u => u.UserId == userId);
            }

            SelectList listOfManagers = new SelectList(
                managersFromDatabase,
                "UserId",
                "FullName");

            return listOfManagers;
        }

        private bool PropertyNameAlreadyExists(Property property)
        {
            return _context.Properties.Where(p =>
                    p.PropertyName == property.PropertyName).Any();
        }

        private bool CurrentUserIsAllowedToManageProperty(Property property)
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c =>
                c.Type == "Type" &&
                (
                    c.Value == "Manager"
                )))
            {
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (property.ManagerId != userId)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
