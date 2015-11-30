using System;
using System.ComponentModel;
using Windows.UI.Core;

namespace Bookie.Common
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
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () => { PropertyChanged(this, new PropertyChangedEventArgs(info)); });
            }
        }
    }
}