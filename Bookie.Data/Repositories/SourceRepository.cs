using System;
using System.Collections.Generic;
using System.Linq;
using Bookie.Common.Model;
using Bookie.Domain.Interfaces;
using Microsoft.Data.Entity;

namespace Bookie.Data.Repositories
{
    public class SourceRepository : ISourceRepository
    {
        public ICollection<Source> Find(Func<Source, bool> where)
        {
            using (var ctx = new Context())
            {
                return ctx.Sources.Include(t=>t.Books).Where(where).ToList();
            }
        }

        public Source Add(Source source)
        {
            using (var ctx = new Context())
            {
                var added = ctx.Sources.Add(source);
                ctx.SaveChanges();
                return added.Entity;
            }
        }

        public ICollection<Source> GetAll()
        {
            using (var ctx = new Context())
            {
                return ctx.Sources.Include(r=> r.Books).ToList();
            }
        }
    }
}