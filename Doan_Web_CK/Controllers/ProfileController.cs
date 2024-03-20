using Doan_Web_CK.Models;
using Doan_Web_CK.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Doan_Web_CK.Controllers
{
    [Authorize(Roles = "Member")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountRepository _accountRepository;
        private readonly IBlogRepository _blogRepository;
        public ProfileController(UserManager<ApplicationUser> userManager, IAccountRepository accountRepository, IBlogRepository blogRepository)
        {
            _userManager = userManager;
            _accountRepository = accountRepository;
            _blogRepository = blogRepository;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var account = await _accountRepository.GetByIdAsync(user.Id);
            var blogs = await _blogRepository.GetAllAsync();

            var accountBlogs = blogs.Where(p => p.AccountId == user.Id).ToList();
            ViewBag.Account = account;
            ViewBag.blogList = accountBlogs;
            ViewBag.currentUser = user;
            ViewBag.GetPhotoById = new Func<string, string>(GetPhotoById);
            ViewBag.GetUserName = new Func<string, string>(GetUserName);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IFormFile profile_photo)
        {
            var user = await _userManager.GetUserAsync(User);
            user.ImageUrl = await SaveImage(profile_photo);

            await _accountRepository.UpdateAsync(user);

            var account = await _accountRepository.GetByIdAsync(user.Id);
            ViewBag.Account = account;
            return RedirectToAction("Index");
        }
        private async Task<string> SaveImage(IFormFile imageFile)
        {
            var savePath = Path.Combine("wwwroot/images", imageFile.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            Console.WriteLine("/images/" + imageFile.FileName);
            return "/images/" + imageFile.FileName;
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
        public async Task<string> GetPhotoByIdAsync(string id)
        {
            var user = await _accountRepository.GetByIdAsync(id);
            return user.ImageUrl;
        }
        public string GetPhotoById(string id)
        {
            var task = GetPhotoByIdAsync(id);
            task.Wait();
            return task.Result;
        }
    }
}
