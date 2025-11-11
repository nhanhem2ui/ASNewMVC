using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.Categories
{
    public class DetailsModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public DetailsModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

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
    }
}
