using BussinessObject;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FUNewsManagement.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly ISystemAccountService _accountService;
        private readonly IConfiguration _configuration;

        public LoginModel(ISystemAccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var adminEmail = _configuration["Admin:AdminEmail"];
            var adminPassword = _configuration["Admin:AdminPassword"];

            if (Input.Email == adminEmail && Input.Password == adminPassword)
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

                return RedirectToPage("/SystemAccounts/Index");
            }

            var user = _accountService.GetSystemAccounts().FirstOrDefault(a => a.AccountEmail == Input.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            if (string.IsNullOrWhiteSpace(user.AccountPassword))
            {
                ModelState.AddModelError(string.Empty, "This account uses Google login. Please sign in with Google.");
                return Page();
            }

            var hasher = new PasswordHasher<object>();
            var result = hasher.VerifyHashedPassword(null, user.AccountPassword, Input.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            await SignInUserWithClaimsAndSession(user);

            return user.AccountRole switch
            {
                1 => RedirectToPage("/SystemAccounts/Index"),
                _ => RedirectToPage("/NewsArticles/Index"),
            };
        }

        public IActionResult OnPostGoogleLogin()
        {
            var redirectUrl = Url.Page("./Login", pageHandler: "GoogleCallback");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Google");
        }

        public async Task<IActionResult> OnGetGoogleCallbackAsync()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Google");

            if (!authenticateResult.Succeeded)
            {
                TempData["ErrorMessage"] = "Error loading external login information.";
                return RedirectToPage("./Login");
            }

            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            var fullName = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Email not received from Google.";
                return RedirectToPage("./Login");
            }

            var user = _accountService.GetSystemAccounts().FirstOrDefault(a => a.AccountEmail == email);

            if (user == null)
            {
                user = new SystemAccount
                {
                    AccountName = fullName,
                    AccountEmail = email,
                    AccountRole = 2 // Default role
                };
                _accountService.SaveSystemAccount(user);
                user = _accountService.GetSystemAccounts().FirstOrDefault(a => a.AccountEmail == email);
            }

            await SignInUserWithClaimsAndSession(user);

            return user.AccountRole switch
            {
                1 => RedirectToPage("/SystemAccounts/Index"),
                _ => RedirectToPage("/NewsArticles/Index"),
            };
        }

        private async Task SignInUserWithClaimsAndSession(SystemAccount user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.AccountName),
                new Claim(ClaimTypes.NameIdentifier, user.AccountId.ToString()),
                new Claim(ClaimTypes.Email, user.AccountEmail),
                new Claim(ClaimTypes.Role, user.AccountRole == 1 ? "Admin" : "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            HttpContext.Session.SetString("UserRole", user.AccountRole == 1 ? "Admin" : "User");
            HttpContext.Session.SetString("UserEmail", user.AccountEmail);
            HttpContext.Session.SetInt32("UserId", user.AccountId);
        }
    }
}
