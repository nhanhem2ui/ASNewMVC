using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.NewsArticles
{
    public class DetailsModel : PageModel
    {
        private readonly INewsArticleService _newsArticleService;

        public DetailsModel(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        public NewsArticle NewsArticle { get; set; }

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
            return Page();
        }
    }
}
