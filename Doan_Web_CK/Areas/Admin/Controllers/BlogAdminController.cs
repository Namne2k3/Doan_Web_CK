using Doan_Web_CK.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Doan_Web_CK.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BlogAdminController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        public BlogAdminController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }
        public async Task<IActionResult> Index()
        {
            var blogs = await _blogRepository.GetAllAsync();
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
