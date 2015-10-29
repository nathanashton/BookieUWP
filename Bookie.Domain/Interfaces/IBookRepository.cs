using Bookie.Common.Model;
using System;
using System.Collections.Generic;

namespace Bookie.Domain.Interfaces
{
    public interface IBookRepository
    {
        ICollection<Book> Find(Func<Book, bool> where);

        Book Add(Book book);

        ICollection<Book> GetAll();

        void Update(Book book);
    }
}