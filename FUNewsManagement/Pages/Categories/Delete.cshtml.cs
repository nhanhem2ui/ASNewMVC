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
        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short? id)
        {
            if (id == null || _categoryService == null)
            {
                return NotFound();
            }

            var category = _categoryService.GetCategoryById(id.Value);

            if (category == null)
            {
                return NotFound();
            }
            else
            {
                Category = category;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(short? id)
        {
            if (id == null || _categoryService == null)
            {
                return NotFound();
            }
            var category = _categoryService.GetCategoryById(id.Value);

            if (category != null)
            {
                Category = category;
                _categoryService.DeleteCategory(Category.CategoryId);
            }

            return RedirectToPage("./Index");
        }
    }
}
