using System;
using System.Collections.Generic;
using Bookie.Common.Model;

namespace Bookie.Domain.Interfaces
{
    public interface IBookRepository
    {
        ICollection<Book> Find(Func<Book, bool> where);
        Book Add(Book book);
        ICollection<Book> GetAll();
    }
}
