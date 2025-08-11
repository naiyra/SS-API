using SS_API.Core;

namespace SS_API.MachineLearning
{
    public interface IClusterer
    {
        ClusteredData Cluster(IDataFrame featureMatrix);
    }
}
