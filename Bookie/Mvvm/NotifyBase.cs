using System;
using System.ComponentModel;
using Windows.UI.Core;

namespace Bookie.Mvvm
{
    public class NotifyBase : INotifyPropertyChanged
    {
        public CoreDispatcher Dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

        public event PropertyChangedEventHandler PropertyChanged;

        public async void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                await
                    Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                        () => { PropertyChanged(this, new PropertyChangedEventArgs(info)); });
            }
        }
    }
}