using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;

namespace FUNewsManagement.Pages.NewsArticles
{
    public class BrowseModel : PageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;

        public BrowseModel(INewsArticleService newsService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
        }

        public IList<NewsArticle> NewsArticles { get; set; }
        public SelectList Categories { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int TotalCount { get; set; }

        [BindProperty(SupportsGet = true)]
        public short? SelectedCategory { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; } = "newest";

        public void OnGet()
        {
            var articles = _newsService.GetNewsArticles();

            // Apply filters
            if (SelectedCategory.HasValue)
            {
                articles = articles.Where(a => a.CategoryId == SelectedCategory).ToList();
            }

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                articles = articles.Where(a => a.NewsTitle.Contains(SearchTerm) || a.NewsContent.Contains(SearchTerm)).ToList();
            }

            // Apply sorting
            articles = SortBy switch
            {
                "oldest" => articles.OrderBy(a => a.CreatedDate).ToList(),
                "title" => articles.OrderBy(a => a.NewsTitle).ToList(),
                _ => articles.OrderByDescending(a => a.CreatedDate).ToList(),
            };

            TotalCount = articles.Count;
            NewsArticles = articles.Skip((Page - 1) * PageSize).Take(PageSize).ToList();

            Categories = new SelectList(_categoryService.GetCategorys(), "CategoryId", "CategoryDesciption");
        }
    }
}
