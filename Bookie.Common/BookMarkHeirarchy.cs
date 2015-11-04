using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookie.Common
{
    public class BookMarkBase
    {
        public string Title { get; set; }
        public int PageNumber { get; set; }
        public ObservableCollection<BookMarkBase> BookMarks { get; set; }
    }
}
