using Model;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BookManagementAPI.Controllers
{
    public class BookController : ApiController
    {
        private IBookServices _bookService;

        public BookController(IBookServices bookService)
        {
            _bookService = bookService;
        }

        // GET: api/Book
        [Route("api/book")]
        public HttpResponseMessage Get()
        {
            var books = _bookService.GetAllBooks();
            if (books != null)
            {
                var booksEntity = books as List<Book> ?? books.ToList();
                if (booksEntity.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, booksEntity);
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Books not found");
        }

        // GET: api/Book/5
        public HttpResponseMessage Get(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, book);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, "Book not found");
        }

        // POST: api/Book
        public int Post([FromBody]Book book)
        {
            return _bookService.CreateBook(book);
        }

        // PUT: api/Book/5
        public bool Put(int id, [FromBody]Book book)
        {
            if (id > 0)
            {
                return _bookService.UpdateBook(id, book);
            }
            return false;
        }

        // DELETE: api/Book/5
        public bool Delete(int id)
        {
            if (id > 0)
            {
                return _bookService.DeleteBook(id);
            }
            return false;
        }
    }
}
