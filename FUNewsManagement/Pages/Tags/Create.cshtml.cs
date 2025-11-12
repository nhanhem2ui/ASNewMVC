
using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.Tags
{
    [Authorize(Roles = "Admin,Staff")]
    public class CreateModel : PageModel
    {
        private readonly ITagService _tagService;

        public CreateModel(ITagService tagService)
        {
            _tagService = tagService;
        }

        [BindProperty]
        public Tag Tag { get; set; }

        public IActionResult OnGet()
        {
            Tag = new Tag();
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
                _tagService.SaveTag(Tag);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Failed to create tag: {ex.Message}");
                return Page();
            }
        }
    }
}
