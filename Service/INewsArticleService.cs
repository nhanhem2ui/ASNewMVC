using BussinessObject;

namespace Service
{
    public interface INewsArticleService
    {
        void SaveNewsArticle(NewsArticle p);
        void DeleteNewsArticle(string id);
        void UpdateNewsArticle(NewsArticle p);
        List<NewsArticle> GetNewsArticles();
        NewsArticle GetNewsArticleById(string id);
        List<NewsArticle> SearchNewsArticlesByHeadline(string searchHeadline);
    }
}
