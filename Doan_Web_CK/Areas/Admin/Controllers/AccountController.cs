using Doan_Web_CK.Models;
using Doan_Web_CK.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Doan_Web_CK.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountController(IAccountRepository accountRepository, UserManager<ApplicationUser> userManager)
        {
            _accountRepository = accountRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var accounts = await _accountRepository.GetAllAsync();
            var user = await _userManager.GetUserAsync(User);
            var adminId = "";
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                // Người dùng hiện tại là admin
                adminId = user.Id;
            }
            var accountList = accounts.Where(p => p.Id != adminId).ToList();
            return View(accountList);
        }
    }
}
