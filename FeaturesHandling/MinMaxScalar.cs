using SS_API.Core;
using SS_API.FeaturesHandling;

public class MinMaxScaler
{
    public IDataFrame Scale(IDataFrame df, List<string> numericCols, FeatureEngineeringContext context)
    {
        foreach (var col in numericCols)
        {
            var values = df.GetColumnValues(col)
                           .Select(v => Convert.ToDouble(v))
                           .ToList();

            double min = values.Min();
            double max = values.Max();

            // Save min/max to context
            context.NumericRanges[col] = (min, max);

            List<object> scaled;
            if (Math.Abs(max - min) < 1e-9)
            {
                scaled = values.Select(v => (object)0.0).ToList();
            }
            else
            {
                scaled = values.Select(v => (object)((v - min) / (max - min))).ToList();
            }

            df.SetColumnValues(col, scaled);
        }

        return df;
    }
}
