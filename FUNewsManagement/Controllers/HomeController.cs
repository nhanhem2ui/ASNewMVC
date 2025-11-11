using BussinessObject;
using FUNewsManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Diagnostics;

namespace FUNewsManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly INewsArticaleService _newsService;
        private readonly ITagService _tagService;

        public HomeController(INewsArticaleService newsService, ITagService tagService)
        {
            _newsService = newsService;
            _tagService = tagService;
        }

        [HttpGet]
        public IActionResult Index(string searchTerm)
        {
            var articles = _newsService.GetNewsArticles().ToList();
            var tags = _tagService.GetTags().ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                articles = articles.Where(a =>
                    (a.NewsTitle != null && a.NewsTitle.Contains(searchLower, StringComparison.CurrentCultureIgnoreCase)) ||
                    (a.Headline != null && a.Headline.Contains(searchLower, StringComparison.CurrentCultureIgnoreCase)) ||
                    (a.Category != null && a.Category.CategoryName != null && a.Category.CategoryName.Contains(searchLower, StringComparison.CurrentCultureIgnoreCase)) ||
                    (a.Tags != null && a.Tags.Any(t => t.TagName != null && t.TagName.Contains(searchLower, StringComparison.CurrentCultureIgnoreCase)))
                ).ToList();
            }

            // Statistics
            ViewBag.TotalArticles = articles.Count;
            ViewBag.TotalCategories = articles
                .Where(a => a.Category != null)
                .Select(a => a.Category.CategoryId)
                .Distinct()
                .Count();
            ViewBag.TotalTags = tags.Count;
            ViewBag.ActiveArticles = articles.Count(a => a.NewsStatus == true);
            ViewBag.SearchTerm = searchTerm;

            return View(articles);
        }

        [HttpGet("{id}")]
        public IActionResult Details(string id)
        {
            var article = _newsService.GetNewsArticles().FirstOrDefault(a => a.NewsArticleId == id);
            if (article == null)
                return NotFound();

            return View(article);
        }

        [HttpPost]
        public IActionResult Create([FromForm] NewsArticle article)
        {
            if (!ModelState.IsValid)
                return View(article);

            _newsService.SaveNewsArticle(article);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Update([FromForm] NewsArticle article)
        {
            if (article == null || string.IsNullOrEmpty(article.NewsArticleId))
                return BadRequest();

            _newsService.UpdateNewsArticle(article);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            _newsService.DeleteNewsArticle(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}