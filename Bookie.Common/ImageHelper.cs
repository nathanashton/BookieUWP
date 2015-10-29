using Bookie.Common.Model;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Bookie.Common
{
    public static class ImageHelper
    {
        public static async Task<ImageSource> GetImage(Book book)
        {
            var storageFolder = await Globals.GetCoversFolder();
            var storageFile = await storageFolder.GetFileAsync(book.Cover.FileName);

            BitmapImage bitmapImage = null;

            if (storageFile != null)
            {
                bitmapImage = new BitmapImage();

                using (var stream = await storageFile.OpenReadAsync())
                {
                    await bitmapImage.SetSourceAsync(stream);
                }
            }
            return (bitmapImage);
        }
    }
}