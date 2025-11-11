using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.SystemAccounts
{
    public class DetailsModel : PageModel
    {
        private readonly ISystemAccountService _systemAccountService;

        public DetailsModel(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

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
    }
}
