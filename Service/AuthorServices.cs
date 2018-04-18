using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Data;
using System.Transactions;
using Data.Entities;
using System.Net;

namespace Service
{
    public class AuthorServices : IAuthorServices
    {
        private IUnitOfWork _unitofwork;

        public AuthorServices(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public int CreateAuthor(Author author)
        {
            using (var scope = new TransactionScope())
            {
                _unitofwork.AuthorRepository.Insert(author);
                _unitofwork.Save();
                scope.Complete();
                return author.Id;
            }
        }

        public bool DeleteAuthor(int AuthorId)
        {
            var success = false;
            if (AuthorId > 0)
            {
                //Book books = _unitofwork.BookRepository.Get((x)=>x.Author_Id==AuthorId);
                //if (books != null)
                //    return false;
                using (var scope = new TransactionScope())
                {
                    var author = _unitofwork.AuthorRepository.GetById(AuthorId);

                    if (author != null)
                    {
                        if (author.Books.Count != 0)
                            return false;
                        _unitofwork.AuthorRepository.Delete(author);
                        _unitofwork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        public IEnumerable<Author> GetAllAuthors()
        {
            var authors = _unitofwork.AuthorRepository.GetAll().ToList();
            if (authors.Any())
            {
                return authors;
            }
            throw new ApiDataException("Author not found", HttpStatusCode.NotFound);
        }

        public Author GetAuthorById(int AuthorId)
        {
            var author = _unitofwork.AuthorRepository.GetById(AuthorId);
            if (author == null)
            {
                throw new ApiDataException(string.Format("Can not find Author with ID = {0}", AuthorId), HttpStatusCode.NotFound);
            }
            return author;
        }

        public bool UpdateAuthor(int AuthorId, Author author)
        {
            var success = false;
            if (author != null)
            {
                using (var scope = new TransactionScope())
                {
                    var au = _unitofwork.AuthorRepository.GetById(AuthorId);
                    if (au != null)
                    {
                        au.Author_Name = author.Author_Name;
                        au.Author_Email = author.Author_Email;
                        au.Author_Adress = author.Author_Adress;
                        au.Author_Phone = author.Author_Phone;
                        _unitofwork.AuthorRepository.Update(au);
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
