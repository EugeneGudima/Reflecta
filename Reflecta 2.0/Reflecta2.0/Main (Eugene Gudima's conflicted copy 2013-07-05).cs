using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace Reflecta2._0
{
    public partial class MainWindow : Form
    {
        Dictionary<string, List<byte>> allDevices = new Dictionary<string, List<byte>>();
        Dictionary<string, List<byte>> devicesForExperiment = new Dictionary<string, List<byte>>();
        Timer timer;
        KeyValuePair<string, byte> channel1, channel2;
        PointPairList channelOneVariableOne, channelOneVariableTwo, channelTwoVariableOne, channelTwoVariableTwo;
        List<ABaseblock> baseblockInfo = new List<ABaseblock>();
        int experimentGroup;
        List<AThread> threads = new List<AThread>();
        ADatabase db;
        private bool runningFlag = false;
        private bool afterInitialize = false;
        private bool realTimeRefl = true;
        ReflectogramViewer window;

        public MainWindow()
        {
            InitializeComponent();

            start_dateTimePicker.Value = DateTime.Now;
            end_dateTimePicker.Value = DateTime.Now;

            db = new ADatabase();
            if (!File.Exists("data\\database.fdb"))
            {
                db.CreateDB();
                db.Connect();
                db.CreateDBStructure();
                db.WriteParameters();
            }
            else
            {
                db.Connect();
            }

            ALayout.SetStatisticParametersListbox(allParameters_listbox, parametersToShow_listbox, ADatabaseWorker.GetAllParameters(db));
            ALayout.SetStatisticZedgraphStyle(statistic_graph);
            ALayout.SetReflectogramZedgraphStyle(refl_graph);

            ScanTasks();

        }

        private void FormLoad(object sender, EventArgs e)
        {
            //afterInitialize = true;

            timer = new Timer();
            timer.Interval = (int)_dbInterval_numericUpDown.Value * 1000;
            timer.Tick += timer_Tick;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            statistic_graph.GraphPane.CurveList.Clear();
            statistic_graph.GraphPane.GraphObjList.Clear();
            statistic_graph.Invalidate();

            if (realTimeRefl)
            {
                refl_graph.GraphPane.CurveList.Clear();
                refl_graph.GraphPane.GraphObjList.Clear();
                refl_graph.Invalidate();
            }

            #region ReflectogramTask
            if (channel1.Key != null )
            {
                reflChannel1_checkListBox.Visible = true;

                List<string> datetimeChannel1 = ADatabaseWorker.GetAllReflectograms(db, experimentGroup, channel1.Key, channel1.Value);
                int count = 0;
                foreach (string datetime in datetimeChannel1)
                {
                    if (ReflectogamIsNotAdded(reflChannel1_checkListBox, datetime))
                        reflChannel1_checkListBox.Items.Add(datetime, false);
                    else if(realTimeRefl)
                        reflChannel1_checkListBox.SetItemChecked(count, false);
                    count++;
                }
                if (reflChannel1_checkListBox.Items.Count != 0)
                {
                    if (realTimeRefl && reflChannel1_checkListBox.Items.Count > 0)
                        reflChannel1_checkListBox.SetItemChecked(reflChannel1_checkListBox.Items.Count - 1, true);
                    //else
                    //    reflChannel1_checkListBox.SetItemChecked(reflChannel1_checkListBox.Items.Count, true);
                }
            }
            else
            {
                reflChannel1_checkListBox.Items.Clear();
                reflChannel1_checkListBox.Visible = false;
            }

            if (channel2.Key != null)
            {
                reflChannel2_checkListBox.Visible = true;

                List<string> datetimeChannel2 = ADatabaseWorker.GetAllReflectograms(db, experimentGroup, channel2.Key, channel2.Value);
                int count = 0;
                foreach (string datetime in datetimeChannel2)
                {
                    if (ReflectogamIsNotAdded(reflChannel2_checkListBox, datetime))
                        reflChannel2_checkListBox.Items.Add(datetime, false);
                    else if (realTimeRefl)
                        reflChannel2_checkListBox.SetItemChecked(count, false);
                    count++;
                }
                if (reflChannel2_checkListBox.Items.Count != 0)
                {
                    if (realTimeRefl && reflChannel2_checkListBox.Items.Count > 0)
                        reflChannel2_checkListBox.SetItemChecked(reflChannel2_checkListBox.Items.Count - 1, true);
                    //else
                    //    reflChannel2_checkListBox.SetItemChecked(reflChannel2_checkListBox.Items.Count, true);
                }
            }
            else
            {
                reflChannel2_checkListBox.Items.Clear();
                reflChannel2_checkListBox.Visible = false;
            }
            #endregion

            #region First Channel Task
            if (channel1.Key != null && parametersToShow_listbox.Items.Count > 0)
            {
                switch (parametersToShow_listbox.Items.Count)
                {
                    case 1:
                        int parameterID = ADatabase.GetParameterID(parametersToShow_listbox.Items[0].ToString());
                        channelOneVariableOne = ADatabaseWorker.GetData(db, experimentGroup, channel1.Key, channel1.Value, parameterID);
                        

                        // Generate a red curve with diamond symbols, and "Alpha" in the legend
                        LineItem myCurve = statistic_graph.GraphPane.AddCurve(parametersToShow_listbox.Items[0].ToString(),
                            channelOneVariableOne, SystemColors.HotTrack, SymbolType.None);                        

                        myCurve.Line.IsSmooth = false;
                        myCurve.IsY2Axis = false;
                        myCurve.YAxisIndex = 0;
                        statistic_graph.GraphPane.YAxisList[0].IsVisible = true;
                        statistic_graph.GraphPane.Y2AxisList[0].IsVisible = false;
                        statistic_graph.GraphPane.YAxisList[0].Title.Text = parametersToShow_listbox.Items[0].ToString();
                        // Tell ZedGraph to calculate the axis ranges
                        // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
                        // up the proper scrolling parameters
                        //graphpane.CurveList.Reverse();
                        statistic_graph.AxisChange();
                        // Make sure the Graph gets redrawn
                        statistic_graph.Invalidate();
                        break;
                    case 2:
                        int parameterID1 = ADatabase.GetParameterID(parametersToShow_listbox.Items[0].ToString());
                        channelOneVariableOne = ADatabaseWorker.GetData(db, experimentGroup, channel1.Key, channel1.Value, parameterID1);                        

                        // Generate a red curve with diamond symbols, and "Alpha" in the legend
                        LineItem myCurve1 = statistic_graph.GraphPane.AddCurve(parametersToShow_listbox.Items[0].ToString(),
                            channelOneVariableOne, SystemColors.HotTrack, SymbolType.None);                        

                        myCurve1.Line.IsSmooth = false;
                        myCurve1.IsY2Axis = false;
                        myCurve1.YAxisIndex = 0;
                        statistic_graph.GraphPane.YAxisList[0].IsVisible = true;
                        statistic_graph.GraphPane.YAxisList[0].Title.Text = parametersToShow_listbox.Items[0].ToString();

                        int parameterID2 = ADatabase.GetParameterID(parametersToShow_listbox.Items[1].ToString());
                        channelOneVariableTwo = ADatabaseWorker.GetData(db, experimentGroup, channel1.Key, channel1.Value, parameterID2);
                       
                        // Generate a red curve with diamond symbols, and "Alpha" in the legend
                        LineItem myCurve2 = statistic_graph.GraphPane.AddCurve(parametersToShow_listbox.Items[1].ToString(),
                            channelOneVariableTwo, Color.Crimson , SymbolType.None);                        

                        myCurve2.Line.IsSmooth = false;
                        myCurve2.IsY2Axis = true;
                        myCurve1.YAxisIndex = 0;
                        statistic_graph.GraphPane.Y2AxisList[0].IsVisible = true;
                        statistic_graph.GraphPane.Y2AxisList[0].Title.Text = parametersToShow_listbox.Items[1].ToString();
                        // Tell ZedGraph to calculate the axis ranges
                        // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
                        // up the proper scrolling parameters
                        //graphpane.CurveList.Reverse();
                        statistic_graph.AxisChange();
                        // Make sure the Graph gets redrawn
                        statistic_graph.Invalidate();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                statistic_graph.GraphPane.YAxisList[0].IsVisible = false;
                statistic_graph.GraphPane.Y2AxisList[0].IsVisible = false;
            }
#endregion

            #region Second Channel Task
            if (channel2.Key != null && parametersToShow_listbox.Items.Count > 0)
            {
                switch (parametersToShow_listbox.Items.Count)
                {
                    case 1:
                        int parameterID = ADatabase.GetParameterID(parametersToShow_listbox.Items[0].ToString());
                        channelTwoVariableOne = ADatabaseWorker.GetData(db, experimentGroup, channel2.Key, channel2.Value, parameterID);

                        // Generate a red curve with diamond symbols, and "Alpha" in the legend
                        LineItem myCurve = statistic_graph.GraphPane.AddCurve(parametersToShow_listbox.Items[0].ToString(),
                            channelTwoVariableOne, Color.MediumSeaGreen, SymbolType.None);

                        myCurve.Line.IsSmooth = false;
                        myCurve.IsY2Axis = false;
                        myCurve.YAxisIndex = 1;
                        statistic_graph.GraphPane.YAxisList[1].IsVisible = true;
                        statistic_graph.GraphPane.Y2AxisList[1].IsVisible = false;
                        statistic_graph.GraphPane.YAxisList[1].Title.Text = parametersToShow_listbox.Items[0].ToString();
                        // Tell ZedGraph to calculate the axis ranges
                        // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
                        // up the proper scrolling parameters
                        //graphpane.CurveList.Reverse();
                        statistic_graph.AxisChange();
                        // Make sure the Graph gets redrawn
                        statistic_graph.Invalidate();
                        break;
                    case 2:
                        int parameterID1 = ADatabase.GetParameterID(parametersToShow_listbox.Items[0].ToString());
                        channelTwoVariableOne = ADatabaseWorker.GetData(db, experimentGroup, channel2.Key, channel2.Value, parameterID1);

                        // Generate a red curve with diamond symbols, and "Alpha" in the legend
                        LineItem myCurve1 = statistic_graph.GraphPane.AddCurve(parametersToShow_listbox.Items[0].ToString(),
                            channelTwoVariableOne, Color.MediumSeaGreen, SymbolType.None);

                        myCurve1.Line.IsSmooth = false;
                        myCurve1.IsY2Axis = false;
                        myCurve1.YAxisIndex = 1;
                        statistic_graph.GraphPane.YAxisList[1].IsVisible = true;
                        statistic_graph.GraphPane.YAxisList[1].Title.Text = parametersToShow_listbox.Items[0].ToString();

                        int parameterID2 = ADatabase.GetParameterID(parametersToShow_listbox.Items[1].ToString());
                        channelTwoVariableTwo = ADatabaseWorker.GetData(db, experimentGroup, channel2.Key, channel2.Value, parameterID2);

                        // Generate a red curve with diamond symbols, and "Alpha" in the legend
                        LineItem myCurve2 = statistic_graph.GraphPane.AddCurve(parametersToShow_listbox.Items[1].ToString(),
                            channelTwoVariableTwo, Color.DarkOrange, SymbolType.None);

                        myCurve2.Line.IsSmooth = false;
                        myCurve2.IsY2Axis = true;
                        myCurve2.YAxisIndex = 1;
                        statistic_graph.GraphPane.Y2AxisList[1].IsVisible = true;
                        statistic_graph.GraphPane.Y2AxisList[1].Title.Text = parametersToShow_listbox.Items[1].ToString();
                        // Tell ZedGraph to calculate the axis ranges
                        // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
                        // up the proper scrolling parameters
                        //graphpane.CurveList.Reverse();
                        statistic_graph.AxisChange();
                        // Make sure the Graph gets redrawn
                        statistic_graph.Invalidate();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                statistic_graph.GraphPane.YAxisList[1].IsVisible = false;
                statistic_graph.GraphPane.Y2AxisList[1].IsVisible = false;
            }
            #endregion

            #region Stats

            double[] channel1Var1 = AStatistic.GetDatafromPointPairList(channelOneVariableOne, statistic_graph.GraphPane.XAxis.Scale.Min, statistic_graph.GraphPane.XAxis.Scale.Max);
            double[] channel1Var2 = AStatistic.GetDatafromPointPairList(channelOneVariableTwo, statistic_graph.GraphPane.XAxis.Scale.Min, statistic_graph.GraphPane.XAxis.Scale.Max);
            double[] channel2Var1 = AStatistic.GetDatafromPointPairList(channelTwoVariableOne, statistic_graph.GraphPane.XAxis.Scale.Min, statistic_graph.GraphPane.XAxis.Scale.Max);
            double[] channel2Var2 = AStatistic.GetDatafromPointPairList(channelTwoVariableTwo, statistic_graph.GraphPane.XAxis.Scale.Min, statistic_graph.GraphPane.XAxis.Scale.Max);

            if (channel1Var1 != null && channel1Var1.Length > 0 && parametersToShow_listbox.Items.Count > 0)
            {
                string paramName = parametersToShow_listbox.Items[0].ToString();
                Channel1Param1.Text = paramName.Substring(0, paramName.IndexOf(',')) + " - " + channel1Var1.Length + " набл.";
                statisticParameter1Current_label.Text = "V = " + Math.Round(channel1Var1.Last(), 3).ToString();
                statisticParameter1Max_label.Text = "MAX = " + Math.Round(channel1Var1.Max(), 3).ToString();
                statisticParameter1Min_label.Text = "MIN = " + Math.Round(channel1Var1.Min(), 3).ToString();
                statisticParameter1Diff_label.Text = "ΔV = " + Math.Round(channel1Var1.Max() - channel1Var1.Min(), 3).ToString();
                statisticParameter1M_label.Text = "μ = " + Math.Round(channel1Var1.Average(), 3).ToString();
                statisticParameter1Sigma_label.Text = "σ = " + Math.Round(AStatistic.StaticDeviation(channel1Var1), 3).ToString();
            }

            if (channel1Var2 != null && channel1Var2.Length > 0 && parametersToShow_listbox.Items.Count > 1)
            {
                string paramName = parametersToShow_listbox.Items[1].ToString();
                Channel1Param2.Text = paramName.Substring(0, paramName.IndexOf(',')) + " - " + channel1Var2.Length + " набл.";
                statisticParameter2Current_label.Text = "V = " + Math.Round(channel1Var2.Last(), 3).ToString();
                statisticParameter2Max_label.Text = "MAX = " + Math.Round(channel1Var2.Max(), 3).ToString();
                statisticParameter2Min_label.Text = "MIN = " + Math.Round(channel1Var2.Min(), 3).ToString();
                statisticParameter2Diff_label.Text = "ΔV = " + Math.Round(channel1Var2.Max() - channel1Var2.Min(), 3).ToString();
                statisticParameter2M_label.Text = "μ = " + Math.Round(channel1Var2.Average(), 3).ToString();
                statisticParameter2Sigma_label.Text = "σ = " + Math.Round(AStatistic.StaticDeviation(channel1Var2), 3).ToString();
            }

            if (channel2Var1 != null && channel2Var1.Length > 0 && parametersToShow_listbox.Items.Count > 0)
            {
                string paramName = parametersToShow_listbox.Items[0].ToString();
                Channel2Param1.Text = paramName.Substring(0, paramName.IndexOf(',')) + " - " + channel2Var1.Length + " набл.";
                statisticParameter3Current_label.Text = "V = " + Math.Round(channel2Var1.Last(), 3).ToString();
                statisticParameter3Max_label.Text = "MAX = " + Math.Round(channel2Var1.Max(), 3).ToString();
                statisticParameter3Min_label.Text = "MIN = " + Math.Round(channel2Var1.Min(), 3).ToString();
                statisticParameter3Diff_label.Text = "ΔV = " + Math.Round(channel2Var1.Max() - AStatistic.GetDatafromPointPairList(channelTwoVariableOne).Min(), 3).ToString();
                statisticParameter3M_label.Text = "μ = " + Math.Round(channel2Var1.Average(), 3).ToString();
                statisticParameter3Sigma_label.Text = "σ = " + Math.Round(AStatistic.StaticDeviation(channel2Var1), 3).ToString();
            }

            if (channel2Var2 != null && channel2Var2.Length > 0 && parametersToShow_listbox.Items.Count > 1)
            {
                string paramName = parametersToShow_listbox.Items[1].ToString();
                Channel2Param2.Text = paramName.Substring(0, paramName.IndexOf(',')) + " - " + channel2Var2.Length + " набл.";
                statisticParameter4Current_label.Text = "V = " + Math.Round(channel2Var2.Last(), 3).ToString();
                statisticParameter4Max_label.Text = "MAX = " + Math.Round(channel2Var2.Max(), 3).ToString();
                statisticParameter4Min_label.Text = "MIN = " + Math.Round(channel2Var2.Min(), 3).ToString();
                statisticParameter4Diff_label.Text = "ΔV = " + Math.Round(channel2Var2.Max() - channel2Var2.Min(), 3).ToString();
                statisticParameter4M_label.Text = "μ = " + Math.Round(channel2Var2.Average(), 3).ToString();
                statisticParameter4Sigma_label.Text = "σ = " + Math.Round(AStatistic.StaticDeviation(channel2Var2), 3).ToString();
            }

            #endregion

        }

        private bool ReflectogamIsNotAdded(CheckedListBox reflChannel1_checkListBox, string datetime)
        {
            for (int i = 0; i < reflChannel1_checkListBox.Items.Count; i++)
            {
                if (reflChannel1_checkListBox.Items[i].ToString() == datetime)
                    return false;
            }
            return true;
        }

        private void Scan_btn_Click(object sender, EventArgs e)
        {
            ScanTasks();
        }

        private void Open_btn_Click(object sender, EventArgs e)
        {

        }

        private void Start_btn_Click(object sender, EventArgs e)
        {
            StartTasks();
            //foreach (AThread tr in threads)
            //    tr.Start();
        }

        private void Settings_btn_Click(object sender, EventArgs e)
        {
            SettingTasks();
        }

        private void Thermo_btn_Click(object sender, EventArgs e)
        {
            splitContainer.Panel1Collapsed = !splitContainer.Panel1Collapsed;
            subSplitContainer.Panel2Collapsed = false;
        }

        private void ShowStatistic_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Statistic_splitContainer.Panel2Collapsed = !ShowStatistic_checkBox.Checked;
        }

        private void KeyDownEvent(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.F2:
                    break;

                case Keys.F5:
                    StartTasks();
                    runningFlag = !runningFlag;
                    break;

                case Keys.F8:
                    SettingTasks();
                    break;

                case Keys.F12:
                    ScanTasks();
                    break;




            }
        }

        private void ScanTasks()
        {
            сhannels_checkedListBox.Items.Clear();
            channelSelect1_combobox.Items.Clear();
            channelSelect2_combobox.Items.Clear();

            Search sw = new Search();
            sw.Show();

            List<string> comportsWithCP210x = ASearch.SearchAmicoComports();
            if (comportsWithCP210x.Count > 0)
            {
                List<string> comportsWithBaseblocks = ASearch.CheckComportsForBaseblock(comportsWithCP210x, (int)power_delay.Value);
                if (comportsWithBaseblocks.Count > 0)
                {
                    allDevices = ASearch.SearchSensors(comportsWithBaseblocks, (int)power_delay.Value);
                    if (allDevices.Count > 0)
                    {
                        ALayout.SetChannelsCheckboxList(сhannels_checkedListBox, allDevices);
                        sw.Close();
                    }
                    else
                    {
                        sw.Close();
                        MessageBox.Show("Не обнаружены датчики для опроса. Проверьте правильность подключения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    sw.Close();
                    MessageBox.Show("Не обнаружены базовые блоки.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                sw.Close();
                MessageBox.Show("Не обнаружены преобразователи интерфейсов RS-485 -> USB", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SettingTasks()
        {
            splitContainer.Panel1Collapsed = !splitContainer.Panel1Collapsed;
            subSplitContainer.Panel1Collapsed = false;
            subSplitContainer.Panel2Collapsed = true;
        }

        private void StartTasks()
        {
            if (!runningFlag)
            {
                devicesForExperiment = ALayout.GetDevicesForExperiment(сhannels_checkedListBox);
                if (devicesForExperiment.Count > 0)
                {
                    splitContainer.Panel1Collapsed = true;
                    Start_btn.Image = ((System.Drawing.Image)(Properties.Resources.pause));
                    StartStop_label.Text = "Стоп";
                    ALayout.SetChannelsComboBox(сhannels_checkedListBox, channelSelect1_combobox, channelSelect2_combobox);

                    baseblockInfo = ABaseblock.WriteBaseblockInfo(devicesForExperiment);

                    ADatabaseWorker.WriteBaseblocks(db, ref baseblockInfo);
                    ADatabaseWorker.WriteChannels(db, ref baseblockInfo);
                    ADatabaseWorker.WriteExperiments(db, ref experimentGroup, baseblockInfo, (int)power_delay.Value, (int)comm_delay.Value, experimentDescripthon_txtbox.Text);
                    //write data to database

                    foreach (ABaseblock device in baseblockInfo)
                    {
                        threads.Add(new AThread(device, experimentGroup, db, (int)power_delay.Value, (int)comm_delay.Value, true));
                    }
                    timer.Start();
                }
                else
                {
                    MessageBox.Show("Не выбраны датчики для опроса", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Start_btn.Image = ((System.Drawing.Image)(Properties.Resources.play));
                StartStop_label.Text = "Старт";
                timer.Stop();
                foreach (AThread thread in threads)
                {
                    if (!thread.Stop())
                    {
                    }
                }

            }
        }

        private void Logo_btn_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.amico.ua");
        }

        private void twoZond_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (twoZond_checkBox.Checked)
                twoZond_checkBox.Text = "Один зонд";
            else
                twoZond_checkBox.Text = "Два зонда";
        }

        private void writeRefl_checkbox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void addParameter_btn_Click(object sender, EventArgs e)
        {
            if (allParameters_listbox.SelectedItem != null)
            {
                if (parametersToShow_listbox.Items.Count < 2)
                {
                    parametersToShow_listbox.Items.Add(allParameters_listbox.SelectedItem);
                    allParameters_listbox.Items.Remove(allParameters_listbox.SelectedItem);
                }
                else
                    MessageBox.Show("Невозможно добавить более двух параметров!!");
            }
            else
            {
                MessageBox.Show("Выберите параметр!!");
            }
        }

        private void removeParameter_btn_Click(object sender, EventArgs e)
        {
            if (parametersToShow_listbox.SelectedItem != null)
            {
                allParameters_listbox.Items.Add(parametersToShow_listbox.SelectedItem);
                parametersToShow_listbox.Items.Remove(parametersToShow_listbox.SelectedItem);
            }
            else
            {
                MessageBox.Show("Выберите параметр!!");
            }
        }

        private void Channel1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (afterInitialize)
            //{
               channel1 = ALayout.GetChannelForDisplay(channelSelect1_combobox);                
            //}
        }

        private void Channel2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (afterInitialize)
            //{
                channel2 = ALayout.GetChannelForDisplay(channelSelect2_combobox);                
            //}
        }

        private void reflParameters_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Reflectogram_splitContainer.Panel2Collapsed = !reflParameters_checkBox.Checked;
        }

        private void _dbInterval_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            timer.Interval = (int)_dbInterval_numericUpDown.Value * 1000;
        }

        private void reflChannel1_ItemCheck(object sender, ItemCheckEventArgs e)
        {            
            if (channel1.Key != null && e.NewValue == CheckState.Checked)
            {
                CheckedListBox check = sender as CheckedListBox;
                PointPairList channelOneRefl = ADatabaseWorker.GetReflectogramData(db, experimentGroup, channel1.Key, channel1.Value, check.Items[e.Index].ToString());
                LineItem myCurve;
                if (realTimeRefl)
                    myCurve = refl_graph.GraphPane.AddCurve(check.Items[e.Index].ToString(),
                           channelOneRefl, SystemColors.HotTrack, SymbolType.None);
                else
                    myCurve = refl_graph.GraphPane.AddCurve(check.Items[e.Index].ToString(),
                            channelOneRefl, RandomColor(), SymbolType.None);

                myCurve.Line.IsSmooth = false;

                refl_graph.AxisChange();
                // Make sure the Graph gets redrawn
                refl_graph.Invalidate();
            }
        }

        private void reflChannel2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (channel2.Key != null && e.NewValue == CheckState.Checked)
            {
                CheckedListBox check = sender as CheckedListBox;
                PointPairList channelTwoRefl = ADatabaseWorker.GetReflectogramData(db, experimentGroup, channel2.Key, channel2.Value, check.Items[e.Index].ToString());
                LineItem myCurve;
                if(realTimeRefl)
                    myCurve = refl_graph.GraphPane.AddCurve(check.Items[e.Index].ToString(),
                           channelTwoRefl, Color.Crimson, SymbolType.None);
                else
                    myCurve = refl_graph.GraphPane.AddCurve(check.Items[e.Index].ToString(),
                            channelTwoRefl, RandomColor(), SymbolType.None);

                myCurve.Line.IsSmooth = false;

                refl_graph.AxisChange();
                // Make sure the Graph gets redrawn
                refl_graph.Invalidate();
            }
        }

        public Color RandomColor()
        {
            Random r = new Random();
            byte red = (byte)r.Next(0, 200);
            byte green = (byte)r.Next(0, 200);
            byte blue = (byte)r.Next(0, 200);

            return Color.FromArgb(red, green, blue);
        }

        private void clearReflcheckbox_btn_Click(object sender, EventArgs e)
        {
            refl_graph.GraphPane.CurveList.Clear();
            refl_graph.AxisChange();
            refl_graph.Invalidate();
            for (int i = 0; i < reflChannel1_checkListBox.Items.Count; i++)
                reflChannel1_checkListBox.SetItemChecked(i, false);
            for (int i = 0; i < reflChannel2_checkListBox.Items.Count; i++)
                reflChannel2_checkListBox.SetItemChecked(i, false);
        }

        private void realtime_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (realtime_checkBox.Checked)
            {
                realTimeRefl = false;
                realtime_checkBox.Text = "Выбор рефлектограмм";
            }
            else
            {
                realTimeRefl = true;
                realtime_checkBox.Text = "Опрос в реал. времени";
            }
        }

        private void Calibration_btn_Click(object sender, EventArgs e)
        {

        }

       
        private void StatisticGraph_Click(object sender, MouseEventArgs e)
        {
            CurveItem curve;

            // Сюда будет сохранен номер точки кривой, ближайшей к точке клика
            int index;            

            // Максимальное расстояние от точки клика до кривой в пикселях, 
            // при котором еще считается, что клик попал в окрестность кривой.
            GraphPane.Default.NearestTol = 10;

            bool result = statistic_graph.GraphPane.FindNearestPoint(e.Location, out curve, out index);

            if (result)
            {
                // Максимально расстояние от точки клика до кривой не превысило NearestTol

                // Добавим точку на график, вблизи которой произошел клик
                PointPair point = curve[index];

                // Кривая, состоящая из одной точки. Точка будет отмечена синим кругом
                LineItem curvePount = statistic_graph.GraphPane.AddCurve("",
                    new double[] { curve[index].X },
                    new double[] { curve[index].Y },
                    Color.Blue,
                    SymbolType.Circle);

                // 
                curvePount.Line.IsVisible = false;

                // Цвет заполнения круга - колубой
                curvePount.Symbol.Fill.Color = Color.Blue;

                // Тип заполнения - сплошная заливка
                curvePount.Symbol.Fill.Type = FillType.Solid;

                // Размер круга
                curvePount.Symbol.Size = 7;

                double temperature = ADatabaseWorker.GetTemperature(db, experimentGroup, channel1.Key, channel1.Value, (DateTime.FromOADate(curve[index].X)).ToString("dd.MM.yy HH:mm:ss.ff"));
                PointPairList reflectogram = ADatabaseWorker.GetReflectogramData(db, experimentGroup, channel1.Key, channel1.Value, (DateTime.FromOADate(curve[index].X)).ToString("dd.MM.yy HH:mm:ss.ff"));
                if (reflectogram != null && reflectogram.Count > 0)
                {
                    if (window == null || window.IsDisposed)
                    {
                        window = new ReflectogramViewer(reflectogram, DateTime.FromOADate(curve[index].X), temperature);
                        window.Show();
                    }
                    else
                        window.AddReflectogram(reflectogram, DateTime.FromOADate(curve[index].X), temperature);
                    
                }
            }
        }
       
    }
}
