using SS_API.Core;
using System.Collections.Generic;

namespace SS_API.MachineLearning
{
    public class ClusteredData
    {
        public IDataFrame Features { get; set; }
        public List<int> ClusterAssignments { get; set; }
    }
}
