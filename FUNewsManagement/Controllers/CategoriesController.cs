using BussinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;

namespace FUNewsManagement.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var categories = _categoryService.GetCategorys();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Details(short? id)
        {
            if (id == null)
                return NotFound();

            var category = _categoryService.GetCategoryById(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _categoryService.GetCategorys();
            ViewData["ParentCategoryId"] = new SelectList(categories, "CategoryId", "CategoryDesciption");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryService.SaveCategory(category);
                return RedirectToAction(nameof(Index));
            }

            var categories = _categoryService.GetCategorys();
            ViewData["ParentCategoryId"] = new SelectList(categories, "CategoryId", "CategoryDesciption", category.ParentCategoryId);
            return View(category);
        }

        [HttpGet]
        public IActionResult Edit(short? id)
        {
            if (id == null)
                return NotFound();

            var category = _categoryService.GetCategoryById(id.Value);
            if (category == null)
                return NotFound();

            var categories = _categoryService.GetCategorys();
            ViewData["ParentCategoryId"] = new SelectList(categories, "CategoryId", "CategoryDesciption", category.ParentCategoryId);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(short id, Category category)
        {
            if (id != category.CategoryId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _categoryService.UpdateCategory(category);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Failed to update category.");
                }
            }

            var categories = _categoryService.GetCategorys();
            ViewData["ParentCategoryId"] = new SelectList(categories, "CategoryId", "CategoryDesciption", category.ParentCategoryId);
            return View(category);
        }

        [HttpGet]
        public IActionResult Delete(short? id)
        {
            if (id == null)
                return NotFound();

            var category = _categoryService.GetCategoryById(id.Value);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(short id)
        {
            try
            {
                _categoryService.DeleteCategory(id);
            }
            catch
            {
                var category = _categoryService.GetCategoryById(id);
                if (category != null)
                {
                    ModelState.AddModelError(string.Empty, "Cannot delete this category because it is referenced by other records.");
                    return View("Delete", category);
                }
            }

            return RedirectToAction(nameof(Index));
        }
        private bool CategoryExists(short id)
        {
            var category = _categoryService.GetCategoryById(id);
            return category != null;
        }
    }
}
