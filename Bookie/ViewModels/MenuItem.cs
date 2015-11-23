using System;
using System.Windows.Input;
using Bookie.Mvvm;

namespace Bookie.ViewModels
{
    internal class MenuItem : BindableBase
    {
        private RelayCommand _command;
        private string _glyph;
        private Type _navigationDestination;
        private string _text;

        public string Glyph
        {
            get { return _glyph; }
            set { SetProperty(ref _glyph, value); }
        }

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        public ICommand Command
        {
            get { return _command; }
            set { SetProperty(ref _command, (RelayCommand) value); }
        }

        public Type NavigationDestination
        {
            get { return _navigationDestination; }
            set { SetProperty(ref _navigationDestination, value); }
        }

        public bool IsNavigation => _navigationDestination != null;

        public object ViewModel { get; set; }
    }
}