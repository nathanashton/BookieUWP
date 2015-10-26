using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookie.Common.Interfaces;
using Bookie.Common.Model;
using Bookie.Data;

namespace Bookie.Repository
{
    public class BookRepository
    {

        private readonly IBookieData<Book> _data;

        public BookRepository()
        {
            _data = new BookDal();
        }

        public List<Book> GetAll()
        {
            return _data.GetAll();
        }

        public Book Add(Book book)
        {
            return _data.Add(book);
        }

        public void Update(Book book)
        {
            _data.Udpate(book);
        }

    }
}
