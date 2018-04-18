using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Data;
using System.Transactions;

namespace Service
{
    public class CategoryServices : ICategoryServices
    {
        private IUnitOfWork _unitofwork;

        public CategoryServices(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public int CreateCategory(Category category)
        {
            using (var scope = new TransactionScope())
            {
                _unitofwork.CategoryRepository.Insert(category);
                _unitofwork.Save();
                scope.Complete();
                return category.Id;
            }
        }

        public bool DeleteCategory(int CategoryId)
        {
            var success = false;
            if (CategoryId > 0)
            {
                //Book books = _unitofwork.BookRepository.Get((x) => x.Category_Id == CategoryId);
                //if (books != null)
                //    return false;
                using (var scope = new TransactionScope())
                {
                    var category = _unitofwork.CategoryRepository.GetById(CategoryId);
                    if (category != null)
                    {
                        if (category.Books.Count != 0)
                            return false;
                        _unitofwork.CategoryRepository.Delete(category);
                        _unitofwork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        public IEnumerable<Category> GetAllCategory()
        {
            var categorys = _unitofwork.CategoryRepository.GetAll().ToList();
            if (categorys.Any())
            {
                return categorys;
            }
            return null;
        }

        public Category GetCategoryById(int CategoryId)
        {
            var category = _unitofwork.CategoryRepository.GetById(CategoryId);
            if (category != null)
            {
                return category;
            }
            return null;
        }

        public bool UpdateCategory(int CategoryId, Category category)
        {
            var success = false;
            if (category != null)
            {
                using (var scope = new TransactionScope())
                {
                    var cate = _unitofwork.CategoryRepository.GetById(CategoryId);
                    if (cate != null)
                    {
                        cate.Category_Name = category.Category_Name;
                        _unitofwork.CategoryRepository.Update(cate);
                        _unitofwork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }
    }
}
