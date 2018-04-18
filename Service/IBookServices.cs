using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IBookServices
    {
        Book GetBookById(int BookId);

        IEnumerable<Book> GetAllBooks();

        int CreateBook(Book book);

        bool UpdateBook(int BookId, Book book);

        bool DeleteBook(int BookId);
    }
}
