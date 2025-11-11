using BussinessObject;
using Repository;

namespace Service
{
    public class NewsArticleService : INewsArticaleService
    {
        private readonly INewsArticleRepository iNewsArticleRepository;

        public NewsArticleService(INewsArticleRepository newsArticleRepository)
        {
            iNewsArticleRepository = newsArticleRepository;
        }

        public void DeleteNewsArticle(string id)
        {
            var p = iNewsArticleRepository.GetNewsArticleById(id);
            if (p != null)
            {
                iNewsArticleRepository.DeleteNewsArticle(p);
            }
        }

        public NewsArticle GetNewsArticleById(string id)
        {
            return iNewsArticleRepository.GetNewsArticleById(id);
        }

        public List<NewsArticle> GetNewsArticles()
        {
            return iNewsArticleRepository.GetNewsArticles();
        }

        public void SaveNewsArticle(NewsArticle p)
        {
            iNewsArticleRepository.SaveNewsArticle(p);
        }

        public List<NewsArticle> SearchNewsArticlesByHeadline(string searchHeadline)
        {
            return iNewsArticleRepository.SearchNewsArticlesByHeadline(searchHeadline);
        }

        public void UpdateNewsArticle(NewsArticle p)
        {
            iNewsArticleRepository.UpdateNewsArticle(p);
        }
    }
}
