namespace Bookie.Common.Model
{
    public class BookMark
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public virtual Book Book { get; set; }
    }
}