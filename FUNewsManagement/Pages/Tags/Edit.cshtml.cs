using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObject.Models;
using Service;

namespace FUNewsManagement.Pages.Tags
{
    public class EditModel : PageModel
    {
        private readonly ITagService _tagService;

        public EditModel(ITagService tagService)
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

            var tag =  _tagService.GetTagById(id.Value);
            if (tag == null)
            {
                return NotFound();
            }
            Tag = tag;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _tagService.UpdateTag(Tag);

            return RedirectToPage("./Index");
        }
    }
}
