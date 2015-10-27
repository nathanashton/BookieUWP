﻿using Bookie.Common.Interfaces;
using Bookie.Common.Model;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.ChangeTracking;

namespace Bookie.Data
{
    public class BookDal : IBookieData<Book>
    {
        public Book GetSingle(string query)
        {
            using (var context = new BookieContext())
            {
                return context.Books.FirstOrDefault(x => x.Title == query);
            }
        }

        Book IBookieData<Book>.Add(Book item)
        {
            using (var context = new BookieContext())
            {
                if (Exists(item))
                {
                    return null;
                }
                var addedBook = context.Books.Add(item);
                context.SaveChanges();
                return addedBook.Entity;
            }
        }

        public bool Exists(Book book)
        {
            using (var context = new BookieContext())
            {
                return context.Books.FirstOrDefault(x => x.FullPathAndFileName == book.FullPathAndFileName) != null;
            }
        }

        public void Remove(Book item)
        {
            using (var context = new BookieContext())
            {
                context.Books.Remove(item);
                context.SaveChanges();
            }
        }

        public void Udpate(Book item)
        {
            using (var context = new BookieContext())
            {
                context.Books.Attach(item);
                context.Entry(item).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        private void U(object item)
        {
            var s = (EntityEntryGraphNode)item;

        }

       public List<Book> GetAll()
        {
            using (var context = new BookieContext())
            {
                return context.Books.Include(x => x.Source).Include(y => y.Cover).Include(w=> w.BookMarks).ToList();
            }
}

        public Book GetSingle(int id)
        {
            using (var context = new BookieContext())
            {
                return context.Books.FirstOrDefault(x => x.Id == id);
            }
        }
    }
}