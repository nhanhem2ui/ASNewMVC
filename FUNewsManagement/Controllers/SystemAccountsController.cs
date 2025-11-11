using BussinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Security.Claims;

namespace FUNewsManagement.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class SystemAccountsController : Controller
    {
        private readonly ISystemAccountService _systemAccountService;

        public SystemAccountsController(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index(string? filter)
        {
            var accounts = _systemAccountService.GetSystemAccounts();

            if (!string.IsNullOrEmpty(filter))
            {
                // Check if filter matches role keywords
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
                    // General text search in name and email
                    accounts = accounts
                        .Where(a =>
                            (a.AccountName != null && a.AccountName.Contains(filter, StringComparison.OrdinalIgnoreCase)) ||
                            (a.AccountEmail != null && a.AccountEmail.Contains(filter, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                }
            }

            // Store the current filter in ViewBag for display
            ViewBag.CurrentFilter = filter;

            return View(accounts);
        }

        public IActionResult Details(short? id)
        {
            if (id == null)
                return NotFound();

            var account = _systemAccountService.GetSystemAccountById(id.Value);
            if (account == null)
                return NotFound();

            return View(account);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new SystemAccount());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SystemAccount systemAccount)
        {
            if (!ModelState.IsValid)
                return View(systemAccount);

            try
            {
                // Hash the password before saving
                if (!string.IsNullOrWhiteSpace(systemAccount.AccountPassword))
                {
                    var hasher = new PasswordHasher<object>();
                    systemAccount.AccountPassword = hasher.HashPassword(null, systemAccount.AccountPassword);
                }
                else
                {
                    // If creating via manual form (not Google), password is required
                    ModelState.AddModelError("AccountPassword", "Password is required.");
                    return View(systemAccount);
                }

                _systemAccountService.SaveSystemAccount(systemAccount);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to create account: {ex.Message}");
                return View(systemAccount);
            }
        }

        public IActionResult Edit(short? id)
        {
            if (id == null)
                return NotFound();

            var account = _systemAccountService.GetSystemAccountById(id.Value);
            if (account == null)
                return NotFound();

            ViewBag.UserRole = User.FindFirstValue(ClaimTypes.Role);
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(short id, SystemAccount systemAccount)
        {
            if (id != systemAccount.AccountId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(systemAccount);

            try
            {
                // Get the existing account from the database
                var existingAccount = _systemAccountService.GetSystemAccountById(id);
                if (existingAccount == null)
                    return NotFound();

                // Update properties
                existingAccount.AccountName = systemAccount.AccountName;
                existingAccount.AccountEmail = systemAccount.AccountEmail;
                existingAccount.AccountRole = systemAccount.AccountRole;

                // Only hash and update password if it's been changed (not null/empty)
                if (!string.IsNullOrWhiteSpace(systemAccount.AccountPassword))
                {
                    // Check if the password is already hashed (shouldn't start with plaintext)
                    // Hash the new password
                    var hasher = new PasswordHasher<object>();
                    existingAccount.AccountPassword = hasher.HashPassword(null, systemAccount.AccountPassword);
                }
                // If password is null/empty, keep the existing password (don't overwrite)

                _systemAccountService.UpdateSystemAccount(existingAccount);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to update account: {ex.Message}");
                return View(systemAccount);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(short? id)
        {
            if (id == null)
                return NotFound();

            var account = _systemAccountService.GetSystemAccountById(id.Value);
            if (account == null)
                return NotFound();

            return View(account);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(short id)
        {
            try
            {
                _systemAccountService.DeleteSystemAccount(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var account = _systemAccountService.GetSystemAccountById(id);
                if (account != null)
                {
                    ModelState.AddModelError("", $"Failed to delete account: {ex.Message}");
                    return View("Delete", account);
                }

                return RedirectToAction(nameof(Index));
            }
        }
    }
}