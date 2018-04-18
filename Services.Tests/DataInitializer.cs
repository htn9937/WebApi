using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Tests
{
    public class DataInitializer
    {
        public static List<Book> GetAllBooks()
        {
            var books = new List<Book>
            {
                new Book()
                {
                    Id = 1,
                    Book_Name = "Lord of The Rings",
                    Amount = 2,
                    Price = 40000,
                },
                new Book()
                {
                    Id = 2,
                    Book_Name = "Harry Potter",
                    Amount = 1,
                    Price = 40000,
                },
                new Book()
                {
                    Id = 3,
                    Book_Name = "The Paradox of Choice",
                    Amount = 8,
                    Price = 40000,
                },
                new Book()
                {
                    Id = 4,
                    Book_Name = "Toi Thay Hoa Vang Tren Co Xanh",
                    Amount = 5,
                    Price = 40000,
                }
            };
            return books;
        }

        public static List<Author> GetAllAuthors()
        {
            var authors = new List<Author>
            {
                new Author()
                {
                    Id = 1,
                    Author_Name = "Nguyen Nhat Anh",
                    Author_Adress = "TP. HCM",
                    Author_Email = "anhnguyenhcm@gmail.com",
                    Author_Phone = "01679435456"
                },
                new Author()
                {
                    Id = 1,
                    Author_Name = "David Webb",
                    Author_Adress = "San Fransico",
                    Author_Email = "david_sanfransico@gmail.com",
                    Author_Phone = "01679321908"
                },
                new Author()
                {
                    Id = 1,
                    Author_Name = "John Seagal",
                    Author_Adress = "Saint Petersburg",
                    Author_Email = "Kingdomtheten@gmail.com",
                    Author_Phone = "0935013644"
                },
                new Author()
                {
                    Id = 1,
                    Author_Name = "King Landster",
                    Author_Adress = "Gotham City",
                    Author_Email = "Thebat321@glive.com",
                    Author_Phone = "0167465120"
                }
            };
            return authors;
        }

        public static List<Category> GetAllCategories()
        {
            var categories = new List<Category>
            {
                new Category()
                {
                    Id = 1,
                    Category_Name = "Tam Ly"
                },
                new Category()
                {
                    Id = 2,
                    Category_Name = "Thieu Nhi"
                },
                new Category()
                {
                    Id = 3,
                    Category_Name = "Phieu Luu Mao Hiem"
                }
            };
            return categories;
        }
        public static List<Publisher> GetAllPublishers()
        {
            var publishers = new List<Publisher>
            {
                new Publisher()
                {
                //    Id = 1,
                    Publisher_Address = "TP. HCM",
                    Publisher_Name = "Nha Xuat Ban Kim Dong"
                },
                new Publisher()
                {
                //    Id = 2,
                    Publisher_Address = "Ha Noi",
                    Publisher_Name = "Nha Xuat Ban Tre"
                }, 
                new Publisher()
                {
                 //   Id = 3,
                    Publisher_Name = "Nha Xuat Ban Thieu Nhi",
                    Publisher_Address = "Da Nang" 
                }
            };
            return publishers;
        }
    }
}
