using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ICategoryRepository
    {
        void SaveCategory(Category p);
        void DeleteCategory(Category p);
        void UpdateCategory(Category p);
        List<Category> GetCategorys();
        Category GetCategoryById(short id);
        List<Category> SearchCategory(string search);
    }
}
