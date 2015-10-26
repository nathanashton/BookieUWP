using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Bookie.Common.Model;

namespace Bookie.Data
{
    public class SourceDal
    {
        public void Add(StorageFolder folder)
        {
            using (var context = new BookieContext())
            {
                var source = new Source
                {
                    Path = folder.Path,
                    Token = StorageApplicationPermissions.FutureAccessList.Add(folder)
                };

                if (context.Sources.FirstOrDefault(x => x.Path == source.Path) != null) return;
                context.Sources.Add(source);
                context.SaveChanges();
            }
        }

        public List<Source> GetAllAsSources()
        {
            using (var context = new BookieContext())
            {
                return context.Sources.ToList();
            }
        }

        public async Task<List<StorageFolder>> GetAllAsStorageFolders()
        {
            var folders = new List<StorageFolder>();

            var sources = GetAllAsSources();
            if (sources.Count == 0) return folders;
            foreach (var source in sources)
            {
                var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(source.Token);
                folders.Add(folder);
            }
            return folders;
        }

        //Goes through all files in all folders, creates a book and if it doesnt exist in Db it is added.
        public async void UpdateBooksFromSources()
        {
            var storageFolders = await GetAllAsStorageFolders();

            foreach (var storageFolder in storageFolders)
            {
                var source = GetSourceFromStorageFolderPath(storageFolder.Path);
                var storageFiles = await storageFolder.GetFilesAsync();
                foreach (var storageFile in storageFiles)
                {
                    var book = new Book
                    {
                        Title = storageFile.DisplayName,
                        Source = source,
                        FileName = Path.GetFileName(storageFile.Path),
                        FullPathAndFileName = storageFile.Path
                    };
                    AddBookToSource(book);
                }
            }
        }

        public Source GetSourceFromStorageFolderPath(string path)
        {
            using (var context = new BookieContext())
            {
                var source = context.Sources.FirstOrDefault(x => x.Path == path);
                if (source != null)
                {
                    return source;
                }
                return null;
            }
        }

        public void AddBookToSource(Book book)
        {
            using (var context = new BookieContext())
            {
                if (context.Books.FirstOrDefault(x => x.FullPathAndFileName == book.FullPathAndFileName) != null) return;
                context.Books.Add(book);
                context.SaveChanges();
            }
        }

        public async Task<StorageFolder> GetStorageFolderFromSource(Source source)
        {
            var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(source.Token);
            return folder;
        }
    }
}
