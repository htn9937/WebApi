using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IPublisherServices
    {
        Publisher GetPublisherById(int PublisherId);

        IEnumerable<Publisher> GetAllPublisher();

        int CreatePublisher(Publisher publisher);

        bool UpdatePublisher(int PublisherId, Publisher publisher);

        bool DeletePublisher(int PublisherId);
    }
}
