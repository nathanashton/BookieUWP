using Bookie.Common.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Bookie.Domain.Interfaces
{
    public interface ISourceService
    {
        Source GetById(int id);

        Source GetByUrl(string url);

        List<Source> GetAll();

        Task<List<StorageFolder>> GetAllAsStorageFolders();

        Task<StorageFolder> GetStorageFolderFromSource(Source source);

        Source Add(Source source);
    }
}