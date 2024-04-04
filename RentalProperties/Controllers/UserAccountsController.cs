using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentalProperties;
using RentalProperties.DATA;

namespace RentalProperties.Controllers
{
    public class UserAccountsController : Controller
    {
        private readonly RentalPropertiesDBContext _context;

        public UserAccountsController(RentalPropertiesDBContext context)
        {
            _context = context;
        }

        // GET: UserAccounts
        [Authorize (Policy = "MustBeOwnerOrAdministrator")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserAccounts.ToListAsync());
        }

        [Authorize(Policy = "MustBeOwnerOrAdministrator")]
        // GET: UserAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccounts
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }

        [Authorize(Policy = "MustBeOwnerOrAdministrator")]
        // GET: UserAccounts/Create
        public IActionResult Create()
        {
            ViewBag.DateCreated = DateTime.Today;
            return View();
        }

        [Authorize(Policy = "MustBeOwnerOrAdministrator")]
        // POST: UserAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserType,UserName,UserPassword,DateCreated,FirstName,LastName,UserStatus")] UserAccount userAccount)
        {
            if (ModelState.IsValid)
            {
                if (userAccount.UserType == UserType.Tenant)
                {
                    ViewData["ErrorMessage"] = "You cannot create a Tenant Account!";
                    return View(userAccount);
                }
                if (UserNameExists(userAccount))
                {
                    ViewData["ErrorMessage"] = "The username you entered is already being used by another user! Please choose a unique username.";
                    return View(userAccount);
                }
                _context.Add(userAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userAccount);
        }

        [Authorize(Policy = "MustBeOwnerOrAdministrator")]
        // GET: UserAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccounts.FindAsync(id);
            if (userAccount == null)
            {
                return NotFound();
            }
            return View(userAccount);
        }

        [Authorize(Policy = "MustBeOwnerOrAdministrator")]
        // POST: UserAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserType,UserName,UserPassword,DateCreated,FirstName,LastName,UserStatus")] UserAccount userAccount)
        {
            if (id != userAccount.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (userAccount.UserType == UserType.Tenant)
                {
                    ViewData["ErrorMessage"] = "You cannot transform an user into a Tenant Account!";
                    return View(userAccount);
                }

                // Retrieve the old user account from the database
                UserAccount oldUserAccount = await _context.UserAccounts.FindAsync(id);
                // Detach the old user account from the context
                _context.Entry(oldUserAccount).State = EntityState.Detached;

                if (UserNameExists(userAccount) && oldUserAccount.UserName != userAccount.UserName)
                {
                    ViewData["ErrorMessage"] = "The username you entered is already being used by another user! Please choose a unique username.";
                    return View(userAccount);
                }

                try
                {
                    _context.Update(userAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAccountExists(userAccount.UserId))
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
            return View(userAccount);
        }

        [Authorize(Policy = "MustBeOwnerOrAdministrator")]
        // GET: UserAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAccount = await _context.UserAccounts
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userAccount == null)
            {
                return NotFound();
            }

            return View(userAccount);
        }

        [Authorize(Policy = "MustBeOwnerOrAdministrator")]
        // POST: UserAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userAccount = await _context.UserAccounts.FindAsync(id);
            
            if (userAccount != null)
            {
                var managerHasProperty = _context.Properties.Where(p => p.ManagerId == id).Any();
                if (managerHasProperty)
                {
                    ViewData["ErrorMessage"] = "This manager is responsible for one or more Properties. Please remove all of his managed properties before deleting the User Account.";
                    return View(userAccount);
                }
                _context.UserAccounts.Remove(userAccount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAccountExists(int id)
        {
            return _context.UserAccounts.Any(e => e.UserId == id);
        }

        public IActionResult Search(string queryType, string queryUsername, string queryDateCreation , string queryName, string queryLastName, string queryStatus)
        {
            var filteredTypes = _context.UserAccounts.ToList();
            if (queryType.ToString() != "All")
            {
                filteredTypes = _context.UserAccounts.Where(u => u.UserType == (UserType)Enum.Parse(typeof(UserType), queryType)).ToList();
            }          

            var filteredUsernames = filteredTypes;
            if (!queryUsername.IsNullOrEmpty())
            {
                filteredUsernames = filteredTypes.Where(u => u.UserName.Contains(queryUsername)).ToList();
            }

            var filteredDateCreation = filteredUsernames;
            if (!queryDateCreation.IsNullOrEmpty())
            {
                filteredDateCreation = filteredDateCreation.Where(u => u.DateCreated == DateTime.Parse(queryDateCreation).Date).ToList();
            }

            var filteredNames = filteredDateCreation;
            if (!queryName.IsNullOrEmpty())
            {
                filteredNames = filteredUsernames.Where(u => u.FirstName.Contains(queryName)).ToList();
            }

            var filteredLastNames = filteredNames;
            if (!queryLastName.IsNullOrEmpty())
            {
                filteredLastNames = filteredNames.Where(u => u.LastName.Contains(queryLastName)).ToList();
            }

            var filteredStatus = filteredLastNames;
            if (queryStatus.ToString() != "All")
            {
                filteredStatus = filteredLastNames.Where(u => u.UserStatus == (UserStatus)Enum.Parse(typeof(UserStatus), queryStatus)).ToList();
            }

            return View("Index", filteredStatus);
        }

        private bool UserNameExists(UserAccount userRead)
        {
            return _context.UserAccounts.Any(e => e.UserName == userRead.UserName);
        }
    }
}
