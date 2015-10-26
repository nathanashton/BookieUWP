using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Bookie.Common.Model;

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
