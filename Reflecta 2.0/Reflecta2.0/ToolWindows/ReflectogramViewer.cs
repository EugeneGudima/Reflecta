using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reflecta2._0
{
    public partial class ReflectogramViewer : Form
    {
        private Color[] reflcolors = { Color.Maroon, Color.DarkGray, Color.Chocolate, Color.MediumSeaGreen, Color.Teal, Color.Navy, Color.DarkViolet, Color.HotPink, Color.Black };

        public ReflectogramViewer(ZedGraph.PointPairList reflectogram, DateTime date, double temperature)
        {
            InitializeComponent();

            ALayout.SetReflectogramZedgraphStyle(refl_graph);

            ZedGraph.LineItem myCurve = refl_graph.GraphPane.AddCurve(date.ToString("dd.MM.yy HH:mm:ss.ff") + " - T=" + Math.Round(temperature, 1).ToString() + " °C",
                       reflectogram, SystemColors.HotTrack, ZedGraph.SymbolType.None);            

            myCurve.Line.IsSmooth = false;

            refl_graph.AxisChange();
            // Make sure the Graph gets redrawn
            refl_graph.Invalidate();
        }

        internal void AddReflectogram(ZedGraph.PointPairList reflectogram, DateTime date, double temperature)
        {
            if (refl_graph.GraphPane.CurveList.Count > 10)
            {
                refl_graph.GraphPane.CurveList.Clear();                
            }
            ZedGraph.LineItem myCurve;
            if (refl_graph.GraphPane.CurveList.Count == 0)
            {
                myCurve = refl_graph.GraphPane.AddCurve(date.ToString("dd.MM.yy HH:mm:ss.ff") + " - T=" + Math.Round(temperature, 1).ToString() + " °C",
                       reflectogram, SystemColors.HotTrack, ZedGraph.SymbolType.None);
            }
            else
            {
                myCurve = refl_graph.GraphPane.AddCurve(date.ToString("dd.MM.yy HH:mm:ss.ff") + " - T=" + Math.Round(temperature, 1).ToString() + " °C",
                           reflectogram, reflcolors[refl_graph.GraphPane.CurveList.Count - 1], ZedGraph.SymbolType.None);
            }

            myCurve.Line.IsSmooth = false;

            refl_graph.AxisChange();
            // Make sure the Graph gets redrawn
            refl_graph.Invalidate();
        }
    }
}
