
using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.Tags
{
    [Authorize(Roles = "Admin,Staff")]
    public class EditModel : PageModel
    {
        private readonly ITagService _tagService;

        public EditModel(ITagService tagService)
        {
            _tagService = tagService;
        }

        [BindProperty]
        public Tag Tag { get; set; }

        public IActionResult OnGet(int id)
        {
            var tag = _tagService.GetTagById(id);
            if (tag == null)
            {
                return NotFound();
            }
            Tag = tag;
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            if (id != Tag.TagId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _tagService.UpdateTag(Tag);
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Failed to update tag: {ex.Message}");
                return Page();
            }
        }
    }
}
