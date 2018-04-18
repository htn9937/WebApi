using Data;
using Model;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;

namespace Services.Tests
{
    class PublisherServicesTest
    {
        private IPublisherServices _publisherservices;
        private IUnitOfWork _unitofwork;
        private List<Publisher> _publishers;
        private GenericRepository<Publisher> _publisherRepository;
        private BookManagementEntities _dbEntities;

        [TestFixtureSetUp]
        public void Setup()
        {
            _publishers = SetUpPublishers();
        }

        private static List<Publisher> SetUpPublishers()
        {
            var publisherId = new int();
            var publishers = DataInitializer.GetAllPublishers();
            foreach(var pub in publishers)
            {
                pub.Id = ++publisherId;
            }
            return publishers;
        }

        [TestFixtureTearDown]
        public void DisposeAllObjects()
        {
            _publishers = null;
        }

        [SetUp]
        public void ReInitializeTest()
        {
            _publishers = SetUpPublishers();
            _dbEntities = new Mock<BookManagementEntities>().Object;
            _publisherRepository = SetUpPublisherRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.PublisherRepository).Returns(_publisherRepository);
            _unitofwork = unitOfWork.Object;
            _publisherservices = new PublisherServices(_unitofwork);
        }

        [TearDown]
        public void DisposeTest()
        {
            _publisherservices = null;
            _unitofwork = null;
            _publisherRepository = null;
            if (_dbEntities != null)
                _dbEntities.Dispose();
        }

        private GenericRepository<Publisher> SetUpPublisherRepository()
        {
            var mockRepo = new Mock<GenericRepository<Publisher>>(MockBehavior.Default, _dbEntities);
            mockRepo.Setup(p => p.GetAll()).Returns(_publishers);
            mockRepo.Setup(p => p.GetById(It.IsAny<int>())).Returns(new Func<int, Publisher>(id => _publishers.Find(p => p.Id.Equals(id))));
            mockRepo.Setup(p => p.Insert((It.IsAny<Publisher>())))
                .Callback(new Action<Publisher>(newPublisher => 
                    {
                        dynamic maxPublisherId = _publishers.Last().Id;
                        dynamic nextPublisherId = maxPublisherId + 1;
                        newPublisher.Id = nextPublisherId;
                        _publishers.Add(newPublisher);
                    }));
            mockRepo.Setup(p => p.Update(It.IsAny<Publisher>()))
                .Callback(new Action<Publisher>(pub =>
            {
                var oldPublisher = _publishers.Find(a => a.Id == pub.Id);
                oldPublisher = pub;
            }));
            mockRepo.Setup(p => p.Delete(It.IsAny<Publisher>()))
                .Callback(new Action<Publisher>(pub => {
                    var publisherToRemove = _publishers.Find(a => a.Id == pub.Id);
                    if (publisherToRemove != null)
                        _publishers.Remove(publisherToRemove);
                }));

            return mockRepo.Object;
        }

        public IEnumerable<Publisher> GetAllPublishers()
        {
            var publishers = _unitofwork.PublisherRepository.GetAll().ToList();
            if (publishers.Any())
            {
                return publishers;
            }
            return null;
        }

        [Test]
        public void GetAllPublisherTest()
        {
            var publisher = _publisherservices.GetAllPublisher().ToList();
            var publisherList =
            publisher.Select(
            publisherEntity =>
            new Publisher { Id = publisherEntity.Id, Publisher_Name = publisherEntity.Publisher_Name, Publisher_Address = publisherEntity.Publisher_Address }).ToList();
            var comparer = new PublisherComparer();
            CollectionAssert.AreEqual(
            publisher.OrderBy(pub => pub, comparer),
            _publishers.OrderBy(pub => pub, comparer), comparer);
        }

        [Test]
        public void GetAllPublisherTestForNull()
        {
            _publishers.Clear();
            var products = _publisherservices.GetAllPublisher();
            Assert.Null(products);
            SetUpPublishers();
        }

        [Test]
        public void GetPubliserByRightIdTest()
        {
            var pub = _publisherservices.GetPublisherById(2);
            if (pub != null)
            {
                AssertObjects.PropertyValuesAreEquals(pub,
                                                      _publishers.Find(a => a.Publisher_Name.Contains("Nha Xuat Ban Tre")));
            }
        }

        [Test]
        public void GetPublisherWrongIdTest()
        {
            var product = _publisherservices.GetPublisherById(999);
            Assert.Null(product);
        }

        [Test]
        public void AddNewPublisherTest()
        {
            var newPublisher = new Publisher()
            {
                Publisher_Name = "Nhà Xuất Bản Giáo Dục",
                Publisher_Address = "Lam Dong",
            };

            var maxPublisherIDBeforeAdd = _publishers.Max(a => a.Id);
            newPublisher.Id = maxPublisherIDBeforeAdd + 1;
            _publisherservices.CreatePublisher(newPublisher);
            var addedpublisher = new Publisher() {
                Id = newPublisher.Id,
                Publisher_Name = newPublisher.Publisher_Name,
                Publisher_Address = newPublisher.Publisher_Address,
                Books = newPublisher.Books
            };
            AssertObjects.PropertyValuesAreEquals(addedpublisher, _publishers.Last());
            Assert.That(maxPublisherIDBeforeAdd + 1, Is.EqualTo(_publishers.Last().Id));
        }

        [Test]
        public void UpdatePublisherTest()
        {
            var firstPublisher = _publishers.First();
            firstPublisher.Publisher_Address = "Lao Cai";
            var updatedPublisher = new Publisher()
            {
                Publisher_Address = firstPublisher.Publisher_Address,
                Publisher_Name = firstPublisher.Publisher_Name,
                Books = firstPublisher.Books,
                Id = firstPublisher.Id
            };
            _publisherservices.UpdatePublisher(firstPublisher.Id, updatedPublisher);
            Assert.That(firstPublisher.Id, Is.EqualTo(1)); // hasn't changed
            Assert.That(firstPublisher.Publisher_Name, Is.EqualTo("Nha Xuat Ban Kim Dong")); // hasn't changed
            Assert.That(firstPublisher.Publisher_Address, Is.EqualTo("Lao Cai")); //  changed
        }

        [Test]
        public void DeletePublisherTest()
        {
            int maxID = _publishers.Max(a => a.Id); // Before removal
            var last = _publishers.Last();

            // Remove last Publisher
            _publisherservices.DeletePublisher(last.Id);
            Assert.That(maxID, Is.GreaterThan(_publishers.Max(a => a.Id)));   // Max id reduced by 1
        }
    }
}
