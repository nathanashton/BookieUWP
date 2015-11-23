using Windows.UI.Xaml.Controls;
using Bookie.ViewModels;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bookie.Views
{
    public sealed partial class ImportProgress : ContentDialog
    {
        public ImportProgress()
        {
            InitializeComponent();
            ViewModel = new ImportProgressViewModel();
            DataContext = ViewModel;
        }

        public ImportProgressViewModel ViewModel { get; set; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}