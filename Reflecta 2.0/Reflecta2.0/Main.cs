﻿using System;
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
        static ADatabase db;
        private bool runningFlag = false;
        private bool realTimeRefl = true;
        private Color[] reflColors = { Color.Maroon, Color.DarkGray, Color.Chocolate, Color.MediumSeaGreen, Color.Teal, Color.Navy, Color.DarkViolet, Color.HotPink, Color.Black, Color.Aqua, Color.DeepPink };
        ReflectogramViewer window;

        public MainWindow()
        {
            InitializeComponent();

            db = new ADatabase("database");
            db.Connect();
            if (!ADatabase.MainDatabaseStructureIsOK(db.Connection))
            {                
                
                db.CreateDBStructure();
                db.WriteParameters();
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
            DrawTask(false);
        }

        private void DrawTask(bool openFlag)
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

            if (!openFlag)
            {
                #region ReflectogramTask
                if (channel1.Key != null)
                {
                    reflChannel1_checkListBox.Visible = true;

                    List<string> datetimeChannel1 = ADatabaseWorker.GetAllReflectograms(db, experimentGroup, channel1.Key, channel1.Value);
                    int count = 0;
                    foreach (string datetime in datetimeChannel1)
                    {
                        if (datetimeChannel1.Count < 100)
                        {
                            if (ReflectogamIsNotAdded(reflChannel1_checkListBox, datetime))
                                reflChannel1_checkListBox.Items.Add(datetime, false);
                            else if (realTimeRefl)
                                reflChannel1_checkListBox.SetItemChecked(count, false);
                            count++;
                        }
                        else if (datetimeChannel1.Count > 0)
                            reflChannel1_checkListBox.Items.Add(datetimeChannel1[datetimeChannel1.Count - 1], false);

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
                        if (datetimeChannel2.Count < 100)
                        {
                            if (ReflectogamIsNotAdded(reflChannel2_checkListBox, datetime))
                                reflChannel2_checkListBox.Items.Add(datetime, false);
                            else if (realTimeRefl)
                                reflChannel2_checkListBox.SetItemChecked(count, false);
                            count++;
                        }
                        else if (datetimeChannel2.Count > 0)
                            reflChannel2_checkListBox.Items.Add(datetimeChannel2[datetimeChannel2.Count - 1], false);
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

            }
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
                            channelOneVariableTwo, Color.Crimson, SymbolType.None);

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

            if (channel1Var1 != null && channel1Var1.Length > 0 && parametersToShow_listbox.Items.Count > 0 && channelSelect1_combobox.SelectedIndex != 0)
            {
                string paramName = parametersToShow_listbox.Items[0].ToString();
                Channel1Param1.Text = paramName.Substring(0, paramName.IndexOf(',')) + " - " + channel1Var1.Length + " набл.";
                statisticParameter1Current_label.Text = "V = " + Math.Round(channel1Var1.Last(), 3).ToString();
                statisticParameter1Max_label.Text = "MAX = " + Math.Round(channel1Var1.Max(), 3).ToString();
                statisticParameter1Min_label.Text = "MIN = " + Math.Round(channel1Var1.Min(), 3).ToString();
                statisticParameter1Diff_label.Text = "ΔV = " + Math.Round(channel1Var1.Max() - channel1Var1.Min(), 3).ToString();
                statisticParameter1M_label.Text = "μ = " + Math.Round(channel1Var1.Average(), 3).ToString();
                statisticParameter1Sigma_label.Text = "σ = " + Math.Round(AStatistic.StaticDeviation(channel1Var1), 3).ToString();

                label5.Visible = false;
                CorrelationChannel1_label.Visible = false;
            }
            else
                 ClearStatLabelsCh1Param1();

            if (channel1Var2 != null && channel1Var2.Length > 0 && parametersToShow_listbox.Items.Count > 1 && channelSelect1_combobox.SelectedIndex != 0)
            {
                string paramName = parametersToShow_listbox.Items[1].ToString();
                Channel1Param2.Text = paramName.Substring(0, paramName.IndexOf(',')) + " - " + channel1Var2.Length + " набл.";
                statisticParameter2Current_label.Text = "V = " + Math.Round(channel1Var2.Last(), 3).ToString();
                statisticParameter2Max_label.Text = "MAX = " + Math.Round(channel1Var2.Max(), 3).ToString();
                statisticParameter2Min_label.Text = "MIN = " + Math.Round(channel1Var2.Min(), 3).ToString();
                statisticParameter2Diff_label.Text = "ΔV = " + Math.Round(channel1Var2.Max() - channel1Var2.Min(), 3).ToString();
                statisticParameter2M_label.Text = "μ = " + Math.Round(channel1Var2.Average(), 3).ToString();
                statisticParameter2Sigma_label.Text = "σ = " + Math.Round(AStatistic.StaticDeviation(channel1Var2), 3).ToString();

                label5.Visible = true;
                CorrelationChannel1_label.Visible = true;
                CorrelationChannel1_label.Text = Math.Round(AStatistic.Correlation(channel1Var1, channel1Var2), 5).ToString();
            }
            else
                ClearStatLabelsCh1Param2();

            if (channel2Var1 != null && channel2Var1.Length > 0 && parametersToShow_listbox.Items.Count > 0 && channelSelect2_combobox.SelectedIndex != 0)
            {
                string paramName = parametersToShow_listbox.Items[0].ToString();
                Channel2Param1.Text = paramName.Substring(0, paramName.IndexOf(',')) + " - " + channel2Var1.Length + " набл.";
                statisticParameter3Current_label.Text = "V = " + Math.Round(channel2Var1.Last(), 3).ToString();
                statisticParameter3Max_label.Text = "MAX = " + Math.Round(channel2Var1.Max(), 3).ToString();
                statisticParameter3Min_label.Text = "MIN = " + Math.Round(channel2Var1.Min(), 3).ToString();
                statisticParameter3Diff_label.Text = "ΔV = " + Math.Round(channel2Var1.Max() - AStatistic.GetDatafromPointPairList(channelTwoVariableOne).Min(), 3).ToString();
                statisticParameter3M_label.Text = "μ = " + Math.Round(channel2Var1.Average(), 3).ToString();
                statisticParameter3Sigma_label.Text = "σ = " + Math.Round(AStatistic.StaticDeviation(channel2Var1), 3).ToString();

                label8.Visible = false;
                CorrelationChannel2_label.Visible = false;
            }
            else
                ClearStatLabelsCh2Param1();

            if (channel2Var2 != null && channel2Var2.Length > 0 && parametersToShow_listbox.Items.Count > 1 && channelSelect2_combobox.SelectedIndex != 0)
            {
                string paramName = parametersToShow_listbox.Items[1].ToString();
                Channel2Param2.Text = paramName.Substring(0, paramName.IndexOf(',')) + " - " + channel2Var2.Length + " набл.";
                statisticParameter4Current_label.Text = "V = " + Math.Round(channel2Var2.Last(), 3).ToString();
                statisticParameter4Max_label.Text = "MAX = " + Math.Round(channel2Var2.Max(), 3).ToString();
                statisticParameter4Min_label.Text = "MIN = " + Math.Round(channel2Var2.Min(), 3).ToString();
                statisticParameter4Diff_label.Text = "ΔV = " + Math.Round(channel2Var2.Max() - channel2Var2.Min(), 3).ToString();
                statisticParameter4M_label.Text = "μ = " + Math.Round(channel2Var2.Average(), 3).ToString();
                statisticParameter4Sigma_label.Text = "σ = " + Math.Round(AStatistic.StaticDeviation(channel2Var2), 3).ToString();

                label8.Visible = true;
                CorrelationChannel2_label.Visible = true;
                CorrelationChannel2_label.Text = Math.Round(AStatistic.Correlation(channel2Var1, channel2Var2), 5).ToString();
            }
            else
                ClearStatLabelsCh2Param2();

            #endregion
        }

        private void ClearStatLabelsCh1Param1()
        {
            Channel1Param1.Text = "";
            statisticParameter1Current_label.Text = "";
            statisticParameter1Max_label.Text = "";
            statisticParameter1Min_label.Text = "";
            statisticParameter1Diff_label.Text = "";
            statisticParameter1M_label.Text = "";
            statisticParameter1Sigma_label.Text = "";
        }

        private void ClearStatLabelsCh1Param2()
        {
            Channel1Param2.Text = "";
            statisticParameter2Current_label.Text = "";
            statisticParameter2Max_label.Text = "";
            statisticParameter2Min_label.Text = "";
            statisticParameter2Diff_label.Text = "";
            statisticParameter2M_label.Text = "";
            statisticParameter2Sigma_label.Text = "";
        }

        private void ClearStatLabelsCh2Param1()
        {
            Channel2Param1.Text = "";
            statisticParameter3Current_label.Text = "";
            statisticParameter3Max_label.Text = "";
            statisticParameter3Min_label.Text = "";
            statisticParameter3Diff_label.Text = "";
            statisticParameter3M_label.Text = "";
            statisticParameter3Sigma_label.Text = "";
        }

        private void ClearStatLabelsCh2Param2()
        {  
            Channel2Param2.Text = "";
            statisticParameter4Current_label.Text = "";
            statisticParameter4Max_label.Text = "";
            statisticParameter4Min_label.Text = "";
            statisticParameter4Diff_label.Text = "";
            statisticParameter4M_label.Text = "";
            statisticParameter4Sigma_label.Text = "";
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
            OpenTasks();
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
                        ALayout.SetCalibrationDatagrid(calibParams_dataGridView, allDevices);
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
                    if (experimentDescripthon_txtbox.Text != null && experimentDescripthon_txtbox.Text != "")
                    {
                        splitContainer.Panel1Collapsed = true;
                        pause_checkBox.Visible = true;
                        label7.Visible = true;
                        RedrawStatistic_btn.Enabled = false;
                        Open_btn.Enabled = false;
                        Calibration_btn.Enabled = false;
                        calibParams_dataGridView.Enabled = false;
                        Scan_btn.Enabled = false;
                        Start_btn.Image = ((System.Drawing.Image)(Properties.Resources.icon_circlestop));
                        StartStop_label.Text = "Стоп";
                        ALayout.SetChannelsComboBox(сhannels_checkedListBox, channelSelect1_combobox, channelSelect2_combobox);

                        baseblockInfo = ABaseblock.WriteBaseblockInfo(devicesForExperiment);

                        ADatabaseWorker.WriteBaseblocks(db, ref baseblockInfo);
                        ADatabaseWorker.WriteChannels(db, ref baseblockInfo);
                        ADatabaseWorker.WriteExperiments(db, ref experimentGroup, baseblockInfo, calibParams_dataGridView.Rows, (int)power_delay.Value, (int)comm_delay.Value, experimentDescripthon_txtbox.Text);
                        //write data to database

                        threads.Clear();
                        foreach (ABaseblock device in baseblockInfo)
                        {
                            threads.Add(new AThread(device, experimentGroup, db, (int)power_delay.Value, (int)comm_delay.Value, true));
                        }
                        timer.Start();
                        runningFlag = true;
                    }
                    else
                    {
                        MessageBox.Show("Введите описание эксперимента! Сами потом пожалеете!!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        experimentDescripthon_txtbox.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Не выбраны датчики для опроса", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Start_btn.Image = ((System.Drawing.Image)(Properties.Resources.icon_circleplay));
                StartStop_label.Text = "Старт";
                experimentDescripthon_txtbox.Text = "";
                runningFlag = false;
                pause_checkBox.Visible = false;
                label7.Visible = false;
                pause_checkBox.Checked = false;
                Open_btn.Enabled = true;
                Calibration_btn.Enabled = true;
                Scan_btn.Enabled = true;
                calibParams_dataGridView.Enabled = true;
                timer.Stop();
                foreach (AThread thread in threads)
                {
                    if (!thread.Stop())
                    {
                    }
                }
                ADatabaseWorker.WriteGroupExperimentEndDate(db, experimentGroup);
            }
        }

        private void OpenTasks()
        {
            сhannels_checkedListBox.Items.Clear();
            channelSelect1_combobox.Items.Clear();
            channelSelect2_combobox.Items.Clear();

            RedrawStatistic_btn.Enabled = true;

            List<List<string>> experimantsGroupsList = ADatabaseWorker.GetAllExperimentGroups(db);
            OpenExperimentsGroupsDialog od = new OpenExperimentsGroupsDialog(experimantsGroupsList);
            od.ShowDialog();

            if (od.ExperimentGroupIDsForDelete != null && od.ExperimentGroupIDsForDelete.Count > 0)
            {
                ADatabaseWorker.DeleteExperimentData(db, od.ExperimentGroupIDsForDelete);
            }

            if (od.ExperimentGroupIDsForOpen != null && od.ExperimentGroupIDsForOpen.Count > 0)
            {
                experimentGroup = Convert.ToInt32(od.ExperimentGroupIDsForOpen[0]);
                List<string> data = ADatabaseWorker.GetBaseblockAndChannelsInfo(db, experimentGroup);
                ALayout.SetChannelsComboBox(data, channelSelect1_combobox, channelSelect2_combobox);
                DrawTask(true);
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
            channel1 = ALayout.GetChannelForDisplay(channelSelect1_combobox);
        }

        private void Channel2_SelectedIndexChanged(object sender, EventArgs e)
        {
            channel2 = ALayout.GetChannelForDisplay(channelSelect2_combobox);
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
                if (refl_graph.GraphPane.CurveList.Count > 10)
                {
                    refl_graph.GraphPane.CurveList.Clear();
                    for (int i = 0; i < reflChannel1_checkListBox.Items.Count; i++)
                        reflChannel1_checkListBox.SetItemChecked(i, false);
                }

                CheckedListBox check = sender as CheckedListBox;
                PointPairList channelOneRefl = ADatabaseWorker.GetReflectogramData(db, experimentGroup, channel1.Key, channel1.Value, check.Items[e.Index].ToString());
                LineItem myCurve;
                if (realTimeRefl)
                    myCurve = refl_graph.GraphPane.AddCurve(check.Items[e.Index].ToString(),
                           channelOneRefl, SystemColors.HotTrack, SymbolType.None);
                else
                    myCurve = refl_graph.GraphPane.AddCurve(check.Items[e.Index].ToString(),
                            channelOneRefl, reflColors[refl_graph.GraphPane.CurveList.Count], SymbolType.None);

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
                if (refl_graph.GraphPane.CurveList.Count > 10)
                {
                    refl_graph.GraphPane.CurveList.Clear();
                }

                CheckedListBox check = sender as CheckedListBox;
                PointPairList channelTwoRefl = ADatabaseWorker.GetReflectogramData(db, experimentGroup, channel2.Key, channel2.Value, check.Items[e.Index].ToString());
                LineItem myCurve;

                if (realTimeRefl)
                    myCurve = refl_graph.GraphPane.AddCurve(check.Items[e.Index].ToString(),
                           channelTwoRefl, Color.Crimson, SymbolType.None);
                else
                    myCurve = refl_graph.GraphPane.AddCurve(check.Items[e.Index].ToString(),
                            channelTwoRefl, reflColors[refl_graph.GraphPane.CurveList.Count], SymbolType.None);

                myCurve.Line.IsSmooth = false;

                refl_graph.AxisChange();
                refl_graph.Invalidate();
            }
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
            if (allDevices != null && allDevices.Count > 0)
            {
                Calibration calibWindow = new Calibration(!twoZond_checkBox.Checked, allDevices);
                calibWindow.Show();
            }
            else
                MessageBox.Show("Нет каналов для калибровки!");
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

        private void RedrawStatistic_btn_Click(object sender, EventArgs e)
        {
            DrawTask(true);
        }

        private void refl_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            ToolStripItem newMenuItem = new ToolStripMenuItem("ф-я Василич");
            newMenuItem.Click += newMenuItem_Click;
            menuStrip.Items.Add(newMenuItem);
        }

        private void stat_contextMenuChanged(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            // Добавим свой пункт меню
            ToolStripItem newMenuItem = new ToolStripMenuItem("ф-я Василич");
            newMenuItem.Click += newMenuItem_Click;
            menuStrip.Items.Add(newMenuItem);
        }

        void newMenuItem_Click(object sender, EventArgs e)
        {
                      
            ToolStripItem newMenuItem = (sender as ToolStripItem);
            ContextMenuStrip sp = newMenuItem.Owner as ContextMenuStrip;

            ZedGraphControl z = sp.SourceControl as ZedGraphControl;
            z.GraphPane.Fill.Color = Color.White;
            z.GraphPane.Border.Color = Color.White;
            z.Copy(false);
            z.GraphPane.Fill.Color = SystemColors.Control;
            z.GraphPane.Border.Color = SystemColors.Control;           

        }

        private void pause_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (pause_checkBox.Checked)
            {
                for(int i = 0; i < threads.Count; i++) 
                {
                    threads[i].Stop();
                }
                timer.Stop();
            }
            else
            {
                for (int i = 0; i < threads.Count; i++)
                {
                    threads[i].Start();
                }
                timer.Start();
            }
        }

        private void Info_btn_Click(object sender, EventArgs e)
        {
            List<string> channelInfo = ADatabaseWorker.GetBaseblockAndChannelsInfo(db, experimentGroup);
            List<string> info = ADatabaseWorker.GetExperimentGroupInfo(db, experimentGroup);

            string infoMessage = "";
            if (info != null && info.Count > 6)
            {
                infoMessage += "ID: " + info[0];
                infoMessage += Environment.NewLine + "Начало:" + info[1];
                infoMessage += Environment.NewLine + "Длительность: ";
                infoMessage += info[2] == "0" ? "< 1 мин." : (Convert.ToInt32(info[2] == "" ? "0" : info[2]) / 60).ToString() + ':' + (Convert.ToInt32(info[2] == "" ? "0" : info[2]) % 60).ToString() + " мин.";
                infoMessage += Environment.NewLine + "Зад. питания: " + info[3] + " с";
                infoMessage += Environment.NewLine + "Зад. переключен.: " + info[4] + " с";
                infoMessage += Environment.NewLine + "Кол-во каналов: " + info[6];
                infoMessage += Environment.NewLine + "Описание: " + info[5];

                infoMessage += Environment.NewLine + "Kаналы:";
                foreach (string channel in channelInfo)
                    infoMessage += Environment.NewLine + channel;
                MessageBox.Show(infoMessage, "Описание эксперимента", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Нет информации для отображения!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Export_btn_Click(object sender, EventArgs e)
        {
            if (experimentGroup != 0)
            {
                List<string> experimentsList = ADatabaseWorker.GetExperimentIDsInfo(db, experimentGroup);
                ExportWindow ew = new ExportWindow(experimentsList);
                ew.ShowDialog();                
                if (ew.ExperimentIDs.Count > 0)
                {
                    Export.ExportData(db, ew.ExperimentIDs);
                }
                else
                    MessageBox.Show("Экспорт не будет проведен.\nНе выбраны эксперименты для экспорта.");
            }
            else
            {
                MessageBox.Show("Нет данных для экспорта");
            }
        }

    }
}
