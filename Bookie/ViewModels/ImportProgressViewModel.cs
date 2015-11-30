using Bookie.Common;
using Bookie.Mvvm;

namespace Bookie.ViewModels
{
    public class ImportProgressViewModel : NotifyBase
    {
        private string _operation;

        private int _progress;
        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        public RelayCommand CancelCommand
        {
            get { return new RelayCommand((object args) => { Cancel(); }); }
        }

        public string Operation
        {
            get { return _operation; }
            set
            {
                _operation = value;
                NotifyPropertyChanged("Operation");
            }
        }

        private string _operationSubText;

        public string OperationSubText
        {
            get { return _operationSubText; }
            set
            {
                _operationSubText = value;
                NotifyPropertyChanged("OperationSubText");
            }
        }

        public int Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                NotifyPropertyChanged("Progress");
            }
        }

        private void Cancel()
        {
            ProgressService.Cancel();
        }
    }
}