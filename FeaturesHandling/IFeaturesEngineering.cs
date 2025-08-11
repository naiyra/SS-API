using SS_API.Core;

namespace SS_API.FeaturesHandling
{
    public interface IFeaturesEngineering
    {
        IDataFrame Transform(IDataFrame input, FeatureEngineeringContext context);
    }

}
