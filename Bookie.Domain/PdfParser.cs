using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Syncfusion.Pdf.Parsing;

namespace Bookie.Domain
{
    public class PdfParser
    {
        public async Task<string> Extract(StorageFile file, int pageNumber)
        {
            var doc = new PdfLoadedDocument();
            await doc.OpenAsync(file);
            var page = doc.Pages[pageNumber];
            var extractedText = page.ExtractText();
            return extractedText;
        }

        public async Task<string> Extract(StorageFile file, int startPageNumber, int endPageNumber)
        {
            var doc = new PdfLoadedDocument();
            await doc.OpenAsync(file);
            var sb = new StringBuilder();
            for (var i = startPageNumber; i <= endPageNumber; i++)
            {
                var page = doc.Pages[i];
                sb.Append(page.ExtractText());
            }
            return sb.ToString();
        }
    }
}