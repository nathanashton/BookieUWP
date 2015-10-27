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
    public class BookMarkRepository
    {

        private readonly IBookieData<BookMark> _data;

        public BookMarkRepository()
        {
            _data = new BookMarkDal();
        }

        public List<BookMark> GetAll()
        {
            return _data.GetAll();
        }

        public BookMark Add(BookMark bookmark)
        {
            return _data.Add(bookmark);
        }

        public void Update(BookMark bookmark)
        {
            _data.Udpate(bookmark);
        }

        public bool Exists(BookMark bookmark)
        {
            return _data.Exists(bookmark);
        }

        public void Remove(BookMark bookmark)
        {
            _data.Remove(bookmark);
        }


    }
}
