using System;

namespace FlexibleDBMS
{
    public class BoolEventArgs : EventArgs
    {
        public bool Status { get; private set; }

        public BoolEventArgs(bool status)
        {
            Status = status;
        }
    }
}
