using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IAuthorServices
    {
        Author GetAuthorById(int AuthorId);

        IEnumerable<Author> GetAllAuthors();

        int CreateAuthor(Author author);

        bool UpdateAuthor(int AuthorId, Author author);

        bool DeleteAuthor(int AuthorId);
    }
}
