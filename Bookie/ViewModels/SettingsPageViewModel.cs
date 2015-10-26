using Bookie.Common.Model;
using Bookie.Core;
using Bookie.Mvvm;
using System;
using System.Collections.ObjectModel;
using Bookie.Data;

namespace Bookie.ViewModels
{
    public class SettingsPageViewModel : NotifyBase
    {
        private ObservableCollection<Source> _sources;

        public ObservableCollection<Source> Sources
        {
            get { return _sources; }
            set
            {
                _sources = value;
                NotifyPropertyChanged("Sources");
            }
        }

        private Source _selectedSource;

        public Source SelectedSource
        {
            get { return _selectedSource; }
            set
            {
                _selectedSource = value;
                NotifyPropertyChanged("SelectedSource");
            }
        }

        private RelayCommand _addCommand;

        public RelayCommand AddCommand
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    AddSource();
                });
            }
        }

        private RelayCommand _removeCommand;

        public RelayCommand RemoveCommand
        {
            get
            {
                return new RelayCommand((object args) =>
                {
                    RemoveSource();
                });
            }
        }

        private async void AddSource()
        {
            var picker = new Windows.Storage.Pickers.FolderPicker();
            picker.FileTypeFilter.Add("*");
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            var folder = await picker.PickSingleFolderAsync();
            if (folder == null) return;
            var sources = new SourceDal();
            sources.Add(folder);
            GetAllSources();
        }

        private void RemoveSource()
        { }

        public SettingsPageViewModel()
        {
            GetAllSources();
        }

        private void GetAllSources()
        {
         var sources = new SourceDal();
         var all = sources.GetAllAsSources();
      Sources = new ObservableCollection<Source>(all);
        }
    }
}