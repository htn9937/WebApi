using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service;
using Model;
using Newtonsoft.Json;

namespace BookManagementAPI.Controllers
{
    public class AuthorController : ApiController
    {
        private IAuthorServices _authorservices;

        public AuthorController(IAuthorServices authorsevices)
        {
            _authorservices = authorsevices;
        }

        // GET: api/Author
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(_authorservices.GetAllAuthors());
        }

        // GET: api/Author/5
        public HttpResponseMessage Get(int id)
        {
            return Request.CreateResponse(_authorservices.GetAuthorById(id));
        }

        // POST: api/Author
        public int Post([FromBody]Author author)
        {
            return _authorservices.CreateAuthor(author);
        }

        // PUT: api/Author/5
        public bool Put(int id, [FromBody]Author author)
        {
            if (id > 0)
            {
                return _authorservices.UpdateAuthor(id, author);
            }
            return false;
        }

        // DELETE: api/Author/5
        public bool Delete(int id)
        {
            if (id > 0)
            {
                return _authorservices.DeleteAuthor(id);
            }
            return false;
        }
    }
}
