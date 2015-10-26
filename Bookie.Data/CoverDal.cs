using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bookie.Common.Model;

namespace Bookie.Data
{
    public class CoverDal
    {
        public Cover AddCover(Cover cover)
        {
            using (var context = new BookieContext())
            {
                var addedCover = context.Covers.Add(cover);
                context.SaveChanges();
                return addedCover.Entity;
            }
        }
    }
}
