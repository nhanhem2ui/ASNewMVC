using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface INewsArticleRepository
    {
        void SaveNewsArticle(NewsArticle p);
        void DeleteNewsArticle(NewsArticle p);
        void UpdateNewsArticle(NewsArticle p);
        List<NewsArticle> GetNewsArticles();
        NewsArticle GetNewsArticleById(string id);
        List<NewsArticle> SearchNewsArticlesByHeadline(string searchHeadline);
    }
}
