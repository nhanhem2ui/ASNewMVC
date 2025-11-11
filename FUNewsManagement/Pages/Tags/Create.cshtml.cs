using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObject.Models;
using Service;

namespace FUNewsManagement.Pages.Tags
{
    public class CreateModel : PageModel
    {
        private readonly ITagService _tagService;

        public CreateModel(ITagService tagService)
        {
            _tagService = tagService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Tag Tag { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _tagService == null || Tag == null)
            {
                return Page();
            }

            _tagService.SaveTag(Tag);

            return RedirectToPage("./Index");
        }
    }
}
