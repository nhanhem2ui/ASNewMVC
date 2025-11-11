using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;
using System;
using System.Linq;
using System.Security.Claims;

namespace FUNewsManagement.Pages.NewsArticles
{
    public class EditModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;

        public EditModel(INewsArticleService newsArticleService, ICategoryService categoryService, ITagService tagService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; }

        private short GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (short.TryParse(userIdClaim, out short userId))
            {
                return userId;
            }
            return 0;
        }

        public IActionResult OnGet(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            NewsArticle = _newsArticleService.GetNewsArticleById(id);

            if (NewsArticle == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Staff") && NewsArticle.NewsStatus == true)
            {
                return RedirectToPage("./Index");
            }

            ViewData["CategoryId"] = new SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryDesciption", NewsArticle.CategoryId);
            ViewData["AllTags"] = new MultiSelectList(_tagService.GetTags(), "TagId", "TagName", NewsArticle.Tags?.Select(t => t.TagId));

            return Page();
        }

        public IActionResult OnPost(string id, int[] selectedTags)
        {
            if (id != NewsArticle.NewsArticleId)
            {
                return NotFound();
            }

            ModelState.Remove("NewsArticle.UpdatedById");
            ModelState.Remove("NewsArticle.ModifiedDate");

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryDesciption", NewsArticle.CategoryId);
                ViewData["AllTags"] = new MultiSelectList(_tagService.GetTags(), "TagId", "TagName", selectedTags);
                return Page();
            }
            
            var currentUserId = GetCurrentUserId();
            NewsArticle.UpdatedById = currentUserId;
            NewsArticle.ModifiedDate = DateTime.Now;

            if (selectedTags is { Length: > 0 })
                {
                    var tags = _tagService.GetTags();
                    NewsArticle.Tags = tags.Where(t => selectedTags.Contains(t.TagId)).ToList();
                }
                else
                {
                    NewsArticle.Tags = new System.Collections.Generic.List<Tag>();
                }

            _newsArticleService.UpdateNewsArticle(NewsArticle);

            return RedirectToPage("./Index");
        }
    }
}
