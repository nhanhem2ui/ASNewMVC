using BussinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.Tags
{
    public class IndexModel : PageModel
    {
        private readonly ITagService _tagService;

        public IndexModel(ITagService tagService)
        {
            _tagService = tagService;
        }

        public IList<Tag> Tags { get; set; }

        public void OnGet(string? filter)
        {
            var tags = _tagService.GetTags();

            if (!string.IsNullOrEmpty(filter))
            {
                tags = tags.Where(t => t.TagName.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            Tags = tags;
        }
    }
}
