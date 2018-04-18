using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service;
using Model;

namespace BookManagementAPI.Controllers
{
    public class CategoryController : ApiController
    {
        public ICategoryServices _categoryservice;

        public CategoryController(ICategoryServices categoryservice)
        {
            _categoryservice = categoryservice;
        }

        // GET: api/Category
        public HttpResponseMessage Get()
        {
            var categorys = _categoryservice.GetAllCategory();
            if (categorys != null)
            {
                var categoryEntity = categorys as List<Category> ?? categorys.ToList();
                if (categoryEntity.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, categoryEntity);
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Categorys not found");
        }

        // GET: api/Category/5
        public HttpResponseMessage Get(int id)
        {
            var category = _categoryservice.GetCategoryById(id);
            if (category != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, category);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, "Category not found");
        }

        // POST: api/Category
        public int Post([FromBody]Category category)
        {
            return _categoryservice.CreateCategory(category);
        }

        // PUT: api/Category/5
        public bool Put(int id, [FromBody]Category category)
        {
            if (id > 0)
            {
                return _categoryservice.UpdateCategory(id, category);
            }
            return false;
        }

        // DELETE: api/Category/5
        public bool Delete(int id)
        {
            if (id > 0)
            {
                return _categoryservice.DeleteCategory(id);
            }
            return false;
        }
    }
}
