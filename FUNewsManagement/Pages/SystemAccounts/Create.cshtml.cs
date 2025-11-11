using BussinessObject;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.SystemAccounts
{
    public class CreateModel : PageModel
    {
        private readonly ISystemAccountService _systemAccountService;

        public CreateModel(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

        [BindProperty]
        public SystemAccount SystemAccount { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            if (!string.IsNullOrWhiteSpace(SystemAccount.AccountPassword))
            {
                var hasher = new PasswordHasher<object>();
                SystemAccount.AccountPassword = hasher.HashPassword(null, SystemAccount.AccountPassword);
            }
            else
            {
                ModelState.AddModelError("SystemAccount.AccountPassword", "Password is required.");
                return Page();
            }

            _systemAccountService.SaveSystemAccount(SystemAccount);

            return RedirectToPage("./Index");
        }
    }
}
