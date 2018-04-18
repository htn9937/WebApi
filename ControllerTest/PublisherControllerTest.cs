using BookManagementAPI.Controllers;
using Data;
using Model;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Service;
using Services.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace ControllerTest
{
    class PublisherControllerTest
    {
        private IPublisherServices _publiserService;
        private IUnitOfWork _unitOfWork;
        private List<Publisher> _publishers;
        private GenericRepository<Publisher> _publisherRepository;
        private HttpClient _client;
        private HttpResponseMessage _response;
        private BookManagementEntities _dbEntities;
        //private string _token;
        private const string ServiceBaseURL = "http://localhost:55494/";

        [TestFixtureSetUp]
        public void Setup()
        {
            _publishers = SetUpPublisher();
            _dbEntities = new Mock<BookManagementEntities>().Object;
            _publisherRepository = SetUpPublisherRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.PublisherRepository).Returns(_publisherRepository);
            _unitOfWork = unitOfWork.Object;
            _publiserService = new PublisherServices(_unitOfWork);
            _client = new HttpClient { BaseAddress = new Uri(ServiceBaseURL) };
        }

        private static List<Publisher> SetUpPublisher()
        {
            var pubId = new int();
            var publishers = DataInitializer.GetAllPublishers();
            foreach (Publisher pub in publishers)
                pub.Id = ++pubId;
            return publishers;
        }

        [TestFixtureTearDown]
        public void DisposeAllObjects()
        {
            _publiserService = null;
            _unitOfWork = null;
            _publisherRepository = null;
            _publishers = null;
            if (_response != null)
                _response.Dispose();
            if (_client != null)
                _client.Dispose();
        }

        [SetUp]
        public void ReInitializeTest()
        {
            _client = new HttpClient { BaseAddress = new Uri(ServiceBaseURL) };
        }

        [TearDown]
        public void DisposeTest()
        {
            if (_response != null)
                _response.Dispose();
            if (_client != null)
                _client.Dispose();
        }

        private GenericRepository<Publisher> SetUpPublisherRepository()
        {
            // Initialise repository
            var mockRepo = new Mock<GenericRepository<Publisher>>(MockBehavior.Default, _dbEntities);

            // Setup mocking behavior
            mockRepo.Setup(p => p.GetAll()).Returns(_publishers);

            mockRepo.Setup(p => p.GetById(It.IsAny<int>()))
            .Returns(new Func<int, Publisher>(
            id => _publishers.Find(p => p.Id.Equals(id))));

            mockRepo.Setup(p => p.Insert((It.IsAny<Publisher>())))
            .Callback(new Action<Publisher>(newPublisher =>
            {
                dynamic maxPublisherID = _publishers.Last().Id;
                dynamic nextPublisherID = maxPublisherID + 1;
                newPublisher.Id = nextPublisherID;
                _publishers.Add(newPublisher);
            }));

            mockRepo.Setup(p => p.Update(It.IsAny<Publisher>()))
            .Callback(new Action<Publisher>(pub =>
            {
                var oldPublisher = _publishers.Find(a => a.Id == pub.Id);
                oldPublisher = pub;
            }));

            mockRepo.Setup(p => p.Delete(It.IsAny<Publisher>()))
            .Callback(new Action<Publisher>(pub =>
            {
                var publisherToRemove =
        _publishers.Find(a => a.Id == pub.Id);

                if (publisherToRemove != null)
                    _publishers.Remove(publisherToRemove);
            }));

            // Return mock implementation object
            return mockRepo.Object;
        }

        [Test]
        public void GetAllPublisherTest()
        {
            var publisherController = new PublisherController(_publiserService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseURL + "api/Publisher")
                }
            };
            publisherController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            _response = publisherController.Get();

            var responseResult = JsonConvert.DeserializeObject<List<Publisher>>(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(responseResult.Any(), true);
            var comparer = new PublisherComparer();
            CollectionAssert.AreEqual(
            responseResult.OrderBy(pub => pub, comparer),
            _publishers.OrderBy(pub => pub, comparer), comparer);
        }

        [Test]
        public void GetPublisherByIdTest()
        {
            var publisherController = new PublisherController(_publiserService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseURL + "api/Publisher/2")
                }
            };
            publisherController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            _response = publisherController.Get(2);

            var responseResult = JsonConvert.DeserializeObject<Publisher>(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            var obj = _publishers.Find(ab => ab.Id == 2);
            Assert.AreEqual(responseResult.Publisher_Address, obj.Publisher_Address);
            Assert.AreEqual(responseResult.Publisher_Name, obj.Publisher_Name);
        }

        [Test]
        //[ExpectedException("WebApi.ErrorHelper.ApiDataException")]
        public void GetPublisherByWrongIdTest()
        {
            var publisherController = new PublisherController(_publiserService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseURL + "/api/Publisher/12")
                }
            };
            publisherController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            _response = publisherController.Get(12);
  //          var a =_response.StatusCode;
            Assert.AreEqual(_response.StatusCode, System.Net.HttpStatusCode.NotFound);
            
        }

        [Test]
        public void CreatePublisherTest()
        {
            var publisherController = new PublisherController(_publiserService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(ServiceBaseURL + "api/Publisher/")
                }
            };
            publisherController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var newPublisher = new Publisher()
            {
                Publisher_Name = "Nha Xuat Ban ABC",
                Publisher_Address = "TP ABC"
            };

            var maxPublisherIDBeforeAdd = _publishers.Max(a => a.Id);
            newPublisher.Id = maxPublisherIDBeforeAdd + 1;
            publisherController.Post(newPublisher);
            var addedpublisher = new Publisher()
            {
                Publisher_Name = newPublisher.Publisher_Name,
                Id = newPublisher.Id,
                Publisher_Address = newPublisher.Publisher_Address,
                Books = newPublisher.Books
            };
            AssertObjects.PropertyValuesAreEquals(addedpublisher, _publishers.Last());
            Assert.AreEqual(maxPublisherIDBeforeAdd + 1, (_publishers.Last().Id));
        }

        [Test]
        public void DeletePublisherTest()
        {
            var publisherController = new PublisherController(_publiserService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(ServiceBaseURL + "")
                }
            };
            publisherController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            int maxID = _publishers.Max(a => a.Id); // Before removal
            var lastPublisher = _publishers.Last();

            // Remove last Product
            publisherController.Delete(lastPublisher.Id);
            Assert.That(maxID, Is.GreaterThan(_publishers.Max(a => a.Id))); // Max id reduced by 1
        }

        [Test]
        public void DeletePublisherWrongIdTest()
        {
            var publisherController = new PublisherController(_publiserService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(ServiceBaseURL + "")
                }
            };
            publisherController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var before = _publishers;
            int maxID = _publishers.Max(a => a.Id); // Before removal
            var result = publisherController.Delete(9999); // Call Delete
            Assert.AreEqual(result, false);
            Assert.AreEqual(before, _publishers);
        }
    }
}
