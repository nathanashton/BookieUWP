using System;
using System.Collections.Generic;
using Bookie.Common.Model;

namespace Bookie.Domain.Interfaces
{
    public interface ISourceRepository
    {
        ICollection<Source> Find(Func<Source, bool> where);
        Source Add(Source source);
        ICollection<Source> GetAll();
    }
}
