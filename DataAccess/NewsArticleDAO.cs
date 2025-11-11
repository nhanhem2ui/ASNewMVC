using BussinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class NewsArticleDAO
    {
        public List<NewsArticle> GetNewsArticles()
        {
            var listNewsArticles = new List<NewsArticle>();
            try
            {
                using var db = new FunewsManagementContext();
                listNewsArticles = db.NewsArticles.Include(f => f.Category).Where(n => n.NewsStatus == true).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listNewsArticles;
        }

        public void SaveNewsArticle(NewsArticle p)
        {
            try
            {
                using var context = new FunewsManagementContext();

                // Handle tags if they exist
                if (p.Tags != null && p.Tags.Any())
                {
                    var tagIds = p.Tags.Select(t => t.TagId).ToList();
                    // Load existing tags from database
                    var existingTags = context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                    p.Tags = existingTags;
                }

                context.NewsArticles.Add(p);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void UpdateNewsArticle(NewsArticle p)
        {
            try
            {
                using var context = new FunewsManagementContext();

                // Get the existing article with its tags
                var existingArticle = context.NewsArticles
                    .Include(a => a.Tags)
                    .FirstOrDefault(a => a.NewsArticleId == p.NewsArticleId);

                if (existingArticle == null)
                    throw new Exception("Article not found");

                // Update article properties
                context.Entry(existingArticle).CurrentValues.SetValues(p);

                // Update tags
                existingArticle.Tags.Clear();
                if (p.Tags != null && p.Tags.Any())
                {
                    var tagIds = p.Tags.Select(t => t.TagId).ToList();
                    var existingTags = context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                    foreach (var tag in existingTags)
                    {
                        existingArticle.Tags.Add(tag);
                    }
                }

                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void DeleteNewsArticle(NewsArticle p)
        {
            try
            {
                using var context = new FunewsManagementContext();
                var p1 = context.NewsArticles.SingleOrDefault(c => c.NewsArticleId == p.NewsArticleId);
                context.NewsArticles.Remove(p1);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public NewsArticle GetNewsArticleById(string id)
        {
            using var db = new FunewsManagementContext();
            return db.NewsArticles
                .Include(a => a.Category)
                .Include(a => a.Tags)
                .Include(a => a.CreatedBy)
                .FirstOrDefault(c => c.NewsArticleId.Equals(id)) ?? new();
        }

        public List<NewsArticle> SearchNewsArticlesByHeadline(string searchHeadline)
        {
            var listNewsArticles = new List<NewsArticle>();
            try
            {
                using var db = new FunewsManagementContext();
                if (string.IsNullOrWhiteSpace(searchHeadline))
                {
                    listNewsArticles = db.NewsArticles
                        .Include(f => f.Category)
                        .Include(f => f.Tags)  // Add this line
                        .Where(n => n.NewsStatus == true)
                        .ToList();
                }
                else
                {
                    listNewsArticles = db.NewsArticles
                        .Include(f => f.Category)
                        .Include(f => f.Tags)  // Add this line
                        .Where(n => n.NewsStatus == true &&
                                    n.Headline.Contains(searchHeadline))
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listNewsArticles;
        }
    }
}
