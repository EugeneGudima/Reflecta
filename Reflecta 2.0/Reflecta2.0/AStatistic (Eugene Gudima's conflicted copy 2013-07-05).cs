using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reflecta2._0
{
    public class AStatistic
    {
        public static double StaticDeviation(ZedGraph.PointPairList pointPair)
        {
            double[] data = GetDatafromPointPairList(pointPair);
            if (data != null)
            {
                double sum = 0.0;
                double average = data.Average();
                foreach (double point in data)
                {
                    sum += Math.Pow(point - average, 2);
                }
                return Math.Sqrt(sum / data.Length);
            }
            else
                return -1.0;
        }

        public static double StaticDeviation(double[] data)
        {
            if (data != null)
            {
                double sum = 0.0;
                double average = data.Average();
                foreach (double point in data)
                {
                    sum += Math.Pow(point - average, 2);
                }
                return Math.Sqrt(sum / data.Length);
            }
            else
                return -1.0;
        }

        public static double StaticDeviation(int[] data)
        {
            if (data != null)
            {
                double sum = 0.0;
                foreach (int point in data)
                {
                    sum += point - data.Average();
                }
                return Math.Sqrt(sum / data.Length);
            }
            else
                return -1.0;
        }


        public static double[] GetDatafromPointPairList(ZedGraph.PointPairList data)
        {
            if (data != null)
            {
                List<double> result = new List<double>();
                foreach (ZedGraph.PointPair point in data)
                    result.Add(point.Y);
                return result.ToArray();
            }
            else
                return null;
        }

        internal static double[] GetDatafromPointPairList(ZedGraph.PointPairList data, double begin, double end)
        {
            if (data != null)
            {
                List<double> result = new List<double>();
                foreach (ZedGraph.PointPair point in data)
                    if (point.X > begin && point.X < end)
                        result.Add(point.Y);
                return result.ToArray();
            }
            else
                return null;
        }
    }
}
