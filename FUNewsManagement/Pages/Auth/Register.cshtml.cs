using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;
using System.ComponentModel.DataAnnotations;

namespace FUNewsManagement.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly ISystemAccountService _accountService;

        public RegisterModel(ISystemAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingUser = _accountService.GetSystemAccounts().FirstOrDefault(a => a.AccountEmail == Input.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Email already registered.");
                return Page();
            }

            var hasher = new PasswordHasher<object>();
            var hashedPassword = hasher.HashPassword(null, Input.Password);

            var newUser = new SystemAccount
            {
                AccountName = Input.Name,
                AccountEmail = Input.Email,
                AccountPassword = hashedPassword,
                AccountRole = 2 // default role
            };

            _accountService.SaveSystemAccount(newUser);

            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToPage("./Login");
        }
    }
}
