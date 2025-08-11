using SS_API.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SS_API.FeaturesHandling
{
    public class FeaturesEngineering : IFeaturesEngineering
    {
        private readonly FeaturesTypeDetector detector = new();
        private readonly MinMaxScaler scaler = new();
        private readonly ColumnDropper dropper = new();
        private readonly LabelEncoder labelEncoder = new();

        public IDataFrame Transform(IDataFrame input, FeatureEngineeringContext context)
        {
            var df = input.Clone();

            // Drop irrelevant columns
            df = dropper.DropIrrelevantColumns(df);

            // Detect feature types
            var (numericCols, categoricalCols) = detector.Detect(df);
            context.CategoricalColumns = categoricalCols;

            // Encode categorical features with Label Encoding
            if (categoricalCols.Count > 0)
            {
                df = labelEncoder.Encode(df, categoricalCols);
                // Save decoders into context
                foreach (var col in categoricalCols)
                {
                    if (labelEncoder.HasDecoder(col))
                        context.Decoders[col] = labelEncoder.GetDecoder(col);
                }
            }

            // Scale numeric features
            if (numericCols.Count > 0)
            {
                df = scaler.Scale(df, numericCols, context); // 🛠️ Pass context to save min/max
            }


            return df;
        }

        public IDataFrame Decode(IDataFrame encodedDf, FeatureEngineeringContext context)
        {
           
            return encodedDf.Clone();
        }
    }
}
