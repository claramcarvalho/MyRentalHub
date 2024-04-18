using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RentalProperties.DATA;
using RentalProperties.Models;
using System.Security.Claims;

namespace RentalProperties.Controllers
{
    public class ManagerSlotsController : Controller
    {
        private readonly RentalPropertiesDBContext _context;

        public ManagerSlotsController(RentalPropertiesDBContext context)
        {
            _context = context;
        }

        // GET: ManagerAvailabilities/Insert
        [Authorize(Policy = "CantBeTenant")]
        public async Task<IActionResult> Index()
        {
            var currentUser = HttpContext.User;

            if (await RentalWebsite.UserHasPolicy(HttpContext, "MustBeOwnerOrAdministrator"))
            {
                var listOfSlots = _context.ManagerSlots.Include(s=>s.Manager).OrderBy(s=>s.AvailableSlot);
                return View(await listOfSlots.ToListAsync());
            }
            else if (await RentalWebsite.UserHasPolicy(HttpContext, "MustBeManager"))
            {
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
                var listOfSlots = _context.ManagerSlots.Where(s=> s.ManagerId == userId).Include(s => s.Manager).OrderBy(s => s.AvailableSlot);
                return View(await listOfSlots.ToListAsync());
            }
            return View();
        }

        // GET: ManagerAvailabilities/Insert
        [Authorize(Policy = "CantBeTenant")]
        public async Task<IActionResult> Insert()
        {
            ViewData["ManagerId"] = await GetListOfManagers();
            return View();
        }

        // GET: ManagerAvailabilities/Insert
        [Authorize(Policy = "CantBeTenant")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Insert(DateOnly firstDate, DateOnly lastDate, TimeOnly begin, TimeOnly end, [Bind("ManagerId")] ManagerSlot slot)
        {
            bool dataOk = true;
            List<string> errors = new List<string>();
            if (lastDate<firstDate)
            {
                errors.Add("Last Date cannot be before First Date");
                dataOk = false;
            }
            if (end < begin)
            {
                errors.Add("Final Time cannot be before Initial Time");
                dataOk = false;
            }
            if (!dataOk)
            {
                ViewData["ErrorMessage"] = errors;
                return View();
            }

            //calculating and inserting availabilities
            for (DateOnly day = firstDate; day <= lastDate; day = day.AddDays(1))
            {
                for (TimeOnly hour = begin; hour < end; hour = hour.AddHours(1))
                {
                    DateTime newSlotDate = new DateTime(day.Year, day.Month, day.Day, hour.Hour, hour.Minute, hour.Second);
                    DateTime newSlotDateEnd = newSlotDate.AddHours(1);
                    slot.SlotId = 0;
                    slot.AvailableSlot = newSlotDate;
                    slot.IsAlreadyScheduled = false;

                    if (_context.ManagerSlots.Any(s=>
                            (
                                (newSlotDate>=s.AvailableSlot && newSlotDate<s.AvailableSlot.AddHours(1)) ||
                                (newSlotDateEnd>s.AvailableSlot && newSlotDateEnd <s.AvailableSlot.AddHours(1))
                            ) 
                            && s.ManagerId == slot.ManagerId))
                    {
                        errors.Add("Slot on " + newSlotDate.ToString("dd-MMM-yyyy HH:mm") + " overlaps an existing slot. It was not created.");
                        dataOk = false;
                    } else
                    {
                        _context.Add(slot);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            if (!dataOk)
            {
                errors.Add("Some of the slots might have been created. Check the list of posted slots.");
                ViewData["ErrorMessage"] = errors;
                ViewData["ManagerId"] = await GetListOfManagers();
                return View();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ManagerAvailabilities/Delete/5
        [Authorize(Policy = "CantBeTenant")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var slot = await _context.ManagerSlots
                .Include(a => a.Manager)
                .FirstOrDefaultAsync(m => m.SlotId == id);
            if (slot == null)
            {
                return NotFound();
            }
            if (!CurrentUserIsManagerForSlot(slot))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            return View(slot);
        }

        // POST: ManagerAvailabilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CantBeTenant")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var slot = await _context.ManagerSlots.FindAsync(id);
            if (slot != null)
            {
                _context.ManagerSlots.Remove(slot);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CurrentUserIsManagerForSlot(ManagerSlot slot)
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c =>
                c.Type == "Type" &&
                (
                    c.Value == "Manager"
                )))
            {
                int userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (slot.ManagerId != userId)
                {
                    return false;
                }
            }
            return true;
        }
        private async Task<SelectList> GetListOfManagers()
        {
            var managersFromDatabase = _context.UserAccounts.Where(u=>u.UserType == UserType.Manager).AsQueryable();

            if (!await RentalWebsite.UserHasPolicy(HttpContext, "MustBeOwnerOrAdministrator"))
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
    }
}
