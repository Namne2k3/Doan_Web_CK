using Doan_Web_CK.Models;
using Doan_Web_CK.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Doan_Web_CK.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BlogAdminController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICategoryRepository _categoryRepository;
        public BlogAdminController(IBlogRepository blogRepository, UserManager<ApplicationUser> userManager, ICategoryRepository categoryRepository)
        {
            _blogRepository = blogRepository;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index()
        {
            var blogs = await _blogRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(blogs);
        }

        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);

            blog.IsAccepted = true;
            await _blogRepository.UpdateAsync(blog);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UnAccept(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);

            blog.IsAccepted = false;
            await _blogRepository.UpdateAsync(blog);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Search(string blog_title, DateTime? blog_date, string blog_newest, string cate_filter)
        {
            var blogs = await _blogRepository.GetAllAsync();
            var filteredBlogs = blogs.Where(p => p.IsAccepted == true);
            var categories = await _categoryRepository.GetAllAsync();
            if (cate_filter != null)
            {
                filteredBlogs = filteredBlogs.Where(p => p.CategoryId.ToString() == cate_filter).ToList();
            }
            if (blog_title != null)
            {
                filteredBlogs = filteredBlogs.Where(p => p.Title.ToLower().Contains(blog_title.ToLower())).ToList();
            }

            if (blog_date.HasValue)
            {
                filteredBlogs = filteredBlogs.Where(p => p.PublishDate.Date <= blog_date.Value.Date).ToList();
            }

            if (blog_newest != null)
            {
                if (blog_newest.ToLower() == "true" || blog_newest.ToLower() == "on")
                {
                    filteredBlogs = filteredBlogs.OrderByDescending(p => p.PublishDate).ToList();
                }
            }

            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.BlogList = filteredBlogs;
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            if (currentUser != null)
            {
                ViewBag.MyBlogs = blogs.Where(p => p.AccountId == currentUser.Id);
                ViewBag.CurrentUser = currentUser;
            }
            return View("Index", filteredBlogs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            return View(blog);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _blogRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
