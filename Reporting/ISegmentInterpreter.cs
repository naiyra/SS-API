using SS_API.FeaturesHandling;
using SS_API.MachineLearning;
using SS_API.FeaturesHandling;
using SS_API.Core;

namespace SS_API.Reporting
{
    public interface ISegmentInterpreter
    {
        Task<List<SegmentSummary>> Interpret(ClusteredData clustered, FeatureEngineeringContext context);
    }

}
