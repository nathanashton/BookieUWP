using System;
using Bookie.Common.EventArgs;

namespace Bookie.Common.Interfaces
{
    public interface IProgressPublisher
    {
        event EventHandler<ProgressWindowEventArgs> ProgressChanged;

        event EventHandler<System.EventArgs> ProgressComplete;

        event EventHandler<System.EventArgs> ProgressStarted;

        void ProgressCancel();
    }
}