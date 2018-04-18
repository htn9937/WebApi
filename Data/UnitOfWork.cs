using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private BookManagementEntities _context = null;
        private GenericRepository<Author> _authorRepository;
        private GenericRepository<Book> _bookRepository;
        private GenericRepository<Category> _categoryRepository;
        private GenericRepository<Publisher> _publisherRepository;

        public UnitOfWork()
        {
            _context = new BookManagementEntities();
        }

        public GenericRepository<Author> AuthorRepository
        {
            get
            {
                if (this._authorRepository == null)
                    this._authorRepository = new GenericRepository<Author>(_context);
                return _authorRepository;
            }
        }

        public GenericRepository<Book> BookRepository
        {
            get
            {
                if (this._bookRepository == null)
                    this._bookRepository = new GenericRepository<Book>(_context);
                return _bookRepository;
            }
        }

        public GenericRepository<Category> CategoryRepository
        {
            get
            {
                if (this._categoryRepository == null)
                    this._categoryRepository = new GenericRepository<Category>(_context);
                return _categoryRepository;
            }
        }

        public GenericRepository<Publisher> PublisherRepository
        {
            get
            {
                if (this._publisherRepository == null)
                    this._publisherRepository = new GenericRepository<Publisher>(_context);
                return _publisherRepository;
            }
        }

        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {

                var outputLines = new List<string>();
                foreach (var eve in e.EntityValidationErrors)
                {
                    outputLines.Add(string.Format("{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:", 
                        DateTime.Now, 
                        eve.Entry.Entity.GetType().Name, 
                        eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        outputLines.Add(string.Format("- Property: \"{0}\", Error: \"{1}\"", 
                            ve.PropertyName, 
                            ve.ErrorMessage));
                    }
                }
                System.IO.File.AppendAllLines(@"D:\errors.txt", outputLines);

                throw e;
            }
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing) {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Debug.WriteLine("Unit of Work is being disposed");
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

    }
}
