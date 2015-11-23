using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Bookie.Common
{
    public static class Globals
    {
        public static async Task<StorageFolder> GetCoversFolder()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var desiredName = "Covers";
            var coversFolder = await localFolder.CreateFolderAsync(desiredName, CreationCollisionOption.OpenIfExists);
            return coversFolder;
        }

        public static async Task<StorageFolder> GetThemesFolder()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var desiredName = "Themes";
            var coversFolder = await localFolder.CreateFolderAsync(desiredName, CreationCollisionOption.OpenIfExists);
            return coversFolder;
        }


    }
}