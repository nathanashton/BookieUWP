using Bookie.Common.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookie.Domain.Interfaces
{
    public interface IBookMarkRepository
    {
        ICollection<BookMark> Find(Func<BookMark, bool> where);

        Task<BookMark> Add(BookMark book);

        Task<List<BookMark>> GetAll();

        void Update(BookMark book);

        void Remove(BookMark bookmark);
    }
}