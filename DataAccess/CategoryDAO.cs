using BussinessObject;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class CategoryDAO
    {
        public List<Category> GetCategorys()
        {
            var listCategorys = new List<Category>();
            try
            {
                using var db = new FunewsManagementContext();
                listCategorys = db.Categories.ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listCategorys;
        }

        public void SaveCategory(Category p)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Categories.Add(p);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void UpdateCategory(Category p)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Entry<Category>(p).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void DeleteCategory(Category p)
        {
            try
            {
                using var context = new FunewsManagementContext();
                var p1 = context.Categories.SingleOrDefault(c => c.CategoryId == p.CategoryId);
                context.Categories.Remove(p1);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Category GetCategoryById(short id)
        {
            using var db = new FunewsManagementContext();
            return db.Categories.FirstOrDefault(c => c.CategoryId == id);
        }

        public List<Category> SearchCategory(string search)
        {
            var list = new List<Category>();
            try
            {
                using var db = new FunewsManagementContext();
                if (string.IsNullOrWhiteSpace(search))
                {
                    list = db.Categories.ToList();
                }
                else
                {
                    list = db.Categories
                            .Where(n => n.CategoryName.Contains(search))
                            .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list;
        }
    }
}
