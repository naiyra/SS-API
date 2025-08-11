using SS_API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS_API.DataHandling
{
    public interface IDataCleaner
    {
        IDataFrame Clean(IDataFrame rawInput);
    }



}
