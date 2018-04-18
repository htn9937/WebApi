using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Data;
using System.Transactions;
using Data.Entities;
using Common;

namespace Service
{
    public class PublisherServices : IPublisherServices
    {
        private IUnitOfWork _unitofwork;

        public PublisherServices(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public int CreatePublisher(Publisher publisher)
        {
            using (var scope = new TransactionScope())
            {
                _unitofwork.PublisherRepository.Insert(publisher);
                _unitofwork.Save();
                scope.Complete();
                return publisher.Id;
            }
        }

        public bool DeletePublisher(int PublisherId)
        {
            var success = false;

            if (PublisherId > 0)
            {
                //Book books = _unitofwork.BookRepository.Get((x) => x.Publisher_Id == PublisherId);
                //if (books != null)
                //    return false;
                using (var scope = new TransactionScope())
                {
                    var publisher = _unitofwork.PublisherRepository.GetById(PublisherId);
                    if (publisher != null)
                    {
                        //if (publisher.Books.Count != 0)
                        //    throw new IntendForUserException(Constants.HAD_REF);
                        try
                        {
                            _unitofwork.PublisherRepository.Delete(publisher);
                            _unitofwork.Save();
                            scope.Complete();
                            success = true;
                        }
                        catch (Exception e) { throw e; }
                    }
                }
            }
            return success;
        }

        public IEnumerable<Publisher> GetAllPublisher()
        {
            var publishers = _unitofwork.PublisherRepository.GetAll().ToList();
            if (publishers.Any())
            {
                return publishers;
            }
            return null;
        }

        public Publisher GetPublisherById(int PublisherId)
        {
            var punlisher = _unitofwork.PublisherRepository.GetById(PublisherId);
            if (punlisher != null)
            {
                return punlisher;
            }
            throw new IntendForUserException("No publisher ID Found");
        }

        public bool UpdatePublisher(int PublisherId, Publisher publisher)
        {
            var success = false;
            if (publisher != null)
            {
                using (var scope = new TransactionScope())
                {
                    var pub = _unitofwork.PublisherRepository.GetById(PublisherId);
                    if (pub != null)
                    {
                        pub.Publisher_Address = publisher.Publisher_Address;
                        pub.Publisher_Name = publisher.Publisher_Name;
                        _unitofwork.PublisherRepository.Update(pub);
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
