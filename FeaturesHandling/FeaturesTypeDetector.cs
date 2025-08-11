using SS_API.Core;
using System.Collections.Generic;
using System.Linq;

namespace SS_API.FeaturesHandling
{
    public class FeaturesTypeDetector
    {
        public (List<string> numericCols, List<string> categoricalCols) Detect(IDataFrame df)
        {
            var numeric = new List<string>();
            var categorical = new List<string>();

            foreach (var col in df.GetColumnNames())
            {
                var values = df.GetColumnValues(col)
                               .Where(v => v != null)
                               .Select(v => v.ToString())
                               .ToList();

                if (values.Count == 0)
                    continue; // Skip empty columns

                bool isAllNumeric = values.All(v => double.TryParse(v, out _));
                int uniqueCount = values.Distinct().Count();

                if (isAllNumeric)
                {
                    if (uniqueCount <= 5)
                    {
                        // Small number of unique numeric values
                       
                        categorical.Add(col);
                    }
                    else
                    {
                        // Many unique numeric values > continuous > numeric
                        numeric.Add(col);
                    }
                }
                else
                {
                    // Non-numeric > Categorical
                    categorical.Add(col);
                }
            }

            return (numeric, categorical);
        }
    }
}
