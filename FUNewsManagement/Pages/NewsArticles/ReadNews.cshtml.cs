using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;

namespace FUNewsManagement.Pages.NewsArticles
{
    public class ReadNewsModel : PageModel
    {
        private readonly INewsArticaleService _newsService;

        public ReadNewsModel(INewsArticaleService newsService)
        {
            _newsService = newsService;
        }

        public NewsArticle Article { get; set; }

        public IActionResult OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            Article = _newsService.GetNewsArticleById(id);

            if (Article == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
