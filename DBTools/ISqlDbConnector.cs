using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoAnalysis
{
    public interface ISqlDbConnector
    {
        DataTable GetTable(string query);
    }
}
