using Bookie.Common.Model;
using Bookie.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;

namespace Bookie.Data
{
    public class BookRepository : IBookRepository
    {
      
        public ICollection<Book> Find(Func<Book, bool> where)
        {
            using (var ctx = new Context())
            {
                return ctx.Books.Where(where).ToList();
            }
        }

        public Book Add(Book book)
        {
            using (var ctx = new Context())
            {
                var added = ctx.Books.Add(book);
                ctx.SaveChanges();
                return added.Entity;
            }
        }

        public ICollection<Book> GetAll()
        {
            using (var ctx = new Context())
            {
                return ctx.Books.Include(r=> r.Cover).Include(x=> x.BookMarks).Include(t=> t.Source).ToList();
            }
        }

    }
}