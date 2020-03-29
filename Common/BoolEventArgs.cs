using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAnalyse
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
