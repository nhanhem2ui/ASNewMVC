using BussinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.NewsArticles
{
    [Authorize(Roles = "Admin,Staff")]
    public class ReportModel : PageModel
    {
        private readonly INewsArticleService _newsService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly ISystemAccountService _accountService;

        public ReportModel(
            INewsArticleService newsService,
            ICategoryService categoryService,
            ITagService tagService,
            ISystemAccountService accountService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
            _tagService = tagService;
            _accountService = accountService;
        }

        // Input filters
        [BindProperty(SupportsGet = true)] public DateTime? StartDate { get; set; }
        [BindProperty(SupportsGet = true)] public DateTime? EndDate { get; set; }
        [BindProperty(SupportsGet = true)] public int? CreatedById { get; set; }
        [BindProperty(SupportsGet = true)] public short? CategoryId { get; set; }
        [BindProperty(SupportsGet = true)] public string SortOrder { get; set; } = "desc";

        // Data for view
        public List<NewsArticle> Articles { get; set; } = new();
        public object Stats { get; set; }
        public List<Category> Categories { get; set; } = new();
        public List<object> Authors { get; set; } = new();

        public IActionResult OnGet()
        {
            var articles = _newsService.GetNewsArticles();

            // Apply filters
            if (CreatedById.HasValue)
                articles = articles.Where(a => a.CreatedById == CreatedById).ToList();

            if (CategoryId.HasValue)
                articles = articles.Where(a => a.CategoryId == CategoryId).ToList();

            if (StartDate.HasValue)
                articles = articles.Where(a => a.CreatedDate >= StartDate.Value).ToList();

            if (EndDate.HasValue)
                articles = articles.Where(a => a.CreatedDate <= EndDate.Value).ToList();

            // Calculate stats
            Stats = new
            {
                TotalArticles = articles.Count,
                ActiveArticles = articles.Count(a => a.NewsStatus == true),
                InactiveArticles = articles.Count(a => a.NewsStatus == false),

                ByCategory = articles.GroupBy(a => a.Category?.CategoryName ?? "Uncategorized")
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToList(),

                ByAuthor = articles.GroupBy(a => a.CreatedBy?.AccountName ?? a.CreatedById?.ToString() ?? "Unknown")
                    .Select(g => new { Author = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToList(),

                ByDate = articles.GroupBy(a => a.CreatedDate?.Date)
                    .Select(g => new { Date = g.Key?.ToString("yyyy-MM-dd") ?? "Unknown", Count = g.Count() })
                    .OrderBy(x => x.Date)
                    .ToList()
            };

            // Sort
            var sort = string.IsNullOrEmpty(SortOrder) ? "desc" : SortOrder.ToLower();
            Articles = sort == "asc"
                ? articles.OrderBy(a => a.CreatedDate).ToList()
                : articles.OrderByDescending(a => a.CreatedDate).ToList();

            // Dropdown data
            var allArticles = _newsService.GetNewsArticles();

            Categories = allArticles
                .Where(a => a.Category != null)
                .Select(a => a.Category)
                .DistinctBy(c => c.CategoryId)
                .OrderBy(c => c.CategoryName)
                .ToList();

            Authors = allArticles
                .Where(a => a.CreatedById.HasValue)
                .Select(a => new { Id = a.CreatedById.Value, Name = a.CreatedBy?.AccountName ?? a.CreatedById.ToString() })
                .DistinctBy(a => a.Id)
                .OrderBy(a => a.Name)
                .Cast<object>()
                .ToList();

            return Page();
        }
    }
}
