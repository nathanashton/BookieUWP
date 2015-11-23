using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bookie.Common.Model;

namespace Bookie.Domain.Interfaces
{
    public interface IBookRepository
    {
        ICollection<Book> Find(Func<Book, bool> where);

        Task<Book> Add(Book book);

        Task<List<Book>> GetAll();

        void Update(Book book);
    }
}