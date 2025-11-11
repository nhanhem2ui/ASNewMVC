using BusinessObjects;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;
using System.Collections.Generic;
using System.Linq;

namespace FUNewsManagement.Pages.NewsArticles
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;

        public IndexModel(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        public IList<NewsArticle> NewsArticles { get; set; }

        public void OnGet(int page = 1, int pageSize = 5)
        {
            var articles = _newsArticleService.GetNewsArticles();
            var totalCount = articles.Count;
            var pagedArticles = articles
                .OrderByDescending(a => a.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalCount"] = totalCount;

            NewsArticles = pagedArticles;
        }
    }
}
