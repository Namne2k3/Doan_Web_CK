using Doan_Web_CK.Models;
using Doan_Web_CK.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doan_Web_CK.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryAdminController : Controller
    {
        private readonly ICategoryRepository _catergoryRepository;

        public CategoryAdminController(ICategoryRepository catergoryRepository)
        {
            _catergoryRepository = catergoryRepository;
        }

        // GET: Admin/CategoryAdmin
        public async Task<IActionResult> Index()
        {
            var cates = await _catergoryRepository.GetAllAsync();
            return View(cates);
        }

        // GET: Admin/CategoryAdmin/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _catergoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/CategoryAdmin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/CategoryAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                await _catergoryRepository.AddAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/CategoryAdmin/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _catergoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/CategoryAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _catergoryRepository.UpdateAsync(category);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var finded = await _catergoryRepository.GetByIdAsync(id);
                    if (finded == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/CategoryAdmin/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _catergoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/CategoryAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _catergoryRepository.GetByIdAsync(id);
            if (category != null)
            {
                await _catergoryRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CategoryExists(int id)
        {
            var existed = await _catergoryRepository.GetByIdAsync(id);
            if (existed != null)
            {
                return true;
            }
            return false;
        }
    }
}
