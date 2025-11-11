using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FUNewsManagement.Pages.NewsArticles
{
    public class IndexModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;

        public IndexModel(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        public string TitleSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public IList<NewsArticle> NewsArticles { get;set; }
        
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            TitleSort = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<NewsArticle> newsArticlesIQ = _newsArticleService.GetNewsArticles().AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                newsArticlesIQ = newsArticlesIQ.Where(s => s.NewsTitle.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    newsArticlesIQ = newsArticlesIQ.OrderByDescending(s => s.NewsTitle);
                    break;
                case "Date":
                    newsArticlesIQ = newsArticlesIQ.OrderBy(s => s.CreatedDate);
                    break;
                case "date_desc":
                    newsArticlesIQ = newsArticlesIQ.OrderByDescending(s => s.CreatedDate);
                    break;
                default:
                    newsArticlesIQ = newsArticlesIQ.OrderBy(s => s.NewsTitle);
                    break;
            }
            
            var pageSize = 5;
            PageIndex = pageIndex ?? 1;
            var count = newsArticlesIQ.Count();
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            NewsArticles = newsArticlesIQ.Skip((PageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}
