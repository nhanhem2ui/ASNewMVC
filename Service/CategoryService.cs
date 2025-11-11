using BussinessObject;
using Repository;

namespace Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository iCategoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            iCategoryRepository = categoryRepository;
        }

        public void DeleteCategory(short id)
        {
            var category = iCategoryRepository.GetCategoryById(id);
            if (category != null)
            {
                iCategoryRepository.DeleteCategory(category);
            }
        }

        public Category GetCategoryById(short id)
        {
            return iCategoryRepository.GetCategoryById(id);
        }

        public List<Category> SearchCategory(string search)
        {
            return iCategoryRepository.SearchCategory(search);
        }

        public List<Category> GetCategorys()
        {
            return iCategoryRepository.GetCategorys();
        }

        public void SaveCategory(Category p)
        {
            iCategoryRepository.SaveCategory(p);
        }

        public void UpdateCategory(Category p)
        {
            iCategoryRepository.UpdateCategory(p);
        }
    }
}
