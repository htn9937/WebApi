using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Model;

namespace Services.Tests
{
    public class PublisherComparer : IComparer, IComparer<Publisher>
    {
        public int Compare(Publisher x, Publisher y)
        {
            int temp;
            return (temp =  x.Publisher_Name.CompareTo(y.Publisher_Name)) !=0 ? temp : x.Publisher_Address.CompareTo(y.Publisher_Address);
        }

        public int Compare(object x, object y)
        {
            var lhs = x as Publisher;
            var rhs = y as Publisher;
            if (lhs == null || rhs == null) throw new InvalidOperationException();
            return Compare(lhs, rhs);
        }
    }
}
