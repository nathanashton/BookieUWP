using Bookie.Common.Model;
using System;
using System.Collections.Generic;

namespace Bookie.Domain.Interfaces
{
    public interface IBookMarkRepository
    {
        ICollection<BookMark> Find(Func<BookMark, bool> where);

        BookMark Add(BookMark book);

        ICollection<BookMark> GetAll();

        void Update(BookMark book);

        void Remove(BookMark bookmark);
    }
}