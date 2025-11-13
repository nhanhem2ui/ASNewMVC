using Service;

namespace FUNewsManagement.BackgroundServices
{
    /// <summary>
    /// Background service that automatically archives old inactive news articles
    /// Runs daily at 2 AM to maintain database performance
    /// </summary>
    public class NewsArticleArchiveService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NewsArticleArchiveService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Run once per day

        public NewsArticleArchiveService(
            IServiceProvider serviceProvider,
            ILogger<NewsArticleArchiveService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("News Article Archive Service is starting.");

            // Wait until 2 AM for first run
            await WaitUntil2AM(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("News Article Archive Service is running at: {time}", DateTimeOffset.Now);

                    ArchiveOldArticles(stoppingToken);

                    _logger.LogInformation("News Article Archive Service completed at: {time}", DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while archiving articles: {Message}", ex.Message);
                }

                // Wait 24 hours before next run
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task WaitUntil2AM(CancellationToken stoppingToken)
        {
            var now = DateTime.Now;
            var next2AM = DateTime.Today.AddHours(2);

            if (now > next2AM)
            {
                next2AM = next2AM.AddDays(1);
            }

            var delay = next2AM - now;
            _logger.LogInformation("Waiting until 2 AM. Next run at: {time}", next2AM);

            await Task.Delay(delay, stoppingToken);
        }

        private void ArchiveOldArticles(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var newsService = scope.ServiceProvider.GetRequiredService<INewsArticleService>();

            try
            {
                var allArticles = newsService.GetNewsArticles();
                var archiveThreshold = DateTime.Now.AddMonths(-6); // Archive articles older than 6 months

                var articlesToArchive = allArticles
                    .Where(a => a.NewsStatus == false && // Only inactive articles
                               a.CreatedDate.HasValue &&
                               a.CreatedDate.Value < archiveThreshold)
                    .ToList();

                if (articlesToArchive.Count == 0)
                {
                    _logger.LogInformation("No articles to archive.");
                    return;
                }

                _logger.LogInformation("Found {count} articles to archive", articlesToArchive.Count);

                var archivedCount = 0;
                var errorCount = 0;

                foreach (var article in articlesToArchive)
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogWarning("Archive operation cancelled.");
                        break;
                    }

                    try
                    {
                        // Update article to mark as archived (you could add an IsArchived field)
                        // For now, we'll update the ModifiedDate to indicate archival
                        article.ModifiedDate = DateTime.Now;
                        article.NewsContent = $"[ARCHIVED] {article.NewsContent}";

                        newsService.UpdateNewsArticle(article);
                        archivedCount++;

                        _logger.LogInformation(
                            "Archived article: {id} - {title}",
                            article.NewsArticleId,
                            article.NewsTitle);
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        _logger.LogError(
                            ex,
                            "Failed to archive article {id}: {message}",
                            article.NewsArticleId,
                            ex.Message);
                    }
                }

                _logger.LogInformation(
                    "Archive operation completed. Successfully archived: {archived}, Errors: {errors}",
                    archivedCount,
                    errorCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error during archive operation: {message}", ex.Message);
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("News Article Archive Service is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
}