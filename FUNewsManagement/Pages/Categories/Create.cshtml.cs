using BussinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;

namespace FUNewsManagement.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public CreateModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult OnGet()
        {
            ViewData["ParentCategoryId"] = new SelectList(_categoryService.GetCategorys(), "CategoryId", "CategoryDesciption");
            return Page();
        }

        [BindProperty]
        public Category Category { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _categoryService == null || Category == null)
            {
                return Page();
            }

            _categoryService.SaveCategory(Category);

            return RedirectToPage("./Index");
        }
    }
}
