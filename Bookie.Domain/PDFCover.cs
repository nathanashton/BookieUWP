using System;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Bookie.Common;
using Bookie.Common.Model;
using Bookie.Domain.Interfaces;
using Bookie.Domain.Services;

namespace Bookie.Domain
{
    public class PdfCover
    {
        private PdfDocument _pdfDocument;

        public async Task<string> GenerateCoverImage(Book book, uint pageIndex, ISourceRepository sourceRepository)
        {
            try
            {
                var sources = new SourceService(sourceRepository);
                var storageFolder = await sources.GetStorageFolderFromSource(book.Source);

                var pdfFile = await storageFolder.GetFileAsync(book.FileName);
                ///Load Pdf File
                //  StorageFile pdfFile = null;

                _pdfDocument = await PdfDocument.LoadFromFileAsync(pdfFile);

                if (_pdfDocument != null && _pdfDocument.PageCount > 0)
                {
                    //Get Pdf page
                    var pdfPage = _pdfDocument.GetPage(pageIndex);

                    if (pdfPage != null)
                    {
                        // next, generate a bitmap of the page
                        var thumbfolder = await Globals.GetCoversFolder();

                        var pngFile =
                            await
                                thumbfolder.CreateFileAsync(Utils.GenerateRandomString() + ".png",
                                    CreationCollisionOption.ReplaceExisting);

                        if (pngFile != null)
                        {
                            var randomStream = await pngFile.OpenAsync(FileAccessMode.ReadWrite);
                            await pdfPage.RenderToStreamAsync(randomStream);
                            await randomStream.FlushAsync();

                            randomStream.Dispose();
                            pdfPage.Dispose();
                            //await this.resfreshcontent();
                            return pngFile.Path;
                        }
                        return null;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception)
            {
                //rootPage.NotifyUser("Error: " + err.Message, NotifyType.ErrorMessage);
                return null;
            }
        }
    }
}