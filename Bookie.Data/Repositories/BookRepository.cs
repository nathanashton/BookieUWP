using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookie.Common.Model;
using Bookie.Domain.Interfaces;
using Microsoft.Data.Entity;

namespace Bookie.Data.Repositories
{
    public class BookRepository : IBookRepository
    {
        public  ICollection<Book> Find(Func<Book, bool> where)
        {
            using (var ctx = new Context())
            {
                return ctx.Books.Include(r => r.Cover).Include(x=> x.BookMarks).Include(y=> y.Source).Where(where).ToList();
            }
        }

        public async Task<Book> Add(Book book)
        {
            using (var ctx = new Context())
            {
                var added = ctx.Books.Add(book);
                await ctx.SaveChangesAsync();
                return added.Entity;
            }
        }

        public async Task<List<Book>> GetAll()
        {
            using (var ctx = new Context())
            {
                return await ctx.Books.Include(r => r.Cover).Include(x => x.BookMarks).Include(t => t.Source).ToListAsync();
            }
        }

        public async void Update(Book book)
        {
            using (var ctx = new Context())
            {
                ctx.Books.Attach(book);
                ctx.Entry(book).State = EntityState.Modified;
                await ctx.SaveChangesAsync();
            }
        }
    }
}