using BussinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.Tags
{
    public class DeleteModel : PageModel
    {
        private readonly ITagService _tagService;

        public DeleteModel(ITagService tagService)
        {
            _tagService = tagService;
        }

        [BindProperty]
        public Tag Tag { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _tagService == null)
            {
                return NotFound();
            }

            var tag = _tagService.GetTagById(id.Value);

            if (tag == null)
            {
                return NotFound();
            }
            else
            {
                Tag = tag;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _tagService == null)
            {
                return NotFound();
            }
            var tag = _tagService.GetTagById(id.Value);

            if (tag != null)
            {
                Tag = tag;
                _tagService.DeleteTag(Tag.TagId);
            }

            return RedirectToPage("./Index");
        }
    }
}
