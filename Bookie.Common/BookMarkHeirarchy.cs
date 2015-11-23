using System.Collections.ObjectModel;

namespace Bookie.Common
{
    public class BookMarkBase
    {
        public string Title { get; set; }
        public int PageNumber { get; set; }
        public ObservableCollection<BookMarkBase> BookMarks { get; set; }
    }
}