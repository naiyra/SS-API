using SS_API.Core;
using SS_API.MachineLearning.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SS_API.MachineLearning
{
    public class Clusterer : IClusterer
    {
        public ClusteredData Cluster(IDataFrame features)
        {
            Console.WriteLine("[DEBUG] Starting clustering...");

            int bestK = FindBestK(features, 2, 10); // or (2, 5) if you want smaller range

            Console.WriteLine($"[DEBUG] Optimal number of clusters selected: {bestK}");

            var kmeans = new KMeans(k: bestK);
            var assignments = kmeans.FitPredict(features);

            Console.WriteLine($"[DEBUG] Clustering completed. Total assignments: {assignments.Count}");

            return new ClusteredData
            {
                Features = features,
                ClusterAssignments = assignments
            };
        }

        private int FindBestK(IDataFrame features, int minK, int maxK)
        {
            var scores = new Dictionary<int, double>();

            for (int k = minK; k <= maxK; k++)
            {
                var kmeans = new KMeans(k);
                var assignments = kmeans.FitPredict(features);
                double score = SilhouetteScore.Calculate(features, assignments);

                scores[k] = score;
                Console.WriteLine($"[DEBUG] K={k}: Silhouette={score:F4}");

                // Smart cutoff:
                if (k > 5 && score < 0.45)
                {
                    Console.WriteLine($"[DEBUG] Stopping early at K={k} due to weak silhouette.");
                    break;
                }
            }

            return scores.OrderByDescending(x => x.Value).First().Key;
        }


        private double CalculateSSE(IDataFrame features, List<int> labels)
        {
            int n = features.RowCount;
            var centroids = new Dictionary<int, List<double>>();
            var clusterCounts = new Dictionary<int, int>();

            // Calculate centroids
            for (int i = 0; i < n; i++)
            {
                var row = features.GetRow(i).Select(x => Convert.ToDouble(x)).ToList();
                int label = labels[i];

                if (!centroids.ContainsKey(label))
                {
                    centroids[label] = new List<double>(row);
                    clusterCounts[label] = 1;
                }
                else
                {
                    for (int j = 0; j < row.Count; j++)
                        centroids[label][j] += row[j];
                    clusterCounts[label]++;
                }
            }

            foreach (var label in centroids.Keys.ToList())
            {
                for (int j = 0; j < centroids[label].Count; j++)
                    centroids[label][j] /= clusterCounts[label];
            }

            // Calculate SSE
            double totalSSE = 0;
            for (int i = 0; i < n; i++)
            {
                var row = features.GetRow(i).Select(x => Convert.ToDouble(x)).ToList();
                int label = labels[i];
                var centroid = centroids[label];

                double sum = 0;
                for (int j = 0; j < row.Count; j++)
                    sum += Math.Pow(row[j] - centroid[j], 2);

                totalSSE += sum;
            }

            return totalSSE;
        }

        private int DetectElbow(Dictionary<int, double> sseScores)
        {
            var points = sseScores.Select(kv => (k: kv.Key, sse: kv.Value)).OrderBy(x => x.k).ToList();

            // Line from first to last point
            var first = points.First();
            var last = points.Last();

            double maxDistance = double.MinValue;
            int elbowK = first.k;

            foreach (var point in points)
            {
                double distance = PerpendicularDistance(first, last, point);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    elbowK = point.k;
                }
            }

            return elbowK;
        }

        private double PerpendicularDistance((int k, double sse) lineStart, (int k, double sse) lineEnd, (int k, double sse) point)
        {
            double x0 = point.k, y0 = point.sse;
            double x1 = lineStart.k, y1 = lineStart.sse;
            double x2 = lineEnd.k, y2 = lineEnd.sse;

            double numerator = Math.Abs((y2 - y1) * x0 - (x2 - x1) * y0 + (x2 * y1 - y2 * x1));
            double denominator = Math.Sqrt(Math.Pow(y2 - y1, 2) + Math.Pow(x2 - x1, 2));

            return numerator / denominator;
        }
    }
}
