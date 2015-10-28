using System;
using System.Collections.Generic;
using Bookie.Common.Model;

namespace Bookie.Domain.Interfaces
{
    public interface IBookService
    {
        List<Book> Find(Func<Book, bool> where);
        List<Book> GetByTitle(string title);
        Book GetById(int id);
        List<Book> GetAll();
        Book Add(Book book);
    }
}
