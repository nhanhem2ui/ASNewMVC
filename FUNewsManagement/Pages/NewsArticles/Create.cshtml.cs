using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;
using System.Security.Claims;

namespace FUNewsManagement.Pages.NewsArticles
{
    public class CreateModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;

        public CreateModel(INewsArticleService newsArticleService, ICategoryService categoryService, ITagService tagService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; }

        [BindProperty]
        public bool IsPublished { get; set; }

        public MultiSelectList AllTags { get; set; }

        [BindProperty]
        public List<string> SelectedTags { get; set; } = new List<string>();

        private short GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (short.TryParse(userIdClaim, out short userId))
            {
                return userId;
            }
            return 0;
        }

        public IActionResult OnGet()
        {
            ViewData["CategoryId"] = new SelectList(_categoryService.GetCategorys(), "CategoryId", "CategoryDesciption");
            AllTags = new MultiSelectList(_tagService.GetTags(), "TagId", "TagName");
            NewsArticle = new NewsArticle(); // Initialize NewsArticle
            return Page();
        }


        public IActionResult OnPost()
        {
            AllTags = new MultiSelectList(_tagService.GetTags(), "TagId", "TagName");

            NewsArticle.NewsStatus = IsPublished;

            ModelState.Remove("NewsArticle.CreatedById");
            ModelState.Remove("NewsArticle.CreatedDate");
            ModelState.Remove("NewsArticle.UpdatedById");
            ModelState.Remove("NewsArticle.ModifiedDate");


            var currentUserId = GetCurrentUserId();

            NewsArticle.NewsArticleId = DateTime.Now.ToString("yyyyMMddHHmmssfff") + new Random().Next(100, 999);
            NewsArticle.CreatedById = currentUserId;
            NewsArticle.CreatedDate = DateTime.Now;
            NewsArticle.UpdatedById = currentUserId;
            NewsArticle.ModifiedDate = DateTime.Now;

            if (SelectedTags != null && SelectedTags.Any())
            {
                NewsArticle.Tags = _tagService.GetTags().Where(t => SelectedTags.Contains(t.TagId.ToString())).ToList();
            }
            else
            {
                NewsArticle.Tags = new List<Tag>();
            }

            _newsArticleService.SaveNewsArticle(NewsArticle);

            return RedirectToPage("./Index");
        }
    }
}
