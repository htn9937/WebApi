using Data;
using Model;
using Moq;
using NUnit.Framework;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Tests
{
    class BookServicesTest
    {
        private IBookServices _bookservices;
        private IUnitOfWork _unitofwork;
        private List<Book> _Books;
        private GenericRepository<Book> _BookRepository;
        private BookManagementEntities _dbEntities;

        [TestFixtureSetUp]
        public void Setup()
        {
            _Books = SetUpBooks();
        }

        private static List<Book> SetUpBooks()
        {
            var BookId = new int();
            var Books = DataInitializer.GetAllBooks();
            foreach (var pub in Books)
            {
                pub.Id = ++BookId;
            }
            return Books;
        }

        [TestFixtureTearDown]
        public void DisposeAllObjects()
        {
            _Books = null;
        }

        [SetUp]
        public void ReInitializeTest()
        {
            _Books = SetUpBooks();
            _dbEntities = new Mock<BookManagementEntities>().Object;
            _BookRepository = SetUpBookRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.BookRepository).Returns(_BookRepository);
            _unitofwork = unitOfWork.Object;
            _bookservices = new BookServices(_unitofwork);
        }

        [TearDown]
        public void DisposeTest()
        {
            _bookservices = null;
            _unitofwork = null;
            _BookRepository = null;
            if (_dbEntities != null)
                _dbEntities.Dispose();
        }

        private GenericRepository<Book> SetUpBookRepository()
        {
            var mockRepo = new Mock<GenericRepository<Book>>(MockBehavior.Default, _dbEntities);
            mockRepo.Setup(p => p.GetAll()).Returns(_Books);
            mockRepo.Setup(p => p.GetById(It.IsAny<int>())).Returns(new Func<int, Book>(id => _Books.Find(p => p.Id.Equals(id))));
            mockRepo.Setup(p => p.Insert((It.IsAny<Book>())))
                .Callback(new Action<Book>(newBook =>
                {
                    dynamic maxBookId = _Books.Last().Id;
                    dynamic nextBookId = maxBookId + 1;
                    newBook.Id = nextBookId;
                    _Books.Add(newBook);
                }));
            mockRepo.Setup(p => p.Update(It.IsAny<Book>()))
                .Callback(new Action<Book>(pub =>
                {
                    var oldBook = _Books.Find(a => a.Id == pub.Id);
                    oldBook = pub;
                }));
            mockRepo.Setup(p => p.Delete(It.IsAny<Book>()))
                .Callback(new Action<Book>(pub => {
                    var BookToRemove = _Books.Find(a => a.Id == pub.Id);
                    if (BookToRemove != null)
                        _Books.Remove(BookToRemove);
                }));

            return mockRepo.Object;
        }

        public IEnumerable<Book> GetAllBooks()
        {
            var Books = _unitofwork.BookRepository.GetAll().ToList();
            if (Books.Any())
            {
                return Books;
            }
            return null;
        }

        [Test]
        public void GetAllBookTest()
        {
            var Book = _bookservices.GetAllBooks();
            var BookList =
            Book.Select(
            BookEntity =>
            new Book {
                Id = BookEntity.Id,
                Book_Name = BookEntity.Book_Name,
                Amount = BookEntity.Amount,
                Price = BookEntity.Price,
                Publisher_Id = BookEntity.Publisher_Id,
                Author_Id = BookEntity.Author_Id,
                Category_Id = BookEntity.Category_Id,
                Category = BookEntity.Category,
                Publisher = BookEntity.Publisher,
                Author = BookEntity.Author
            }).ToList();

            var comparer = new BookComparer();
            CollectionAssert.AreEqual(
            BookList,
            _Books, comparer);
        }

        [Test]
        public void GetAllBookTestForNull()
        {
            _Books.Clear();
            var products = _bookservices.GetAllBooks();
            Assert.Null(products);
            SetUpBooks();
        }

        [Test]
        public void GetBookByRightIdTest()
        {
            var pub = _bookservices.GetBookById(2);
            if (pub != null)
            {
                AssertObjects.PropertyValuesAreEquals(pub,
                                                      _Books.Find(a => a.Book_Name.Contains("Harry Potter")));
            }
        }

        [Test]
        public void GetBookWrongIdTest()
        {
            var product = _bookservices.GetBookById(999);
            Assert.Null(product);
        }

        [Test]
        public void AddNewBookTest()
        {
            var newBook = new Book()
            {
                Book_Name = "Nhung Ke Mong Mo",
                Amount = 14,
                Price = 50000
            };

            var maxBookIDBeforeAdd = _Books.Max(a => a.Id);
            newBook.Id = maxBookIDBeforeAdd + 1;
            _bookservices.CreateBook(newBook);
            var addedBook = new Book()
            {
                Id = newBook.Id,
                Book_Name = newBook.Book_Name,
                Amount = newBook.Amount,
                Price = newBook.Price,
                Publisher_Id = newBook.Publisher_Id,
                Category_Id = newBook.Category_Id,
                Author_Id = newBook.Author_Id
            };
            AssertObjects.PropertyValuesAreEquals(addedBook, _Books.Last());
            Assert.That(maxBookIDBeforeAdd + 1, Is.EqualTo(_Books.Last().Id));
        }

        [Test]
        public void UpdateBookTest()
        {
            var firstBook = _Books.First();
            firstBook.Book_Name = "The Hobbit";
            var updatedBook = new Book()
            {
                Amount = firstBook.Amount,
                Book_Name = firstBook.Book_Name,
                Price = firstBook.Price,
                Id = firstBook.Id,
                Publisher_Id = firstBook.Publisher_Id,
                Author_Id = firstBook.Author_Id,
                Category_Id= firstBook.Category_Id,
            };
            _bookservices.UpdateBook(firstBook.Id, updatedBook);
            Assert.That(firstBook.Id, Is.EqualTo(1)); // hasn't changed
            Assert.That(firstBook.Book_Name, Is.EqualTo("The Hobbit")); // changed
            Assert.That(firstBook.Amount, Is.EqualTo(2)); // hasn't changed
        }

        [Test]
        public void DeleteBookTest()
        {
            int maxID = _Books.Max(a => a.Id); // Before removal
            var last = _Books.Last();

            // Remove last Book
            _bookservices.DeleteBook(last.Id);
            Assert.That(maxID, Is.GreaterThan(_Books.Max(a => a.Id)));   // Max id reduced by 1
        }
    }
}
