using System;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Bookie.Common.EventArgs;
using Bookie.Common.Model;
using Bookie.Views;

namespace Bookie.ViewModels
{
    internal class ShellViewModel : ViewModelBase
    {
        public ShellViewModel()
        {
            MainViewModel = new MainPageViewModel();
            PdfViewModel = new PdfViewModel();
            SettingsViewModel = new SettingsPageViewModel();

            Menu.Add(new MenuItem
            {
                Glyph = "",
                Text = "Library",
                NavigationDestination = typeof (MainPage),
                ViewModel = MainViewModel
            });
            Menu.Add(new MenuItem
            {
                Glyph = "",
                Text = "Viewer",
                NavigationDestination = typeof (PdfPage),
                ViewModel = PdfViewModel
            });
            Menu.Add(new MenuItem
            {
                Glyph = "",
                Text = "Settings",
                NavigationDestination = typeof (SettingsPage),
                ViewModel = SettingsViewModel
            });
        }

        public MainPageViewModel MainViewModel { get; set; }
        public PdfViewModel PdfViewModel { get; set; }
        public SettingsPageViewModel SettingsViewModel { get; set; }

        public static Book SelectedBook { get; set; }

        public delegate void MessageHandler(BookieMessageEventArgs e);

        public static event MessageHandler ShowMessageEvent;

        public static event MessageHandler ShowMessageDialogEvent;


        public static List<Book> Books;

        public static void ShowMessage(string message, string moredetails)
        {
            if (ShowMessageEvent != null)
            {
                ShowMessageEvent(new BookieMessageEventArgs {MoreDetails = moredetails, Message = message});
            }
        }

        public static async void ShowMessageDialog(string message)
        {
            MessageDialog m = new MessageDialog(message);
            await m.ShowAsync();
        }

        public ElementTheme Theme
        {
            get
            {
                if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    return ElementTheme.Light;
                }
                return ElementTheme.Dark;
            }
        }
    }
}