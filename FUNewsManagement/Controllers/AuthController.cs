using BussinessObject;
using FUNewsManagement.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Security.Claims;

namespace FUNewsManagement.Controllers
{
    public class AuthController : Controller
    {
        private readonly ISystemAccountService _accountService;
        private readonly IConfiguration _configuration;

        public AuthController(ISystemAccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var adminEmail = _configuration["Admin:AdminEmail"];
            var adminPassword = _configuration["Admin:AdminPassword"];

            // --- Admin login (plaintext from config, not hashed) ---
            if (model.Email == adminEmail && model.Password == adminPassword)
            {
                var adminClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "Administrator"),
            new Claim(ClaimTypes.NameIdentifier, "-100"),
            new Claim(ClaimTypes.Email, adminEmail),
            new Claim(ClaimTypes.Role, "Admin")
        };

                var adminIdentity = new ClaimsIdentity(adminClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(adminIdentity));

                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("UserEmail", adminEmail);

                return RedirectToAction("Index", "SystemAccounts");
            }

            var user = _accountService
                .GetSystemAccounts()
                .FirstOrDefault(a => a.AccountEmail == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(user.AccountPassword))
            {
                ModelState.AddModelError("", "This account uses Google login. Please sign in with Google.");
                return View(model);
            }

            var hasher = new PasswordHasher<object>();
            PasswordVerificationResult result;

            try
            {
                result = hasher.VerifyHashedPassword(null, user.AccountPassword, model.Password);
            }
            catch (FormatException)
            {
                // Password is not properly hashed - this indicates a data issue
                // Log this error for investigation
                ModelState.AddModelError("", "Account error. Please contact support or try resetting your password.");
                return View(model);
            }

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            // Optionally rehash password if it needs updating
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.AccountPassword = hasher.HashPassword(null, model.Password);
                _accountService.SaveSystemAccount(user);
            }

            // Create user claims and sign in
            await SignInUserWithClaimsAndSession(user);

            return user.RoleDisplay switch
            {
                "Admin" => RedirectToAction("Index", "SystemAccounts"),
                "Lecturer" => RedirectToAction("Index", "NewsArticles"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        [HttpPost]
        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var redirectUrl = Url.Action("GoogleCallback", "Auth", new { returnUrl });
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Action("Index", "Home");

            if (remoteError != null)
            {
                TempData["ErrorMessage"] = $"Error from external provider: {remoteError}";
                return RedirectToAction("Login");
            }

            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                TempData["ErrorMessage"] = "Error loading external login information.";
                return RedirectToAction("Login");
            }

            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            var fullName = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);
            var googleId = authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Email not received from Google.";
                return RedirectToAction("Login");
            }

            // Check if user exists
            var existingUser = _accountService
                .GetSystemAccounts()
                .FirstOrDefault(a => a.AccountEmail == email);

            if (existingUser != null)
            {
                // User exists, sign them in
                await SignInUserWithClaimsAndSession(existingUser);

                return existingUser.RoleDisplay switch
                {
                    "Admin" => RedirectToAction("Index", "SystemAccounts"),
                    "Lecturer" => RedirectToAction("Index", "NewsArticles"),
                    _ => LocalRedirect(returnUrl)
                };
            }

            var newUser = new SystemAccount
            {
                AccountName = fullName ?? email.Split('@')[0],
                AccountEmail = email,
                AccountPassword = null,
                AccountRole = 2
            };

            _accountService.SaveSystemAccount(newUser);

            // Retrieve the newly created user to get the AccountId
            var createdUser = _accountService
                .GetSystemAccounts()
                .FirstOrDefault(a => a.AccountEmail == email);

            if (createdUser != null)
            {
                await SignInUserWithClaimsAndSession(createdUser);
                return LocalRedirect(returnUrl);
            }

            TempData["ErrorMessage"] = "Error creating account from Google login.";
            return RedirectToAction("Login");
        }

        private async Task SignInUserWithClaimsAndSession(SystemAccount user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.AccountName),
                new Claim(ClaimTypes.NameIdentifier, user.AccountId.ToString()),
                new Claim(ClaimTypes.Email, user.AccountEmail),
                new Claim(ClaimTypes.Role, user.RoleDisplay)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            HttpContext.Session.SetString("UserRole", user.RoleDisplay);
            HttpContext.Session.SetString("UserEmail", user.AccountEmail);
            HttpContext.Session.SetInt32("UserId", user.AccountId);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = _accountService
                .GetSystemAccounts()
                .FirstOrDefault(a => a.AccountEmail == model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "Email already registered.");
                return View(model);
            }

            var hasher = new PasswordHasher<object>();
            var hashedPassword = hasher.HashPassword(null, model.Password);

            var newUser = new SystemAccount
            {
                AccountName = model.Name,
                AccountEmail = model.Email,
                AccountPassword = hashedPassword,
                AccountRole = 2 // default role, can be changed
            };

            _accountService.SaveSystemAccount(newUser);

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Accounts()
        {
            var accounts = _accountService.GetSystemAccounts();
            return View(accounts);
        }
    }
}