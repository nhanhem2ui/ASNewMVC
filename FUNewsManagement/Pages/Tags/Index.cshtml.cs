using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObject.Models;
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

        public IList<Tag> Tag { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Tag = (IList<Tag>)_tagService.GetTags();
        }
    }
}
