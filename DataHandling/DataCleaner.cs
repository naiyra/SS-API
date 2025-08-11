using SS_API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS_API.DataHandling
{
    public class DataCleaner : IDataCleaner
    {
        public IDataFrame Clean(IDataFrame rawInput)
        {
            var df = rawInput.Clone();
            int totalRows = df.RowCount;

            var smartDf = (SmartDataFrame)df;
            var internalTable = smartDf.GetInternalTable();

           
            foreach (var col in df.GetColumnNames().ToList())
            {
                var unique = df.GetColumnValues(col).Where(v => v != null).Distinct().ToList();
                if (unique.Count == 1)
                {
                    Console.WriteLine($"[Cleaner] Dropping constant column: {col}");
                    smartDf.DropColumn(col);
                }
            }

            // Drop high uniqueness (ID-like) columns
            foreach (var col in df.GetColumnNames().ToList())
            {
                var values = df.GetColumnValues(col).Where(v => v != null).Select(v => v.ToString()).ToList();
                double uniqueRatio = (double)values.Distinct().Count() / totalRows;

                if (uniqueRatio >= 0.95)
                {
                    Console.WriteLine($"[Cleaner] Dropping ID-like column: {col}");
                    smartDf.DropColumn(col);
                }
            }

            
            var rowsToRemove = new HashSet<int>();

            foreach (var col in df.GetColumnNames())
            {
                var values = df.GetColumnValues(col).ToList(); 

                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i] == null)
                    {
                        rowsToRemove.Add(i); 
                    }
                }
            }

            if (rowsToRemove.Count > 0)
            {
                var orderedRows = rowsToRemove.OrderByDescending(x => x).ToList();
                foreach (var idx in orderedRows)
                {
                    internalTable.Rows.RemoveAt(idx);
                }
                Console.WriteLine($"[Cleaner] Dropped {rowsToRemove.Count} rows with missing (null) values.");
            }


            // Fill missing values (for safety — almost unnecessary after dropping nulls)
            foreach (var col in df.GetColumnNames())
            {
                if (df.GetColumnType(col) == "numeric")
                {
                    var nonNulls = df.GetColumnValues(col).Where(v => v != null).Select(v => Convert.ToDouble(v)).ToList();
                    if (nonNulls.Count == 0) continue;

                    double median = nonNulls.OrderBy(x => x).ElementAt(nonNulls.Count / 2);

                    var filled = df.GetColumnValues(col)
                                   .Select(v => v ?? (object)median)
                                   .ToList();

                    smartDf.SetColumnValues(col, filled);
                }
                else if (df.GetColumnType(col) == "categorical")
                {
                    var nonNulls = df.GetColumnValues(col).Where(v => v != null).Select(v => v.ToString()).ToList();
                    if (nonNulls.Count == 0) continue;

                    string mode = nonNulls.GroupBy(x => x).OrderByDescending(g => g.Count()).First().Key;

                    var filled = df.GetColumnValues(col)
                                   .Select(v => v ?? (object)mode)
                                   .ToList();

                    smartDf.SetColumnValues(col, filled);
                }
            }

            // Fix simple typos ( dictionary-based)
            var typoCorrections = new Dictionary<string, string>
            {
                {"Femael", "Female"},
                {"Mlae", "Male"},
                
            };

            foreach (var col in df.GetColumnNames())
            {
                if (df.GetColumnType(col) == "categorical")
                {
                    var corrected = df.GetColumnValues(col)
                                      .Select(v => v != null && typoCorrections.ContainsKey(v.ToString()) ? typoCorrections[v.ToString()] : v)
                                      .ToList();
                    smartDf.SetColumnValues(col, corrected);
                }
            }

            // Deduplicate
            internalTable = internalTable.DefaultView.ToTable(true);
            Console.WriteLine($"[Cleaner] Deduplication complete. Remaining rows: {internalTable.Rows.Count}");

            // Handle numeric outliers (capping using z-score)
            foreach (var col in df.GetColumnNames())
            {
                if (df.GetColumnType(col) == "numeric")
                {
                    var vals = df.GetColumnValues(col).Where(v => v != null).Select(v => Convert.ToDouble(v)).ToList();
                    if (vals.Count == 0) continue;

                    double mean = vals.Average();
                    double std = Math.Sqrt(vals.Sum(x => Math.Pow(x - mean, 2)) / vals.Count);

                    var capped = vals.Select(v =>
                        v > mean + 3 * std ? mean + 3 * std :
                        v < mean - 3 * std ? mean - 3 * std :
                        v
                    ).Cast<object>().ToList();

                    smartDf.SetColumnValues(col, capped);
                }
            }

            return new SmartDataFrame(internalTable);
        }
    }
}
