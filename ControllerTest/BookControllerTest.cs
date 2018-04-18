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
    class BookControllerTest
    {
        private IBookServices _bookService;
        private IUnitOfWork _unitOfWork;
        private List<Book> _Books;
        private GenericRepository<Book> _BookRepository;
        private HttpClient _client;
        private HttpResponseMessage _response;
        private BookManagementEntities _dbEntities;
        private const string ServiceBaseURL = "http://localhost:55494/";

        [TestFixtureSetUp]
        public void Setup()
        {
            _Books = SetUpBook();
            _dbEntities = new Mock<BookManagementEntities>().Object;
            _BookRepository = SetUpBookRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.BookRepository).Returns(_BookRepository);
            _unitOfWork = unitOfWork.Object;
            _bookService = new BookServices(_unitOfWork);
            _client = new HttpClient { BaseAddress = new Uri(ServiceBaseURL) };
        }

        private static List<Book> SetUpBook()
        {
            var bokId = new int();
            var Books = DataInitializer.GetAllBooks();
            foreach (Book bok in Books)
                bok.Id = ++bokId;
            return Books;
        }

        [TestFixtureTearDown]
        public void DisposeAllObjects()
        {
            _bookService = null;
            _unitOfWork = null;
            _BookRepository = null;
            _Books = null;
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

        private GenericRepository<Book> SetUpBookRepository()
        {
            // Initialise repository
            var mockRepo = new Mock<GenericRepository<Book>>(MockBehavior.Default, _dbEntities);

            // Setup mocking behavior
            mockRepo.Setup(p => p.GetAll()).Returns(_Books);

            mockRepo.Setup(p => p.GetById(It.IsAny<int>()))
            .Returns(new Func<int, Book>(
            id => _Books.Find(p => p.Id.Equals(id))));

            mockRepo.Setup(p => p.Insert((It.IsAny<Book>())))
            .Callback(new Action<Book>(newBook =>
            {
                dynamic maxBookID = _Books.Last().Id;
                dynamic nextBookID = maxBookID + 1;
                newBook.Id = nextBookID;
                _Books.Add(newBook);
            }));

            mockRepo.Setup(p => p.Update(It.IsAny<Book>()))
            .Callback(new Action<Book>(bok =>
            {
                var oldBook = _Books.Find(a => a.Id == bok.Id);
                oldBook = bok;
            }));

            mockRepo.Setup(p => p.Delete(It.IsAny<Book>()))
            .Callback(new Action<Book>(bok =>
            {
                var BookToRemove =
        _Books.Find(a => a.Id == bok.Id);

                if (BookToRemove != null)
                    _Books.Remove(BookToRemove);
            }));

            // Return mock implementation object
            return mockRepo.Object;
        }

        [Test]
        public void GetAllBookTest()
        {
            var BookController = new BookController(_bookService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseURL + "api/Book")
                }
            };
            BookController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            _response = BookController.Get();

            var responseResult = JsonConvert.DeserializeObject<List<Book>>(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            Assert.AreEqual(responseResult.Any(), true);
            var comparer = new BookComparer();
            CollectionAssert.AreEqual(
            responseResult.OrderBy(bok => bok, comparer),
            _Books.OrderBy(bok => bok, comparer), comparer);
        }

        [Test]
        public void GetBookByIdTest()
        {
            var BookController = new BookController(_bookService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseURL + "api/Book/2")
                }
            };
            BookController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            _response = BookController.Get(2);

            var responseResult = JsonConvert.DeserializeObject<Book>(_response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(_response.StatusCode, HttpStatusCode.OK);
            var obj = _Books.Find(ab => ab.Id == 2);
            ////AssertObjects.PropertyValuesAreEquals(responseResult,
            ////_Books.Find(a => a.Id==2));
            Assert.AreEqual(responseResult.Amount, obj.Amount);
            Assert.AreEqual(responseResult.Book_Name, obj.Book_Name);
            Assert.AreEqual(responseResult.Price, obj.Price);
            Assert.AreEqual(responseResult.Publisher_Id, obj.Publisher_Id);
            Assert.AreEqual(responseResult.Category_Id, obj.Category_Id);
            Assert.AreEqual(responseResult.Author_Id, obj.Author_Id);
        }

        [Test]
        //[ExpectedException("WebApi.ErrorHelper.ApiDataException")]
        public void GetBookByWrongIdTest()
        {
            var BookController = new BookController(_bookService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(ServiceBaseURL + "/api/Book/12")
                }
            };
            BookController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            _response = BookController.Get(12);
            //          var a =_response.StatusCode;
            Assert.AreEqual(_response.StatusCode, System.Net.HttpStatusCode.NotFound);

        }

        [Test]
        public void CreateBookTest()
        {
            var BookController = new BookController(_bookService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(ServiceBaseURL + "api/Book/")
                }
            };
            BookController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            var newBook = new Book()
            {
                Book_Name = "Nhung Nguoi Khon Kho",
                Price = 40000
            };

            var maxBookIDBeforeAdd = _Books.Max(a => a.Id);
            newBook.Id = maxBookIDBeforeAdd + 1;
            BookController.Post(newBook);
            var addedBook = new Book()
            {
                Book_Name = newBook.Book_Name,
                Id = newBook.Id,
                Price = newBook.Price,
                Amount = newBook.Amount,
                Category_Id=newBook.Category_Id,
                Publisher_Id=newBook.Publisher_Id,
                Author_Id=newBook.Author_Id
            };
            AssertObjects.PropertyValuesAreEquals(addedBook, _Books.Last());
            Assert.AreEqual(maxBookIDBeforeAdd + 1, (_Books.Last().Id));
        }

        [Test]
        public void DeleteBookTest()
        {
            var BookController = new BookController(_bookService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(ServiceBaseURL + "")
                }
            };
            BookController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

            int maxID = _Books.Max(a => a.Id); // Before removal
            var lastBook = _Books.Last();

            // Remove last Product
            BookController.Delete(lastBook.Id);
            Assert.That(maxID, Is.GreaterThan(_Books.Max(a => a.Id))); // Max id reduced by 1
        }

        [Test]
        public void DeleteBookWrongIdTest()
        {
            var BookController = new BookController(_bookService)
            {
                Request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(ServiceBaseURL + "")
                }
            };
            BookController.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            var before = _Books;
            int maxID = _Books.Max(a => a.Id); // Before removal
            var result = BookController.Delete(9999); // Call Delete
            Assert.AreEqual(result, false);
            Assert.AreEqual(before, _Books);
        }
    }
}
