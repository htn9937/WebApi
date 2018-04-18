using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public interface IUnitOfWork
    {
        GenericRepository<Author> AuthorRepository { get; }
        GenericRepository<Book> BookRepository { get; }
        GenericRepository<Category> CategoryRepository { get; }
        GenericRepository<Publisher> PublisherRepository { get; }
        void Save();
    }
}
