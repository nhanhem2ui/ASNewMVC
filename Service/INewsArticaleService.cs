using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface INewsArticaleService
    {
        void SaveNewsArticle(NewsArticle p);
        void DeleteNewsArticle(string id);
        void UpdateNewsArticle(NewsArticle p);
        List<NewsArticle> GetNewsArticles();
        NewsArticle GetNewsArticleById(string id);
        List<NewsArticle> SearchNewsArticlesByHeadline(string searchHeadline);
    }
}
