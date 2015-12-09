using Syncfusion.Pdf.Interactive;

namespace Bookie
{
    public class BMark
    {
        public string Description { get; set; }
        public PdfLoadedBookmark BookMark { get; set; }
        public bool Selected { get; set; }
}
}
