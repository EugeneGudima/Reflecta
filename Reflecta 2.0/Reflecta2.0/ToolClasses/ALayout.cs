using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace Reflecta2._0
{
    class ALayout
    {
        public static void SetStatisticZedgraphStyle(ZedGraphControl zedGraph)
        {
            GraphPane graphpane = zedGraph.GraphPane;
            graphpane.Border.Color = SystemColors.Control;
            graphpane.Fill.Color = SystemColors.Control;            
            graphpane.IsFontsScaled = false;
            graphpane.Legend.IsVisible = false;

            // Set the titles and axis labels
            graphpane.Title.Text = "Статистика";
            graphpane.XAxis.Title.Text = "Время";

            // Show the x axis grid
            graphpane.XAxis.MajorGrid.IsVisible = true;
            graphpane.XAxis.Scale.MaxAuto = true;
            graphpane.XAxis.Scale.MinAuto = true;
            graphpane.XAxis.Type = AxisType.Date;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            graphpane.YAxis.MajorTic.IsOpposite = false;
            graphpane.YAxis.MinorTic.IsOpposite = false;
            // Don't display the Y zero line
            graphpane.YAxis.MajorGrid.IsZeroLine = false;
            // Align the Y axis labels so they are flush to the axis
            graphpane.YAxis.Scale.Align = AlignP.Inside;
            graphpane.YAxis.Scale.MaxAuto = true;
            graphpane.YAxis.Scale.MinAuto = true;
            graphpane.IsBoundedRanges = true;

            //graphpane.Y2Axis.IsVisible = true;
            //graphpane.Y2Axis.MajorTic.IsOpposite = false;
            //graphpane.Y2Axis.MinorTic.IsOpposite = false;
            //// Don't display the Y zero line
            //graphpane.Y2Axis.MajorGrid.IsZeroLine = false;
            //// Align the Y axis labels so they are flush to the axis
            //graphpane.Y2Axis.Scale.Align = AlignP.Inside;
            //graphpane.Y2Axis.Scale.MaxAuto = true;
            //graphpane.Y2Axis.Scale.MinAuto = true;

            // Manually set the axis range            

            // Fill the axis background with a gradient
            graphpane.Chart.Fill = new Fill(Color.White, Color.LightGray, 45.0f);


            // Enable scrollbars if needed            
            zedGraph.IsAutoScrollRange = true;
            zedGraph.IsScrollY2 = true;

            // OPTIONAL: Show tooltips when the mouse hovers over a point
            zedGraph.IsShowPointValues = true;            

            graphpane.YAxisList.Clear();
            graphpane.Y2AxisList.Clear();

            // Создадим три новых оси Y
            // Метод AddYAxis() возвращает индекс новой оси в списке осей (YAxisList)
            int axis1 = graphpane.AddYAxis("Axis 1");
            int axis2 = graphpane.AddYAxis("Axis 2");
            int axis3 = graphpane.AddY2Axis("Axis 3");
            int axis4 = graphpane.AddY2Axis("Axis 4");

            graphpane.YAxisList[axis1].Title.FontSpec.FontColor = SystemColors.HotTrack;           
            graphpane.YAxisList[axis2].Title.FontSpec.FontColor = Color.MediumSeaGreen;
            graphpane.Y2AxisList[axis3].Title.FontSpec.FontColor = Color.Crimson;
            graphpane.Y2AxisList[axis4].Title.FontSpec.FontColor = Color.OrangeRed;
            graphpane.Y2AxisList[axis3].IsVisible = false;
            graphpane.Y2AxisList[axis4].IsVisible = false;
            graphpane.YAxisList[axis1].IsVisible = false;
            graphpane.YAxisList[axis2].IsVisible = false;

            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            zedGraph.AxisChange();
        }

        public static void SetReflectogramZedgraphStyle(ZedGraphControl zedGraph)
        {
            GraphPane graphpane = zedGraph.GraphPane;
            graphpane.Border.Color = SystemColors.Control;
            graphpane.Fill.Color = SystemColors.Control;
            graphpane.IsFontsScaled = false;

            // Set the titles and axis labels
            graphpane.Title.Text = "Рефлектограмма";
            graphpane.XAxis.Title.Text = "Дискрет вермени";
            graphpane.YAxis.Title.Text = "U, ед. АЦП";

            // Show the x axis grid
            graphpane.XAxis.MajorGrid.IsVisible = true;
            graphpane.XAxis.Scale.Max = 4100;

            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            graphpane.YAxis.MajorTic.IsOpposite = false;
            graphpane.YAxis.MinorTic.IsOpposite = false;
            // Don't display the Y zero line
            graphpane.YAxis.MajorGrid.IsZeroLine = false;
            // Align the Y axis labels so they are flush to the axis
            graphpane.YAxis.Scale.Align = AlignP.Inside;
            // Manually set the axis range            

            // Fill the axis background with a gradient
            graphpane.Chart.Fill = new Fill(Color.White, Color.LightGray, 45.0f);
            graphpane.IsBoundedRanges = true;


            // Enable scrollbars if needed            
            zedGraph.IsAutoScrollRange = true;
            zedGraph.IsScrollY2 = true;


            // OPTIONAL: Show tooltips when the mouse hovers over a point
            zedGraph.IsShowPointValues = true;

            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            zedGraph.AxisChange();
        }

        internal static void SetChannelsCheckboxList(CheckedListBox channels, Dictionary<string, List<byte>> devices)
        {
            channels.Items.Clear();            
            foreach (KeyValuePair<string, List<byte>> baseblock in devices)
            {
                foreach (byte channel in baseblock.Value)
                {
                    string text = baseblock.Key + " - Канал " + channel.ToString();
                    channels.Items.Add(text, true);                    
                }                
            }           
        }

        internal static void SetChannelsComboBox(CheckedListBox channels, ComboBox combo1, ComboBox combo2)
        {           
            combo1.Items.Clear();
            combo2.Items.Clear();

            combo1.Items.Add("- - -");
            combo2.Items.Add("- - -");
            foreach (object record in channels.CheckedItems)
            {
                combo1.Items.Add(record.ToString());
                combo2.Items.Add(record.ToString());                
            }
            if(combo1.Items.Count > 1)
                combo1.SelectedIndex = 1;
            else
                combo1.SelectedIndex = 0;
            combo2.SelectedIndex = 0;
        }

        internal static void SetChannelsComboBox(List<string> channels, ComboBox combo1, ComboBox combo2)
        {
            combo1.Items.Clear();
            combo2.Items.Clear();

            combo1.Items.Add("- - -");
            combo2.Items.Add("- - -");
            foreach (string record in channels)
            {
                combo1.Items.Add(record);
                combo2.Items.Add(record);
            }
            if (combo1.Items.Count > 1)
                combo1.SelectedIndex = 1;
            else
                combo1.SelectedIndex = 0;
            combo2.SelectedIndex = 0;
        }

        internal static Dictionary<string, List<byte>> GetDevicesForExperiment(CheckedListBox channels)
        {
            Dictionary<string, List<byte>> result = new Dictionary<string, List<byte>>();
            List<byte> channelsForComport = new List<byte>();
            string previousComport = "";
            string comport = "";
            for (int i = 0; i < channels.CheckedItems.Count; i++)
            {
                string device = (string)channels.CheckedItems[i];                
                int comEnd = device.IndexOf(')')+1;
                int channelStart = device.IndexOf("Канал ") + 6;
                comport = device.Substring(0, comEnd);
                byte channel = Convert.ToByte(device.Substring(channelStart));
                if (i != 0)
                {
                    if (previousComport != comport)
                    {
                        result.Add(previousComport, channelsForComport);
                        channelsForComport = new List<byte>();
                        channelsForComport.Add(channel);
                    }
                    else
                    {
                        channelsForComport.Add(channel);
                    }
                }
                else
                    channelsForComport.Add(channel);
                previousComport = comport;   
            }
            if (channelsForComport.Count > 0)
            {                
                result.Add(comport, channelsForComport);
            }
            return result;
        }

        internal static void SetStatisticParametersListbox(ListBox allListbox, ListBox toShowListbox, List<string> list)
        {
            foreach (string element in list)
            {
                if (element.Contains("уровень (датч.)"))
                    toShowListbox.Items.Add(element);
                else
                    allListbox.Items.Add(element);
            }
        }

        internal static KeyValuePair<string, byte> GetChannelForDisplay(ComboBox channelSelect1_combobox)
        {
            KeyValuePair<string, byte> result = new KeyValuePair<string, byte>();
            if (channelSelect1_combobox.SelectedItem.ToString() != "- - -")
            {
                string device = (string)channelSelect1_combobox.SelectedItem.ToString();               
                int comEnd = device.IndexOf(')')+1;
                int channelStart = device.IndexOf("Канал ") + 6;
                string comport = device.Substring(0, comEnd);
                byte channel = Convert.ToByte(device.Substring(channelStart));
                if (comport != null)
                {
                    result = new KeyValuePair<string, byte>(comport, channel);
                }
                return result;
            }
            else
                return result;
        }

        internal static void SetCalibrationDatagrid(DataGridView calibParams_dataGridView, Dictionary<string, List<byte>> devices)
        {
            calibParams_dataGridView.Rows.Clear();
            foreach (KeyValuePair<string, List<byte>> baseblock in devices)
            {
                foreach (byte channel in baseblock.Value)
                {
                    string text = baseblock.Key + " - Канал " + channel.ToString();
                    calibParams_dataGridView.Rows.Add(text, null);
                }
            }      
        }
    }
}
