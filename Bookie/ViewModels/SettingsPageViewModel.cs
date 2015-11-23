using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Bookie.Common;
using Bookie.Common.Model;
using Bookie.Data.Repositories;
using Bookie.Domain;
using Bookie.Domain.Services;
using Bookie.Mvvm;

namespace Bookie.ViewModels
{
    public class SettingsPageViewModel : NotifyBase
    {
        private Theme _theme;

        public Theme Theme
        {
            get { return _theme; }
            set { _theme = value;
                NotifyPropertyChanged("Theme");
                if (_theme != null && _theme.Resource != null)
                {
                    ChangeTheme(_theme);
                }
            }
        }

        private Source _selectedSource;

        private ObservableCollection<Theme> _themes;

        public ObservableCollection<Theme> Themes
        {
            get { return _themes; }
            set { _themes = value;
                NotifyPropertyChanged("Themes");
            }
        }
        private ObservableCollection<Source> _sources;


        public void Load()
        {
            Themes = new ObservableCollection<Theme>(BookieSettings.Themes);
            Theme = Themes.FirstOrDefault(x => x.Resource.Source == BookieSettings.Theme.Resource.Source);
            GetAllSources();
        }


        public ObservableCollection<Source> Sources
        {
            get { return _sources; }
            set
            {
                _sources = value;
                NotifyPropertyChanged("Sources");
            }
        }

        public Source SelectedSource
        {
            get { return _selectedSource; }
            set
            {
                _selectedSource = value;
                NotifyPropertyChanged("SelectedSource");
            }
        }


        public RelayCommand AddCommand
        {
            get { return new RelayCommand((object args) => { AddSource(); }); }
        }


        public RelayCommand RemoveCommand
        {
            get { return new RelayCommand((object args) => { RemoveSource(); }); }
        }

        public RelayCommand UpdateCommand
        {
            get { return new RelayCommand((object args) => { UpdateBooksFromSources(); }); }
        }





        public void ChangeTheme(Theme theme)
        {
            //Prompt User to restart


            //var loadedResources = App.Current.Resources.MergedDictionaries.ToList();
            //foreach (var resource in loadedResources)
            //{
            //    if (resource.Source.ToString().Contains("Theme"))
            //    {
            //        App.Current.Resources.MergedDictionaries.Remove(resource);
            //    }
            //}
            //App.Current.Resources.MergedDictionaries.Add(theme.Resource);
            BookieSettings.Theme = Theme;
           // Theme = Themes.FirstOrDefault(x => x.Resource.Source == BookieSettings.Theme.Resource.Source);

            BookieSettings.SaveSettings();

        }







        private async void AddSource()
        {
            var picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");
            picker.ViewMode = PickerViewMode.List;
            var folder = await picker.PickSingleFolderAsync();
            if (folder == null) return;
            var source = new Source
            {
                Path = folder.Path,
                Token = StorageApplicationPermissions.FutureAccessList.Add(folder)
            };
            new SourceService(new SourceRepository()).Add(source);
            GetAllSources();
        }

        private void RemoveSource()
        {
        }

        private void GetAllSources()
        {
            var all = new SourceService(new SourceRepository()).GetAll();
            Sources = new ObservableCollection<Source>(all);
        }

        private void UpdateBooksFromSources()
        {
            var importer = new Importer(new BookRepository(), new SourceRepository());

            importer.UpdateBooksFromSources();
        }
    }
}