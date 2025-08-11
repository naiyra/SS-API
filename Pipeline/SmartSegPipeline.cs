using SS_API.Core;
using SS_API.DataHandling;
using SS_API.FeaturesHandling;
using SS_API.MachineLearning;
using SS_API.Reporting;
using System;
using System.Collections.Generic;

namespace SS_API.Pipeline
{
    public class SmartSegPipeline
    {
        private readonly IDataCleaner cleaner;
        private readonly IFeaturesEngineering handling;
        private readonly IClusterer clusterer;
        private readonly ISegmentInterpreter interpreter;

        public SmartSegPipeline()
        {
            cleaner = new DataCleaner();
            handling = new FeaturesEngineering();
            clusterer = new Clusterer();
            interpreter = new SegmentInterpreter();
        }

        public async Task<List<SegmentSummary>> Run(IDataFrame rawData)
        {
            Console.WriteLine("[DEBUG] Starting SmartSeg pipeline...");

            // Step 1: Data Cleaning
            var cleaned = cleaner.Clean(rawData);
            Console.WriteLine($"[DEBUG] Cleaned columns: {string.Join(", ", cleaned.GetColumnNames())}");

            // Step 2: Feature Engineering
            var context = new FeatureEngineeringContext();
            var features = handling.Transform(cleaned, context);
            Console.WriteLine($"[DEBUG] Engineered feature columns: {string.Join(", ", features.GetColumnNames())}");

            // Step 3: Clustering
            var clustered = clusterer.Cluster(features);
            Console.WriteLine($"[DEBUG] Completed clustering. Cluster assignments: {clustered.ClusterAssignments.Count}");

            // Step 4: Segment Interpretation (now using async/await)
            var segments = await interpreter.Interpret(clustered, context);
            Console.WriteLine($"[DEBUG] Generated {segments.Count} segment summaries.");

            return segments;
        }

    }
}
