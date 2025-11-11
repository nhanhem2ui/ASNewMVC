using BussinessObject;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.SystemAccounts
{
    public class IndexModel : PageModel
    {
        private readonly ISystemAccountService _systemAccountService;

        public IndexModel(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

        public IList<SystemAccount> SystemAccounts { get; set; }
        public string CurrentFilter { get; set; }

        public void OnGet(string filter)
        {
            var accounts = _systemAccountService.GetSystemAccounts();

            if (!string.IsNullOrEmpty(filter))
            {
                if (filter.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    accounts = accounts.Where(a => a.AccountRole == 1).ToList();
                }
                else if (filter.Equals("staff", StringComparison.OrdinalIgnoreCase))
                {
                    accounts = accounts.Where(a => a.AccountRole == 2).ToList();
                }
                else if (filter.Equals("lecturer", StringComparison.OrdinalIgnoreCase))
                {
                    accounts = accounts.Where(a => a.AccountRole == 3).ToList();
                }
                else
                {
                    accounts = accounts
                        .Where(a =>
                            (a.AccountName != null && a.AccountName.Contains(filter, StringComparison.OrdinalIgnoreCase)) ||
                            (a.AccountEmail != null && a.AccountEmail.Contains(filter, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }
            }

            CurrentFilter = filter;
            SystemAccounts = accounts;
        }
    }
}
