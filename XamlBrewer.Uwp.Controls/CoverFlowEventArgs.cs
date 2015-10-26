using System;

namespace Controls.CoverFlow
{
    public class CoverFlowEventArgs : EventArgs
    {
        public int Index { get; set; }
        public object Item { get; set; }
    }
}
