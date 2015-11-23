using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bookie.Common.Model;

namespace Bookie.Domain.Interfaces
{
    public interface IBookMarkService
    {
        List<BookMark> Find(Func<BookMark, bool> where);

        BookMark GetById(int id);

        Task<List<BookMark>> GetAll();

        List<BookMark> GetAllForBook(Book book);

        BookMark Add(BookMark book);

        void Update(BookMark book);

        void Remove(BookMark bookmark);
    }
}