using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ICategoryServices
    {
        Category GetCategoryById(int CategoryId);

        IEnumerable<Category> GetAllCategory();

        int CreateCategory(Category category);

        bool UpdateCategory(int CategoryId, Category category);

        bool DeleteCategory(int CategoryId);
    }
}
