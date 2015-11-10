using Bookie.Common.Model;
using Bookie.Domain.Interfaces;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<BookMark> Add(BookMark book)
        {
            using (var ctx = new Context())
            {
                var added = ctx.Bookmarks.Add(book);
                await ctx.SaveChangesAsync();
                return added.Entity;
            }
        }

        public async Task<List<BookMark>> GetAll()
        {
            using (var ctx = new Context())
            {
                return await ctx.Bookmarks.Include(b => b.Book).ToListAsync();
            }
        }

        public async void Update(BookMark book)
        {
            using (var ctx = new Context())
            {
                ctx.Bookmarks.Attach(book);
                ctx.Bookmarks.Update(book);
                await ctx.SaveChangesAsync();
            }
        }

        public async void Remove(BookMark bookmark)
        {
            using (var ctx = new Context())
            {

                    ctx.Bookmarks.Attach(bookmark);
                    ctx.Bookmarks.Remove(bookmark);
                    await ctx.SaveChangesAsync();
            }
        }
    }
}