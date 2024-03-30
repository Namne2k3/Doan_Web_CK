using Doan_Web_CK.Models;
using Doan_Web_CK.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Doan_Web_CK.Controllers
{
    public class FriendController : Controller
    {
        private readonly IFriendShipRepository _friendShipRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotifiticationRepository _notificationRepository;

        public FriendController(IFriendShipRepository friendShipRepository, IAccountRepository accountRepository, UserManager<ApplicationUser> userManager, INotifiticationRepository notificationRepository)
        {
            _friendShipRepository = friendShipRepository;
            _accountRepository = accountRepository;
            _userManager = userManager;
            _notificationRepository = notificationRepository;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var account = await _accountRepository.GetByIdAsync(currentUser.Id);
            ViewBag.GetAllNofOfUser = new Func<string, IEnumerable<Nofitication>>(GetAllNofOfUser);
            ViewBag.IsRequested = new Func<string, string, bool>(IsRequested);
            ViewBag.GetUserName = new Func<string, string>(GetUserName);
            ViewBag.currentUser = account;
            var friends = await _friendShipRepository.GetAllAsync();
            var filtered = friends.Where(p => p.UserId == account.Id || p.FriendId == account.Id).ToList();
            return View(filtered);
        }
        public async Task<string> GetUserNameByIdAsync(string id)
        {
            var user = await _accountRepository.GetByIdAsync(id);
            return "@" + user.UserName;
        }
        public string GetUserName(string id)
        {
            var task = GetUserNameByIdAsync(id);
            task.Wait();
            return task.Result;
        }
        public async Task<IEnumerable<Nofitication>> GetAllNofOfUserAsync(string userId)
        {
            var user = await _accountRepository.GetByIdAsync(userId);

            var nofitications = await _notificationRepository.GetAllNotifitions();
            var filtered = nofitications.Where(p => p.RecieveAccountId == userId).ToList();
            return filtered;
        }
        public IEnumerable<Nofitication> GetAllNofOfUser(string userId)
        {
            var task = GetAllNofOfUserAsync(userId);
            task.Wait();
            return task.Result;
        }
        public async Task<bool> IsRequestedAsync(string userId, string friendId)
        {
            var friendships = await _friendShipRepository.GetAllAsync();
            var finded = friendships.SingleOrDefault(p => p.UserId == userId && p.FriendId == friendId && p.IsConfirmed == false);
            if (finded != null)
            {
                return true;
            }
            return false;
        }
        public bool IsRequested(string userId, string friendId)
        {
            var task = IsRequestedAsync(userId, friendId);
            task.Wait();
            return task.Result;
        }
    }
}
