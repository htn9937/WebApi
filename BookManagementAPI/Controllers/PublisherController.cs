using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service;
using Model;
using log4net;
using Data.Entities;

namespace BookManagementAPI.Controllers
{
    public class PublisherController : ApiController
    {
        public IPublisherServices _publisherservice;

        public PublisherController(IPublisherServices pulisherservice)
        {
            _publisherservice = pulisherservice;
        }
        private static readonly ILog _logger = LogManager.GetLogger(typeof(PublisherController));
        // GET: api/Publisher
        public HttpResponseMessage Get()
        {
            //  return Request.CreateResponse(HttpStatusCode.OK,_publisherservice.GetAllPublisher());
            var publishers = _publisherservice.GetAllPublisher();
            if (publishers != null)
            {
                var publishersEntity = publishers as List<Publisher> ?? publishers.ToList();
                if (publishersEntity.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, publishersEntity);
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Publishers not found");
        }   

        // GET: api/Publisher/5
        public HttpResponseMessage Get(int id)
        {
            try
            {
                return Request.CreateResponse(_publisherservice.GetPublisherById(id));
            }
            catch (ApiDataException e)
            {
                _logger.Error(e);
                return Request.CreateResponse(e.Message);
            }
        }

        // POST: api/Publisher
        public int Post([FromBody]Publisher publisher)
        {
            return _publisherservice.CreatePublisher(publisher);
        }

        // PUT: api/Publisher/5
        public bool Put(int id, [FromBody]Publisher publisher)
        {
            if (id > 0)
            {
                return _publisherservice.UpdatePublisher(id, publisher);
            }
            return false;
        }

        // DELETE: api/Publisher/5
        public bool Delete(int id)
        {
            if (id > 0)
            {
                return _publisherservice.DeletePublisher(id);
            }
            return false;
        }
    }
}
