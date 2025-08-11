using SS_API.Core;
using System.Collections.Generic;
using System.Linq;

namespace SS_API.FeaturesHandling
{
    public class LabelEncoder
    {
        private readonly Dictionary<string, Dictionary<string, int>> encoders = new();
        private readonly Dictionary<string, Dictionary<int, string>> decoders = new();

        public IDataFrame Encode(IDataFrame df, List<string> categoricalCols)
        {
            foreach (var col in categoricalCols)
            {
                var values = df.GetColumnValues(col)
                               .Select(v => v?.ToString() ?? "UNKNOWN")
                               .ToList();

                var unique = values.Distinct().ToList();

                var map = unique.Select((val, idx) => new { val, idx })
                                .ToDictionary(x => x.val, x => x.idx);

                encoders[col] = map;
                decoders[col] = map.ToDictionary(kv => kv.Value, kv => kv.Key);

                var encoded = values.Select(v => (object)map[v]).ToList();
                df.SetColumnValues(col, encoded);
            }

            return df;
        }

        public bool HasDecoder(string columnName)
        {
            return decoders.ContainsKey(columnName);
        }

        public Dictionary<int, string> GetDecoder(string columnName)
        {
            return decoders[columnName];
        }

        public string DecodeSingle(string columnName, int encodedValue)
        {
            if (!decoders.ContainsKey(columnName))
                return encodedValue.ToString();

            var mapping = decoders[columnName];
            return mapping.TryGetValue(encodedValue, out var original) ? original : encodedValue.ToString();
        }
    }
}
