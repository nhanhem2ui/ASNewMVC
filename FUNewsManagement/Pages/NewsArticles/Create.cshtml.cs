using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
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
            ViewData["CategoryId"] = new SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryDesciption");
            ViewData["AllTags"] = new MultiSelectList(_tagService.GetTags(), "TagId", "TagName");
            return Page();
        }


        public IActionResult OnPost(int[] selectedTags)
        {
            ModelState.Remove("NewsArticle.CreatedById");
            ModelState.Remove("NewsArticle.CreatedDate");
            ModelState.Remove("NewsArticle.UpdatedById");
            ModelState.Remove("NewsArticle.ModifiedDate");

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_categoryService.GetCategories(), "CategoryId", "CategoryDesciption");
                ViewData["AllTags"] = new MultiSelectList(_tagService.GetTags(), "TagId", "TagName");
                return Page();
            }
            
            var currentUserId = GetCurrentUserId();

            NewsArticle.NewsArticleId = DateTime.Now.ToString("yyyyMMddHHmmssfff") + new Random().Next(100, 999);
            NewsArticle.CreatedById = currentUserId;
            NewsArticle.CreatedDate = DateTime.Now;
            NewsArticle.UpdatedById = currentUserId;
            NewsArticle.ModifiedDate = DateTime.Now;

            if (selectedTags != null && selectedTags.Length > 0)
            {
                NewsArticle.Tags = selectedTags.Select(tagId => new Tag { TagId = tagId }).ToList();
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
