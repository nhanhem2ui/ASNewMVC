using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.SystemAccounts
{
    public class EditModel : PageModel
    {
        private readonly ISystemAccountService _systemAccountService;

        public EditModel(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

        [BindProperty]
        public SystemAccount SystemAccount { get; set; }

        public IActionResult OnGet(short id)
        {
            SystemAccount = _systemAccountService.GetSystemAccountById(id);

            if (SystemAccount == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost(short id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingAccount = _systemAccountService.GetSystemAccountById(id);
            if (existingAccount == null)
            {
                return NotFound();
            }

            existingAccount.AccountName = SystemAccount.AccountName;
            existingAccount.AccountEmail = SystemAccount.AccountEmail;
            existingAccount.AccountRole = SystemAccount.AccountRole;

            if (!string.IsNullOrWhiteSpace(SystemAccount.AccountPassword))
            {
                var hasher = new PasswordHasher<object>();
                existingAccount.AccountPassword = hasher.HashPassword(null, SystemAccount.AccountPassword);
            }

            _systemAccountService.UpdateSystemAccount(existingAccount);

            return RedirectToPage("./Index");
        }
    }
}
