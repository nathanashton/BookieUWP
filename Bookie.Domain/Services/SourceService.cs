using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Bookie.Common.Model;
using Bookie.Domain.Interfaces;

namespace Bookie.Domain.Services
{
    public class SourceService : ISourceService
    {
        private readonly ISourceRepository _repository;

        public SourceService(ISourceRepository repository)
        {
            _repository = repository;
        }
        public Source GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Source GetByUrl(string url)
        {
            var found = _repository.Find(r => r.Path == url).FirstOrDefault();
            return found;
        }

        public List<Source> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public async Task<List<StorageFolder>> GetAllAsStorageFolders()
        {
            var all = _repository.GetAll();
            var folders = new List<StorageFolder>();
            foreach (var source in all)
            {
                var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(source.Token);
                folders.Add(folder);
            }
            return folders;
        }

        public Source Add(Source source)
        {
            return _repository.Add(source);
        }

        public async Task<StorageFolder> GetStorageFolderFromSource(Source source)
        {
            var f = _repository.Find(x => x.Path == source.Path);
            var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(source.Token);
            return folder;
        }
    }
}
