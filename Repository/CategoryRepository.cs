using BussinessObject;
using DataAccess;

namespace Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryDAO _dao;

        public CategoryRepository(CategoryDAO dao)
        {
            _dao = dao;
        }

        public void DeleteCategory(Category p) => _dao.DeleteCategory(p);
        public Category GetCategoryById(short id) => _dao.GetCategoryById(id);
        public List<Category> SearchCategory(string search) => _dao.SearchCategory(search);
        public List<Category> GetCategorys() => _dao.GetCategorys();
        public void SaveCategory(Category p) => _dao.SaveCategory(p);
        public void UpdateCategory(Category p) => _dao.UpdateCategory(p);
    }
}