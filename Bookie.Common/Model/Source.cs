using System.Collections.Generic;

namespace Bookie.Common.Model
{
    public class Source
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Path { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}
