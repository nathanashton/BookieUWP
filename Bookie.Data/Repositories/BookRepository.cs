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
        public ICollection<Book> Find(Func<Book, bool> where)
        {
            using (var ctx = new Context())
            {
                return
                    ctx.Books.Include(r => r.Cover)
                        .Include(x => x.BookMarks)
                        .Include(y => y.Source)
                        .Where(where)
                        .ToList();
            }
        }

        public async Task<Book> AddAsync(Book book)
        {
            using (var ctx = new Context())
            {
                var added = ctx.Books.Add(book);
                await ctx.SaveChangesAsync();
                return added.Entity;
            }
        }

        public async Task<List<Book>> GetAllAsync()
        {
            using (var ctx = new Context())
            {
                return
                    await ctx.Books.Include(r => r.Cover).Include(x => x.BookMarks).Include(t => t.Source).ToListAsync();
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

        public void Remove(Book book)
        {
            using (var ctx = new Context())
            {
                if (book.Cover != null)
                {
                    ctx.Covers.Attach(book.Cover);
                    ctx.Entry(book.Cover).State = EntityState.Deleted;
                    ctx.SaveChanges();

                }

                if (book.BookMarks != null && book.BookMarks.Count > 0)
                {
                    foreach (var bookmark in book.BookMarks)
                    {
                        ctx.Bookmarks.Attach(bookmark);
                        ctx.Entry(bookmark).State = EntityState.Deleted;
                        ctx.SaveChanges();

                    }
                }

                ctx.Books.Attach(book);
                ctx.Entry(book).State = EntityState.Deleted;

                ctx.SaveChanges();
            }
        }

        public bool Exists(Book book)
        {
            using (var ctx = new Context())
            {
                return ctx.Books.Any(x => x.FullPathAndFileName == book.FullPathAndFileName);
            }
        }
    }
}