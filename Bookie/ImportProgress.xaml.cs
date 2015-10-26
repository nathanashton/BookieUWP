using Bookie.ViewModels;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bookie
{
    public sealed partial class ImportProgress : ContentDialog
    {
        public ImportProgressViewModel ViewModel { get; set; }

        public ImportProgress()
        {
            this.InitializeComponent();
            ViewModel = new ImportProgressViewModel();
            DataContext = ViewModel;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}