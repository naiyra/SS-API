using SS_API.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SS_API.MachineLearning.Utils
{
    public static class SilhouetteScore
    {
        public static double Calculate(IDataFrame features, List<int> labels)
        {
            int n = features.RowCount;
            if (n <= 1) return 0;

            double totalScore = 0;
            for (int i = 0; i < n; i++)
            {
                double a = AverageDistance(features, i, labels[i], labels);
                double b = labels.Distinct().Where(c => c != labels[i])
                                .Select(c => AverageDistance(features, i, c, labels))
                                .Min();

                double s = (b - a) / Math.Max(a, b);
                totalScore += s;
            }
            return totalScore / n;
        }

        private static double AverageDistance(IDataFrame features, int index, int cluster, List<int> labels)
        {
            var indices = labels.Select((c, idx) => (c, idx))
                                 .Where(x => x.c == cluster)
                                 .Select(x => x.idx)
                                 .ToList();

            if (indices.Count == 0) return 0;

            double total = 0;
            foreach (var idx in indices)
            {
                total += EuclideanDistance(features.GetRow(index), features.GetRow(idx));
            }
            return total / indices.Count;
        }

        private static double EuclideanDistance(List<object> a, List<object> b)
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
