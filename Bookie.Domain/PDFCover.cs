using System;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Bookie.Common;
using Bookie.Common.Model;
using Bookie.Domain.Interfaces;

namespace Bookie.Domain
{
    public class PdfCover
    {
        private PdfDocument _pdfDocument;

        public async Task<string> GenerateCoverImage(Book book, uint pageIndex, ISourceRepository sourceRepository, StorageFolder storageFolder, StorageFile pdfFile)
        {
            try
            {
               // var pdfFile = await storageFolder.GetFileAsync(book.FileName);
        
                try
                {
                    _pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile);

                }
                catch (Exception)
                {
                    return null;
                }

                if (_pdfDocument == null || _pdfDocument.PageCount <= 0) return null;
                //Get Pdf page


                using (var pdfPage = _pdfDocument.GetPage(pageIndex))
                {
                    if (pdfPage == null) return null;
                    // next, generate a bitmap of the page
                    var thumbfolder = await Globals.GetCoversFolder();

                    var pngFile = await thumbfolder.CreateFileAsync(Utils.GenerateRandomString() + ".png", CreationCollisionOption.ReplaceExisting);

                    if (pngFile == null) return null;
                    using (var randomStream = await pngFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await pdfPage.RenderToStreamAsync(randomStream, new PdfPageRenderOptions() { DestinationHeight = 340, DestinationWidth = 240 });
                        await randomStream.FlushAsync();

                    }


                    return pngFile.Path;
                }


            }
            catch (Exception)
            {
                //rootPage.NotifyUser("Error: " + err.Message, NotifyType.ErrorMessage);
                return null;
            }
        }
    }
}