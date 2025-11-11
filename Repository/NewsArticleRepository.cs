using BussinessObject;
using DataAccess;

namespace Repository
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        private readonly NewsArticleDAO _dao;

        public NewsArticleRepository(NewsArticleDAO dao)
        {
            _dao = dao;
        }

        public void DeleteNewsArticle(NewsArticle p) => _dao.DeleteNewsArticle(p);
        public NewsArticle GetNewsArticleById(string id) => _dao.GetNewsArticleById(id);
        public List<NewsArticle> GetNewsArticles() => _dao.GetNewsArticles();
        public void SaveNewsArticle(NewsArticle p) => _dao.SaveNewsArticle(p);
        public List<NewsArticle> SearchNewsArticlesByHeadline(string searchHeadline) => _dao.SearchNewsArticlesByHeadline(searchHeadline);
        public void UpdateNewsArticle(NewsArticle p) => _dao.UpdateNewsArticle(p);
    }
}