using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public DeleteModel(ICategoryService categoryService)
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
            return Page();
        }

        public IActionResult OnPost(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                _categoryService.DeleteCategory(id.Value);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Cannot delete this category because it is referenced by other records.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
