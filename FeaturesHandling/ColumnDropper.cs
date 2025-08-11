using SS_API.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SS_API.FeaturesHandling
{
    public class ColumnDropper
    {
        private readonly double missingThreshold;
        private readonly double varianceThreshold;
        private readonly double uniquenessThreshold;

        public ColumnDropper(double missingThreshold = 0.4, double varianceThreshold = 0.01, double uniquenessThreshold = 0.95)
        {
            this.missingThreshold = missingThreshold;
            this.varianceThreshold = varianceThreshold;
            this.uniquenessThreshold = uniquenessThreshold;
        }

        public IDataFrame DropIrrelevantColumns(IDataFrame input)
        {
            var df = input.Clone();
            var toDrop = new List<string>();
            int totalRows = df.RowCount;

            foreach (var col in df.GetColumnNames())
            {
                var values = df.GetColumnValues(col).Where(v => v != null).ToList();

                // Drop based on missing values ratio
                double missingRatio = (double)(totalRows - values.Count) / totalRows;
                if (missingRatio > missingThreshold)
                {
                    Console.WriteLine($"[Dropper] Dropping column (missing ratio {missingRatio:F2}): {col}");
                    toDrop.Add(col);
                    continue;
                }

                // Drop based on low variance
                if (IsNumeric(values))
                {
                    var doubles = values.Select(v => Convert.ToDouble(v)).ToList();
                    double mean = doubles.Average();
                    double variance = doubles.Select(v => Math.Pow(v - mean, 2)).Average();

                    if (variance < varianceThreshold)
                    {
                        Console.WriteLine($"[Dropper] Dropping low variance column: {col}");
                        toDrop.Add(col);
                        continue;
                    }
                }

                // Drop ID-like columns based on high uniqueness
                double uniqueRatio = (double)values.Distinct().Count() / totalRows;
                if (uniqueRatio >= uniquenessThreshold)
                {
                    Console.WriteLine($"[Dropper] Dropping ID-like column (unique ratio {uniqueRatio:F2}): {col}");
                    toDrop.Add(col);
                }
            }

            // Actually remove columns
            var internalTable = ((SmartDataFrame)df).GetInternalTable();
            foreach (var col in toDrop)
            {
                if (internalTable.Columns.Contains(col))
                    internalTable.Columns.Remove(col);
            }

            return new SmartDataFrame(internalTable);
        }

        private bool IsNumeric(IEnumerable<object> values)
        {
            return values.All(v => double.TryParse(v?.ToString(), out _));
        }
    }
}