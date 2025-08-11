using SS_API.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SS_API.MachineLearning
{
    public class KMeans
    {
        private readonly int k;
        private readonly int maxIterations;
        private readonly Random random;

        public KMeans(int k, int maxIterations = 100)
        {
            this.k = k;
            this.maxIterations = maxIterations;
            this.random = new Random();
        }

        public List<int> FitPredict(IDataFrame features)
        {
            int n = features.RowCount;
            var data = Enumerable.Range(0, n).Select(i => features.GetRow(i)).ToList();

            // Randomly initialize centroids
            var centroids = data.OrderBy(_ => random.Next()).Take(k).Select(row => row.ToList()).ToList();
            var assignments = new List<int>(new int[n]);

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                bool changed = false;

                // ssign clusters
                for (int i = 0; i < n; i++)
                {
                    var distances = centroids.Select(c => EuclideanDistance(data[i], c)).ToList();
                    int newCluster = distances.IndexOf(distances.Min());

                    if (assignments[i] != newCluster)
                    {
                        assignments[i] = newCluster;
                        changed = true;
                    }
                }

                //  Update centroids
                for (int cluster = 0; cluster < k; cluster++)
                {
                    var members = Enumerable.Range(0, n)
                        .Where(i => assignments[i] == cluster)
                        .Select(i => data[i])
                        .ToList();

                    if (members.Count > 0)
                    {
                        for (int dim = 0; dim < members[0].Count; dim++)
                        {
                            centroids[cluster][dim] = members.Average(m => Convert.ToDouble(m[dim]));
                        }
                    }
                }

                if (!changed)
                    break; // Converged
            }

            return assignments;
        }

        private double EuclideanDistance(List<object> a, List<object> b)
        {
            double sum = 0;
            for (int i = 0; i < a.Count; i++)
            {
                double valA = Convert.ToDouble(a[i]);
                double valB = Convert.ToDouble(b[i]);
                sum += Math.Pow(valA - valB, 2);
            }
            return Math.Sqrt(sum);
        }
    }
}
