namespace Bookie.Common.EventArgs
{
    public class BookieMessageEventArgs : System.EventArgs
    {
        public string MoreDetails { get; set; }
        public string Message { get; set; }
        public bool Fatal { get; set; }
    }
}