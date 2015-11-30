namespace Bookie.Common.EventArgs
{
    public class ProgressWindowEventArgs : System.EventArgs
    {
        public int ProgressPercentage { get; set; }
        public string OperationName { get; set; }
        public string OperationSubText { get; set; }
        public bool Cancel { get; set; }
        public string ProgressBarText { get; set; }
        public string ProgressText { get; set; }
    }
}