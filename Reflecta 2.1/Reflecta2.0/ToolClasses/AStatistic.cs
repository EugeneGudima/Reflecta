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
            try
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
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "AStatistic", "StaticDeviation", ex.Message);
                return -1.0;
            }
        }

        public static double StaticDeviation(double[] data)
        {
            try
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
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "AStatistic", "StaticDeviation", ex.Message);
                return -1.0;
            }
        }

        public static double StaticDeviation(int[] data)
        {
            try
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
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "AStatistic", "StaticDeviation", ex.Message);
                return -1.0;
            }
        }


        public static double[] GetDatafromPointPairList(ZedGraph.PointPairList data)
        {
            try
            {
                if (data != null)
                {
                    List<double> result = new List<double>();
                    for (int i = 0; i < data.Count; i++)
                    {
                        result.Add(data[i].Y);
                    }
                    return result.ToArray();
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "AStatistic", "GetDatafromPointPairList", ex.Message);
                return null;
            }
        }

        internal static double[] GetDatafromPointPairList(ZedGraph.PointPairList data, double begin, double end)
        {
            try
            {
                if (data != null)
                {
                    List<double> result = new List<double>();
                    for(int i = 0; i < data.Count; i++)
                    {
                        if (data[i].X > begin && data[i].X < end)
                            result.Add(data[i].Y);
                    }
                    return result.ToArray();
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "AStatistic", "GetDatafromPointPairList", ex.Message);
                return null;
            }
        }

        internal static double Correlation(double[] parameter1, double[] parameter2)
        {
            try
            {
                if (parameter1 != null && parameter2 != null && parameter1.Length == parameter2.Length)
                {
                    double sum1 = 0, sum2 = 0, sum3 = 0;
                    double param1Avg = parameter1.Average(), param2Avg = parameter2.Average();
                    for (int i = 0; i < parameter1.Length; i++)
                    {
                        sum1 += (parameter1[i] - param1Avg) * (parameter2[i] - param2Avg);
                        sum2 += Math.Pow(parameter1[i] - param1Avg, 2);
                        sum3 += Math.Pow(parameter2[i] - param2Avg, 2);
                    }
                    return sum1 / Math.Sqrt(sum2 * sum3);
                }
                else
                    return -1.0;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "AStatistic", "Correlation", ex.Message);
                return -1.0;
            }
        }
    }
}
