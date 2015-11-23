using System.Collections.ObjectModel;
using Bookie.Mvvm;

namespace Bookie.ViewModels
{
    internal class ViewModelBase : BindableBase
    {
        private static readonly ObservableCollection<MenuItem> menu = new ObservableCollection<MenuItem>();

        public ObservableCollection<MenuItem> Menu => menu;
    }
}