using Bookie.Common.Model;
using System;
using System.Collections.Generic;

namespace Bookie.Domain.Interfaces
{
    public interface ISourceRepository
    {
        ICollection<Source> Find(Func<Source, bool> where);

        Source Add(Source source);

        ICollection<Source> GetAll();
    }
}