using SS_API.FeaturesHandling;
using SS_API.MachineLearning;
using SS_API.Reporting;
using SS_API.Utils;
using SS_API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

public class SegmentInterpreter : ISegmentInterpreter
{
    private readonly OpenRouterLabelGenerator labelGenerator;

    public SegmentInterpreter()
    {
        labelGenerator = new OpenRouterLabelGenerator();
    }

    public async Task<List<SegmentSummary>> Interpret(ClusteredData clusteredData, FeatureEngineeringContext context)
    {
        Console.WriteLine("[DEBUG] Interpreting segments...");

        var segments = new List<SegmentSummary>();
        var clusterGroups = clusteredData.ClusterAssignments
            .Select((clusterId, index) => new { Index = index, ClusterId = clusterId })
            .GroupBy(x => x.ClusterId);

        foreach (var group in clusterGroups)
        {
            var clusterId = group.Key;
            var indices = group.Select(x => x.Index).ToList();
            var featureSummary = new Dictionary<string, object>();

            foreach (var col in clusteredData.Features.GetColumnNames())
            {
                if (context.CategoricalColumns.Contains(col))
                {
                    var counts = new Dictionary<string, int>();
                    foreach (var idx in indices)
                    {
                        var rawValue = clusteredData.Features.GetValue(idx, col);
                        var decoded = DecodeCategorical(context, col, rawValue);
                        if (!counts.ContainsKey(decoded))
                            counts[decoded] = 0;
                        counts[decoded]++;
                    }
                    var mostCommon = counts.OrderByDescending(x => x.Value).First().Key;
                    featureSummary[col] = mostCommon;
                }
                else
                {
                    double sum = 0;
                    int count = 0;
                    foreach (var idx in indices)
                    {
                        var value = clusteredData.Features.GetValue(idx, col);
                        if (value == null) continue;
                        sum += Convert.ToDouble(value);
                        count++;
                    }

                    if (count > 0)
                    {
                        double avgNormalized = sum / count;
                        if (context.NumericRanges.TryGetValue(col, out var range))
                        {
                            double rescaled = (avgNormalized * (range.Max - range.Min)) + range.Min;
                            if (col == "Age")
                            {
                                var ageRange = GetAgeRange(rescaled);
                                featureSummary["Age Range"] = ageRange;
                            }
                            featureSummary[col] = Math.Round(rescaled, 1);

                        }
                        else
                        {
                            featureSummary[col] = Math.Round(avgNormalized, 2);
                        }
                    }
                    else
                    {
                        featureSummary[col] = null;
                    }
                }
            }

            string label = await labelGenerator.GenerateLabelAsync(featureSummary);

            segments.Add(new SegmentSummary
            {
                ClusterId = clusterId,
                MemberCount = indices.Count,
                FeatureSummary = featureSummary,
                Description = label
            });

            Console.WriteLine($"[DEBUG] Segment {clusterId}: Members={indices.Count}");
        }

        return segments;
    }
    private string GetAgeRange(double averageAge)
    {
        int lower = (int)Math.Floor(averageAge - 5);
        int upper = (int)Math.Ceiling(averageAge + 5);
        return $"{lower}–{upper}";
    }

    private string DecodeCategorical(FeatureEngineeringContext context, string columnName, object encodedValue)
    {
        if (encodedValue == null || !int.TryParse(encodedValue.ToString(), out int code))
            return "Unknown";

        if (context.Decoders.TryGetValue(columnName, out var mapping))
        {
            return mapping.TryGetValue(code, out var decoded) ? decoded : "Unknown";
        }

        return "Unknown";
    }
}



