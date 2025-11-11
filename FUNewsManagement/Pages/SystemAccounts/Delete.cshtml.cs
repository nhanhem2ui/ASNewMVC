using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.SystemAccounts
{
    public class DeleteModel : PageModel
    {
        private readonly ISystemAccountService _systemAccountService;

        public DeleteModel(ISystemAccountService systemAccountService)
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
            _systemAccountService.DeleteSystemAccount(id);

            return RedirectToPage("./Index");
        }
    }
}
