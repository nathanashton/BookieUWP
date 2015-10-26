using Bookie.Mvvm;
using System.Collections.ObjectModel;

namespace Bookie.ViewModels
{
    internal class ViewModelBase : BindableBase
    {
        private static ObservableCollection<MenuItem> menu = new ObservableCollection<MenuItem>();

        public ObservableCollection<MenuItem> Menu => menu;
    }
}