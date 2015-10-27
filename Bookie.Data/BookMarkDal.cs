using Bookie.Common.Interfaces;
using Bookie.Common.Model;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.ChangeTracking;

namespace Bookie.Data
{
    public class BookMarkDal : IBookieData<BookMark>
    {
        public BookMark GetSingle(string query)
        {
            throw new NotImplementedException();
        }

        BookMark IBookieData<BookMark>.Add(BookMark item)
        {
            using (var context = new BookieContext())
            {
                if (Exists(item))
                {
                    return null;
                }
                var addedBookMark = context.BookMarks.Add(item);
                context.SaveChanges();
                return addedBookMark.Entity;
            }
        }

        public bool Exists(BookMark bookmark)
        {
            using (var context = new BookieContext())
            {
                return context.BookMarks.FirstOrDefault(x => x.PageNumber == bookmark.PageNumber && x.Book.Id == bookmark.Book.Id) != null;
            }
        }

        public void Remove(BookMark item)
        {
            using (var context = new BookieContext())
            {
             context.Attach(item);
                context.Entry(item).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public void Udpate(BookMark item)
        {
            using (var context = new BookieContext())
            {
                context.Attach(item);
                context.Entry(item).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public List<BookMark> GetAll()
        {
            using (var context = new BookieContext())
            {

                return context.BookMarks.Include(x => x.Book).ToList();
            }
        }

        public BookMark GetSingle(int id)
        {
            using (var context = new BookieContext())
            {
                return context.BookMarks.FirstOrDefault(x => x.Id == id);
            }
        }
    }
}