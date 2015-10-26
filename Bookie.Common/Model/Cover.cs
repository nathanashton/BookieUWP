using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookie.Common.Model
{
    public class Cover
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public virtual Book Book { get; set; }
    }
}
