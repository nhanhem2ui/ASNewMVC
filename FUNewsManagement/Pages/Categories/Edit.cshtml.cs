using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;

namespace FUNewsManagement.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public EditModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [BindProperty]
        public Category Category { get; set; }

        public IActionResult OnGet(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category = _categoryService.GetCategoryById(id.Value);

            if (Category == null)
            {
                return NotFound();
            }

            ViewData["ParentCategoryId"] = new SelectList(_categoryService.GetCategorys(), "CategoryId", "CategoryDesciption");
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _categoryService.UpdateCategory(Category);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Failed to update category.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
