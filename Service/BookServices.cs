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
    public class BookServices : IBookServices
    {
        private IUnitOfWork _unitofwork;

        public BookServices(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        private bool checkForeign(Book bookEntity)
        {
            if (bookEntity.Author_Id != null)
            {
                if (_unitofwork.AuthorRepository.GetById(bookEntity.Author_Id) == null)
                    return false;
            }
            if (bookEntity.Publisher_Id != null)
            {
                if (_unitofwork.PublisherRepository.GetById(bookEntity.Publisher_Id) == null)
                    return false;
            }
            if (bookEntity.Category_Id != null)
            {
                if (_unitofwork.CategoryRepository.GetById(bookEntity.Category_Id) == null)
                    return false;
            }
            return true;
        }

        public int CreateBook(Book bookEntity)
        {
            using (var scope = new TransactionScope())
            {
                //if (bookEntity.Author_Id != null)
                //{
                //    if (_unitofwork.AuthorRepository.GetById(bookEntity.Author_Id) == null)
                //        return -1;
                //}
                //if (bookEntity.Publisher_Id != null)
                //{
                //    if (_unitofwork.PublisherRepository.GetById(bookEntity.Publisher_Id) == null)
                //        return -1;
                //}
                //if (bookEntity.Category_Id != null)
                //{
                //    if (_unitofwork.CategoryRepository.GetById(bookEntity.Category_Id) == null)
                //        return -1;
                //}
                if(!checkForeign(bookEntity))
                    return -1;
                _unitofwork.BookRepository.Insert(bookEntity);
                _unitofwork.Save();
                scope.Complete();
                return bookEntity.Id;
            }
        }

        public bool DeleteBook(int BookId)
        {
            var success = false;
            if (BookId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var book = _unitofwork.BookRepository.GetById(BookId);
                    if (book != null)
                    {
                        _unitofwork.BookRepository.Delete(book);
                        _unitofwork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        public IEnumerable<Book> GetAllBooks()
        {
            var books = _unitofwork.BookRepository.GetAll().ToList();
            if (books.Any())
            {
                return books;
            }
            return null;
        }

        public Book GetBookById(int BookId)
        {
            var book = _unitofwork.BookRepository.GetById(BookId);
            if (book != null)
            {
                return book;
            }
            return null;
        }

        public bool UpdateBook(int BookId, Book bookEntity)
        {
            var success = false;
            if (bookEntity != null)
            {
                using(var scope = new TransactionScope())
                {
                    var book = _unitofwork.BookRepository.GetById(BookId);
                    if (book != null)
                    {
                        book.Book_Name = bookEntity.Book_Name;
                        book.Amount = bookEntity.Amount;
                        book.Price = bookEntity.Price;
                        book.Author_Id = bookEntity.Author_Id;
                        book.Category_Id = bookEntity.Author_Id;
                        book.Publisher_Id = bookEntity.Publisher_Id;
                        if (!checkForeign(book))
                            return false;
                        _unitofwork.BookRepository.Update(book);
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
