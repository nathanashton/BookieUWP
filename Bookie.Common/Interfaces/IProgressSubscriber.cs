using Bookie.Common.EventArgs;

namespace Bookie.Common.Interfaces
{
    public interface IProgressSubscriber
    {
        void _progress_ProgressChanged(object sender, ProgressWindowEventArgs e);

        void _progress_ProgressStarted(object sender, System.EventArgs e);

        void _progress_ProgressCompleted(object sender, System.EventArgs e);
    }
}