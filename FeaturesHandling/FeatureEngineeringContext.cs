namespace SS_API.FeaturesHandling
{
    public class FeatureEngineeringContext
    {
        public List<string> CategoricalColumns { get; set; } = new();
        public Dictionary<string, Dictionary<int, string>> Decoders { get; set; } = new();

       
        public Dictionary<string, (double Min, double Max)> NumericRanges { get; set; } = new();
    }

}
