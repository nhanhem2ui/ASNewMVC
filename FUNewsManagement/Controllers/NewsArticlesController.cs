using BussinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;
using System.Security.Claims;

namespace FUNewsManagement.Controllers
{
    [Authorize(Roles = "Admin,Staff,Lecturer")]
    public class NewsArticlesController : Controller
    {
        private readonly INewsArticaleService _newsService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly ISystemAccountService _accountService;

        public NewsArticlesController(
            INewsArticaleService newsService,
            ICategoryService categoryService,
            ITagService tagService,
            ISystemAccountService accountService)
        {
            _newsService = newsService;
            _categoryService = categoryService;
            _tagService = tagService;
            _accountService = accountService;
        }

        // Helper method to get current user ID from claims
        private short GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (short.TryParse(userIdClaim, out short userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User ID not found in claims");
        }

        // GET: NewsArticles
        [HttpGet]
        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            var articles = _newsService.GetNewsArticles();

            var totalCount = articles.Count;
            var pagedArticles = articles
                .OrderByDescending(a => a.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(pagedArticles);
        }

        // GET: NewsArticles/Details/id
        [HttpGet]
        public IActionResult Details(string id)
        {
            if (id == null) return NotFound();

            var article = _newsService.GetNewsArticleById(id);
            if (article == null) return NotFound();

            return View(article);
        }

        // GET: NewsArticles/Create
        [Authorize(Roles = "Admin,Staff, Lecturer")]
        [HttpGet]
        public IActionResult Create()
        {
            var categories = _categoryService.GetCategorys();
            var tags = _tagService.GetTags();

            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryDesciption");
            ViewData["AllTags"] = new MultiSelectList(tags, "TagId", "TagName");

            return View(new NewsArticle());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff,Lecturer")]
        public IActionResult Create(NewsArticle newsArticle, int[] selectedTags)
        {
            // Remove validation for fields that will be set automatically
            ModelState.Remove(nameof(newsArticle.CreatedById));
            ModelState.Remove(nameof(newsArticle.CreatedDate));
            ModelState.Remove(nameof(newsArticle.UpdatedById));
            ModelState.Remove(nameof(newsArticle.ModifiedDate));

            try
            {
                var currentUserId = GetCurrentUserId();

                newsArticle.NewsArticleId = DateTime.Now.ToString("yyyyMMddHHmmssfff") + new Random().Next(100, 999);

                newsArticle.CreatedById = currentUserId;
                newsArticle.CreatedDate = DateTime.Now;
                newsArticle.UpdatedById = currentUserId;
                newsArticle.ModifiedDate = DateTime.Now;

                // Handle tags - attach existing tags without adding them as new entities
                if (selectedTags != null && selectedTags.Length > 0)
                {
                    // Create a list of tag references
                    newsArticle.Tags = selectedTags.Select(tagId => new Tag { TagId = tagId }).ToList();
                }
                else
                {
                    newsArticle.Tags = new List<Tag>();
                }

                _newsService.SaveNewsArticle(newsArticle);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                var categories = _categoryService.GetCategorys();
                var tagsAll = _tagService.GetTags();

                ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryDesciption", newsArticle.CategoryId);
                ViewData["AllTags"] = new MultiSelectList(tagsAll, "TagId", "TagName", selectedTags);

                return View(newsArticle);
            }
        }

        // GET: NewsArticles/Edit/5
        [Authorize(Roles = "Admin,Staff,Lecturer")]
        public IActionResult Edit(string id)
        {
            if (id == null) return NotFound();
            var newsArticle = _newsService.GetNewsArticleById(id);

            if (User.IsInRole("Staff") && newsArticle.NewsStatus == true)
            {
                return RedirectToAction("Index");
            }
            var article = _newsService.GetNewsArticleById(id);
            if (article == null) return NotFound();

            var categories = _categoryService.GetCategorys();
            var tags = _tagService.GetTags();

            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryDesciption", article.CategoryId);
            ViewData["AllTags"] = new MultiSelectList(tags, "TagId", "TagName", article.Tags?.Select(t => t.TagId));

            return View(article);
        }

        // POST: NewsArticles/Edit/5
        [Authorize(Roles = "Admin,Staff,Lecturer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, NewsArticle newsArticle, int[] selectedTags)
        {
            if (id != newsArticle.NewsArticleId) return NotFound();

            // Remove validation for fields that will be updated automatically
            ModelState.Remove(nameof(newsArticle.UpdatedById));
            ModelState.Remove(nameof(newsArticle.ModifiedDate));

            try
            {
                // Update modifier and modification date
                var currentUserId = GetCurrentUserId();
                newsArticle.UpdatedById = currentUserId;
                newsArticle.ModifiedDate = DateTime.Now;

                if (selectedTags is { Length: > 0 })
                {
                    var tags = _tagService.GetTags();
                    newsArticle.Tags = tags.Where(t => selectedTags.Contains(t.TagId)).ToList();
                }
                else
                {
                    newsArticle.Tags = new List<Tag>();
                }

                _newsService.UpdateNewsArticle(newsArticle);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                var categories = _categoryService.GetCategorys();
                var tagsAll = _tagService.GetTags();

                ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryDesciption", newsArticle.CategoryId);
                ViewData["AllTags"] = new MultiSelectList(tagsAll, "TagId", "TagName", selectedTags);

                return View(newsArticle);
            }
        }

        // GET: NewsArticles/Delete/5
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Delete(string id)
        {
            var article = _newsService.GetNewsArticleById(id);
            if (article == null) return NotFound();

            return View(article);
        }

        // POST: NewsArticles/Delete/5
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            _newsService.DeleteNewsArticle(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Report(DateTime? startDate, DateTime? endDate, int? createdById, short? categoryId, string sortOrder)
        {
            var articles = _newsService.GetNewsArticles();

            // Apply filters
            if (createdById.HasValue)
                articles = articles.Where(a => a.CreatedById == createdById).ToList();

            if (categoryId.HasValue)
                articles = articles.Where(a => a.CategoryId == categoryId).ToList();

            if (startDate.HasValue)
                articles = articles.Where(a => a.CreatedDate >= startDate.Value).ToList();

            if (endDate.HasValue)
                articles = articles.Where(a => a.CreatedDate <= endDate.Value).ToList();

            // Calculate statistics
            var stats = new
            {
                TotalArticles = articles.Count,
                ActiveArticles = articles.Count(a => a.NewsStatus == true),
                InactiveArticles = articles.Count(a => a.NewsStatus == false),

                // Group by category
                ByCategory = articles.GroupBy(a => a.Category?.CategoryName ?? "Uncategorized")
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToList(),

                // Group by author
                ByAuthor = articles.GroupBy(a => a.CreatedBy?.AccountName ?? a.CreatedById?.ToString() ?? "Unknown")
                    .Select(g => new { Author = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .ToList(),

                // Articles by date (for chart)
                ByDate = articles.GroupBy(a => a.CreatedDate?.Date)
                    .Select(g => new { Date = g.Key?.ToString("yyyy-MM-dd") ?? "Unknown", Count = g.Count() })
                    .OrderBy(x => x.Date)
                    .ToList()
            };

            // Sort articles
            sortOrder = string.IsNullOrEmpty(sortOrder) ? "desc" : sortOrder.ToLower();
            articles = sortOrder == "asc"
                ? articles.OrderBy(a => a.CreatedDate).ToList()
                : articles.OrderByDescending(a => a.CreatedDate).ToList();

            // Get filter dropdowns data
            var allArticles = _newsService.GetNewsArticles();

            // Get unique categories from articles
            var categories = allArticles
                .Where(a => a.Category != null)
                .Select(a => a.Category)
                .DistinctBy(c => c.CategoryId)
                .OrderBy(c => c.CategoryName)
                .ToList();

            var authors = allArticles
                .Where(a => a.CreatedById.HasValue)
                .Select(a => new { Id = a.CreatedById.Value, Name = a.CreatedBy?.AccountName ?? a.CreatedById.ToString() })
                .DistinctBy(a => a.Id)
                .OrderBy(a => a.Name)
                .ToList();

            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.CreatedById = createdById;
            ViewBag.CategoryId = categoryId;
            ViewBag.SortOrder = sortOrder == "asc" ? "desc" : "asc";
            ViewBag.Stats = stats;
            ViewBag.Categories = categories;
            ViewBag.Authors = authors;

            return View(articles);
        }   

        [HttpGet]
        public IActionResult Browse(int page = 1, int pageSize = 9, short? categoryId = null, string searchTerm = null, string sortBy = "newest")
        {
            var articles = _newsService.GetNewsArticles();

            // Apply filters
            if (categoryId.HasValue)
                articles = articles.Where(a => a.CategoryId == categoryId).ToList();

            if (!string.IsNullOrEmpty(searchTerm))
                articles = articles.Where(a => a.NewsTitle.Contains(searchTerm) ||
                                              a.NewsContent.Contains(searchTerm)).ToList();

            // Apply sorting
            articles = sortBy switch
            {
                "oldest" => articles.OrderBy(a => a.CreatedDate).ToList(),
                "title" => articles.OrderBy(a => a.NewsTitle).ToList(),
                _ => articles.OrderByDescending(a => a.CreatedDate).ToList()
            };

            var totalCount = articles.Count;
            var pagedArticles = articles.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.Categories = _categoryService.GetCategorys();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;

            return View(pagedArticles);
        }

        [HttpGet]
        public IActionResult ReadNews(string? id)
        {
            var article = _newsService.GetNewsArticleById(id);
            return View(article ?? null);
        }
    }
}