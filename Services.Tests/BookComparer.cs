using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Tests
{
    public class BookComparer : IComparer, IComparer<Book>
    {
        public int Compare(Book x, Book y)
        {
            if (x.Author == y.Author && x.Amount == y.Amount && x.Author_Id == y.Author_Id && x.Category_Id == y.Category_Id && x.Book_Name == y.Book_Name && x.Publisher_Id == y.Publisher_Id)
                return 0;
            return 1;
        }

        public int Compare(object x, object y)
        {
            var lhs = x as Book;
            var rhs = y as Book;
            if (lhs == null || rhs == null) throw new InvalidOperationException();
            return Compare(lhs, rhs);
        }
    }
}
