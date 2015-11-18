using Bookie.Mvvm;
using System.Collections.ObjectModel;

namespace Bookie.ViewModels
{
    internal class ViewModelBase : BindableBase
    {
        private static readonly ObservableCollection<MenuItem> menu = new ObservableCollection<MenuItem>();

        public ObservableCollection<MenuItem> Menu => menu;
    }
}