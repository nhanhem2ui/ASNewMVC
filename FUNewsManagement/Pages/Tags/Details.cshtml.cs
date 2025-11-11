using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.Tags
{
    public class DetailsModel : PageModel
    {
        private readonly ITagService _tagService;

        public DetailsModel(ITagService tagService)
        {
            _tagService = tagService;
        }

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
    }
}
