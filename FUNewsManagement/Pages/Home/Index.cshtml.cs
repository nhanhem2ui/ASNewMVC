using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ITagService _tagService;

        public IndexModel(INewsArticleService newsService, ITagService tagService)
        {
            _newsService = newsService;
            _tagService = tagService;
        }

        // This is your model that the Razor page receives
        public List<NewsArticle> Articles { get; set; } = new();

        // Bind search term
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        // Stats
        public int TotalArticles { get; set; }
        public int TotalCategories { get; set; }
        public int TotalTags { get; set; }
        public int ActiveArticles { get; set; }

        public void OnGet()
        {
            var articles = _newsService.GetNewsArticles().ToList();
            var tags = _tagService.GetTags().ToList();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var searchLower = SearchTerm.ToLower();
                articles = articles.Where(a =>
                       a.NewsTitle != null && a.NewsTitle.Contains(searchLower, StringComparison.CurrentCultureIgnoreCase) ||
                       a.Headline != null && a.Headline.Contains(searchLower, StringComparison.CurrentCultureIgnoreCase) ||
                       a.Category != null && a.Category.CategoryName.Contains(searchLower, StringComparison.CurrentCultureIgnoreCase) ||
                       a.Tags != null && a.Tags.Any(t => t.TagName.Contains(searchLower, StringComparison.CurrentCultureIgnoreCase))
                ).ToList();
            }

            // Assign to properties
            Articles = articles;

            TotalArticles = articles.Count;
            TotalCategories = articles
                .Where(a => a.Category != null)
                .Select(a => a.Category.CategoryId)
                .Distinct()
                .Count();

            TotalTags = tags.Count;
            ActiveArticles = articles.Count(a => a.NewsStatus == true);
        }
    }
}
