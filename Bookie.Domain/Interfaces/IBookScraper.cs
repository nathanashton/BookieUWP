using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Bookie.Common;

namespace Bookie.Domain.Interfaces
{
    public interface IBookScraper
    {
        object SearchQuery { get; set; }
        Task<ObservableCollection<SearchResult>> SearchBooks(object searchQuery);
    }
}