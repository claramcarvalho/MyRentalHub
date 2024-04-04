using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalProperties.DATA;
using System.Security.Claims;

namespace RentalProperties.Controllers
{
    public class IdentityController : Controller
    {
        private readonly RentalPropertiesDBContext _context;

        public IdentityController(RentalPropertiesDBContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserName,UserPassword,RememberMe")] UserAccount userRead)
        {
            if (UserAccountExists(userRead))
            {
                UserAccount userFound = _context.UserAccounts.FirstOrDefault(e => e.UserName == userRead.UserName);
                userFound.RememberMe = userRead.RememberMe;
                string type = userFound.UserType.ToString();
                //Creating the security context
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, userFound.UserName),
                    new Claim(ClaimTypes.NameIdentifier, userFound.UserId.ToString()),
                    new Claim("Type",type)
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties();
                authProperties.IsPersistent = userFound.RememberMe;

                await HttpContext.SignInAsync("MyCookieAuth", userPrincipal, authProperties);

                return RedirectToAction("Index", "Home");
            }
            ViewData["Message"] = "No user in our database matches the provided username and password.";
            return View();
        }

        private bool UserAccountExists(UserAccount userRead)
        {
            return _context.UserAccounts.Any(e => e.UserName == userRead.UserName && e.UserPassword == userRead.UserPassword);
        }

        private UserType UserAccountType(string userName)
        {
            UserAccount userAccount = _context.UserAccounts.FirstOrDefault(e => e.UserName == userName);
            if (userAccount != null)
            {
                return userAccount.UserType;
            }
            return UserType.Anonymous;
        }

        public IActionResult SignUp()
        {
            return View();
        }

        // POST: Identity/SignUp
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("UserId,UserType,UserName,UserPassword,DateCreated,FirstName,LastName,UserStatus")] UserAccount userAccount)
        {
            if (ModelState.IsValid)
            {
                if (UserNameExists(userAccount))
                {
                    ViewData["Message"] = "The username you entered is already being used by another user! Please choose a unique username.";
                    return View(userAccount);
                }
                userAccount.UserType = UserType.Tenant;
                _context.Add(userAccount);
                await _context.SaveChangesAsync();

                string type = userAccount.UserType.ToString();
                //Creating the security context
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, userAccount.UserName),
                    new Claim("Type",type)
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync("MyCookieAuth", userPrincipal);

                return RedirectToAction("Index", "Home");
            }
            return View(userAccount);
        }

        private bool UserNameExists(UserAccount userRead)
        {
            return _context.UserAccounts.Any(e => e.UserName == userRead.UserName);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login", "Identity");
        }
    }
}
