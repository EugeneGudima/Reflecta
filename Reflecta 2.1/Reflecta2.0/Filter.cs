using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reflecta2._0
{
    

    class Filter
    {
        public static void FilterData(ZedGraph.PointPairList data, ref ZedGraph.PointPairList filter, double speed_max, double alfa_max, double alfa_null, int periodInMin)
        {
            if (data != null)
            {
                if (filter == null || filter.Count < 2)
                {
                    filter = new ZedGraph.PointPairList();
                    filter.Add(data[0]);
                    filter.Add(data[1]);
                }

                ZedGraph.PointPairList filterPoints = new ZedGraph.PointPairList();


                for (int i = filter.Count; i < data.Count; i++)
                {
                    filterPoints.Clear();
                    ZedGraph.PointPair notFilteredMeasure = data[i];
                    for (int j = filter.Count-1; j > 0 ; j--)
                    {
                        if (DateTime.FromOADate(notFilteredMeasure.X).Subtract(new TimeSpan(0, periodInMin, 0)) < DateTime.FromOADate(filter[j].X))
                        {
                            filterPoints.Add(filter[j]);
                        }
                        else
                            break;
                    }
                    double midX = filterPoints.Average(value => value.X);
                    double midY = filterPoints.Average(value => value.Y);
                    double alfa = 0.0;
                    double nominalSpeed = speed_max / 2.0;
                    //raschet skorosti
                    TimeSpan interval = DateTime.FromOADate(notFilteredMeasure.X) - DateTime.FromOADate(midX);
                    double speed = Math.Abs(notFilteredMeasure.Y - midY) / (0.5 * interval.TotalSeconds);
                    //alfa ot v
                    if (speed < nominalSpeed)
                    {
                        alfa = (alfa_max - alfa_null) * speed / nominalSpeed + alfa_null;
                    }
                    else if (speed >= nominalSpeed && speed < speed_max)
                    {
                        alfa = alfa_max;
                    }
                    else if (speed >= speed_max && speed < 2 * speed_max)
                    {
                        alfa = 2 - speed / speed_max;
                    }
                    else if (speed > 2 * speed_max)
                    {
                        alfa = 0;
                    }

                    //vichislenie skorrekt urovnia
                    double skor_uroven = notFilteredMeasure.Y * alfa + (1 - alfa) * midY;
                    //skor_uroven = Math.Round(skor_uroven, 2);
                    //vichislenie skorrekt urovnia            

                    filter.Add(new ZedGraph.PointPair(notFilteredMeasure.X, skor_uroven));
                }
                if (filter.Count > 3)
                {
                    filter[0].Y = filter[2].Y;
                    filter[1].Y = filter[2].Y;
                }
            }
        }
    }
}
