using Bookie.Common.Model;
using Bookie.Domain.Interfaces;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookie.Data.Repositories
{
    public class BookMarkRepository : IBookMarkRepository
    {
        public ICollection<BookMark> Find(Func<BookMark, bool> @where)
        {
            using (var ctx = new Context())
            {
                return ctx.Bookmarks.Include(b => b.Book).Where(where).ToList();
            }
        }

        public BookMark Add(BookMark book)
        {
            using (var ctx = new Context())
            {
                var added = ctx.Bookmarks.Add(book);
                ctx.SaveChanges();
                return added.Entity;
            }
        }

        public ICollection<BookMark> GetAll()
        {
            using (var ctx = new Context())
            {
                return ctx.Bookmarks.Include(b => b.Book).ToList();
            }
        }

        public void Update(BookMark book)
        {
            using (var ctx = new Context())
            {
                ctx.Bookmarks.Attach(book);
                ctx.Bookmarks.Update(book);
                ctx.SaveChanges();
            }
        }

        public void Remove(BookMark bookmark)
        {
            using (var ctx = new Context())
            {

                    ctx.Bookmarks.Attach(bookmark);
                    ctx.Bookmarks.Remove(bookmark);
                    ctx.SaveChanges();


            }
        }
    }
}