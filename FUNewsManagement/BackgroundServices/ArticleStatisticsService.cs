using Service;

namespace FUNewsManagement.BackgroundServices
{
    /// <summary>
    /// Background service that tracks and logs news article statistics
    /// Runs every hour to monitor system health and article trends
    /// </summary>
    public class ArticleStatisticsService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ArticleStatisticsService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Run every hour

        public ArticleStatisticsService(
            IServiceProvider serviceProvider,
            ILogger<ArticleStatisticsService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Article Statistics Service is starting.");

            // Small initial delay to let the app start up
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CollectAndLogStatistics(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while collecting statistics: {Message}", ex.Message);
                }

                // Wait for the next interval
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CollectAndLogStatistics(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var newsService = scope.ServiceProvider.GetRequiredService<INewsArticleService>();
            var categoryService = scope.ServiceProvider.GetRequiredService<ICategoryService>();
            var accountService = scope.ServiceProvider.GetRequiredService<ISystemAccountService>();

            try
            {
                var articles = newsService.GetNewsArticles();
                var categories = categoryService.GetCategorys();
                var accounts = accountService.GetSystemAccounts();

                // Calculate statistics
                var totalArticles = articles.Count;
                var activeArticles = articles.Count(a => a.NewsStatus == true);
                var inactiveArticles = articles.Count(a => a.NewsStatus == false);

                var today = DateTime.Today;
                var articlesCreatedToday = articles.Count(a =>
                    a.CreatedDate.HasValue &&
                    a.CreatedDate.Value.Date == today);

                var articlesCreatedThisWeek = articles.Count(a =>
                    a.CreatedDate.HasValue &&
                    a.CreatedDate.Value >= today.AddDays(-7));

                var articlesCreatedThisMonth = articles.Count(a =>
                    a.CreatedDate.HasValue &&
                    a.CreatedDate.Value >= today.AddDays(-30));

                // Category distribution
                var categoryStats = articles
                    .Where(a => a.Category != null)
                    .GroupBy(a => a.Category.CategoryName)
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();

                // Author statistics
                var authorStats = articles
                    .Where(a => a.CreatedBy != null)
                    .GroupBy(a => a.CreatedBy.AccountName)
                    .Select(g => new { Author = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();

                // Log comprehensive statistics
                _logger.LogInformation("=== Article Statistics Report ===");
                _logger.LogInformation("Timestamp: {time}", DateTime.Now);
                _logger.LogInformation("Total Articles: {total}", totalArticles);
                _logger.LogInformation("Active Articles: {active} ({percentage:F1}%)",
                    activeArticles,
                    totalArticles > 0 ? (activeArticles * 100.0 / totalArticles) : 0);
                _logger.LogInformation("Inactive Articles: {inactive} ({percentage:F1}%)",
                    inactiveArticles,
                    totalArticles > 0 ? (inactiveArticles * 100.0 / totalArticles) : 0);
                _logger.LogInformation("Articles Created Today: {count}", articlesCreatedToday);
                _logger.LogInformation("Articles Created This Week: {count}", articlesCreatedThisWeek);
                _logger.LogInformation("Articles Created This Month: {count}", articlesCreatedThisMonth);
                _logger.LogInformation("Total Categories: {count}", categories.Count);
                _logger.LogInformation("Total Authors: {count}", accounts.Count);

                if (categoryStats.Any())
                {
                    _logger.LogInformation("Top 5 Categories:");
                    foreach (var stat in categoryStats)
                    {
                        _logger.LogInformation("  - {category}: {count} articles", stat.Category, stat.Count);
                    }
                }

                if (authorStats.Any())
                {
                    _logger.LogInformation("Top 5 Authors:");
                    foreach (var stat in authorStats)
                    {
                        _logger.LogInformation("  - {author}: {count} articles", stat.Author, stat.Count);
                    }
                }

                // Check for potential issues
                await CheckForIssues(articles, activeArticles, articlesCreatedThisWeek);

                _logger.LogInformation("=== End of Statistics Report ===");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to collect statistics: {message}", ex.Message);
                throw;
            }
        }

        private async Task CheckForIssues(
            List<BussinessObject.NewsArticle> articles,
            int activeArticles,
            int articlesCreatedThisWeek)
        {
            // Check for low content
            if (activeArticles < 10)
            {
                _logger.LogWarning("Low active article count: Only {count} active articles", activeArticles);
            }

            // Check for no recent activity
            if (articlesCreatedThisWeek == 0)
            {
                _logger.LogWarning("No articles created this week - consider content creation!");
            }

            // Check for articles without categories
            var uncategorizedCount = articles.Count(a => a.CategoryId == null);
            if (uncategorizedCount > 0)
            {
                _logger.LogWarning("{count} articles without categories", uncategorizedCount);
            }

            // Check for old articles that might need updating
            var staleThreshold = DateTime.Now.AddMonths(-3);
            var staleArticles = articles.Count(a =>
                a.NewsStatus == true &&
                a.ModifiedDate.HasValue &&
                a.ModifiedDate.Value < staleThreshold);

            if (staleArticles > 0)
            {
                _logger.LogInformation("{count} active articles haven't been updated in 3+ months", staleArticles);
            }

            await Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Article Statistics Service is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
}