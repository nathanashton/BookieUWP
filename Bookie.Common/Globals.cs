using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Bookie.Common
{
    public static class Globals
    {
    
        public async static Task<StorageFolder> GetCoversFolder()
        {
            StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            string desiredName = "Covers";
            StorageFolder coversFolder = await localFolder.CreateFolderAsync(desiredName,CreationCollisionOption.OpenIfExists);
            return coversFolder;
        }

    }
}
