using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reflecta2._0
{
    public partial class TestCalibration : Form
    {
        private bool twoZond;
        private static ADatabase db;
        private BackgroundWorker bw;

        public TestCalibration(bool _twoZond, Dictionary<string, List<byte>> devices)
        {
            InitializeComponent();

            db = new ADatabase("calibration");
            db.Connect();
            if (!ADatabase.CalibrationDatabaseStructureIsOK(db.Connection))
            { 
                db.CreateCalibrationDBStructure();
            }
            

            twoZond = _twoZond;

            List<string> devicesForCalibration = new List<string>();
            foreach (KeyValuePair<string, List<byte>> baseblock in devices)
            {
                foreach (byte channel in baseblock.Value)
                {
                    devicesForCalibration.Add(baseblock.Key + " - Канал " + channel.ToString());
                }
            }

            channel_combobox.Items.Clear();

            foreach (string record in devicesForCalibration)
            {
                channel_combobox.Items.Add(record);
            }
            channel_combobox.SelectedIndex = 0;

            dataGridView.Focus();

            bw = new BackgroundWorker();
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.WorkerReportsProgress = true;            

            int MinID = dataGridView.Rows[0].Cells.Cast<DataGridViewTextBoxCell>()
                        .Min(r => Convert.ToInt32(r.Value));

            dataGridView.Rows[0].Cells.Cast<DataGridViewTextBoxCell>().Single(x => x.Value.ToString() == MinID.ToString()).Style.BackColor = Color.LightGreen;

            int MaxID2 = dataGridView.Rows[1].Cells.Cast<DataGridViewTextBoxCell>()
                        .Min(r => Convert.ToInt32(r.Value));

            dataGridView.Rows[1].Cells.Cast<DataGridViewTextBoxCell>().Single(x => x.Value.ToString() == MaxID2.ToString()).Style.BackColor = Color.LightGreen;

            int MaxID3 = dataGridView.Rows[2].Cells.Cast<DataGridViewTextBoxCell>()
                        .Min(r => Convert.ToInt32(r.Value));

            dataGridView.Rows[2].Cells.Cast<DataGridViewTextBoxCell>().Single(x => x.Value.ToString() == MaxID3.ToString()).Style.BackColor = Color.LightGreen;

            int MaxID4 = dataGridView.Rows[3].Cells.Cast<DataGridViewTextBoxCell>()
                        .Min(r => Convert.ToInt32(r.Value));

            dataGridView.Rows[3].Cells.Cast<DataGridViewTextBoxCell>().Single(x => x.Value.ToString() == MaxID4.ToString()).Style.BackColor = Color.LightGreen;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView.Enabled = true;            
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            List<double> temp = e.UserState as List<double>;
            if (temp != null && temp.Count == 2)
                dataGridView.Rows[dataGridView.RowCount - 2].Cells[1].Value = Math.Round(temp[0] / temp[1], 3);

            int step = 100 / (int)average_numericUpDown.Value;
            progressBar.Value = (int)(step * temp[1]);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string device = e.Argument as string;
            int comStart = device.IndexOf('(') + 1;
            int comEnd = device.IndexOf(')') - comStart;
            string comport = device.Substring(comStart, comEnd);
            int channelStart = device.IndexOf("Канал ") + 6;
            byte channel = Convert.ToByte(device.Substring(channelStart));
            ADriver dr = new ADriver(comport, 100);
            string error = "";
            if (dr.OpenPort(ref error))
            {
                if (dr.SetChannel(channel))
                {
                    double[] reflectogram, delays;
                    double level = 0.0, sum = 0.0;
                    for (int i = 0; i < (int)average_numericUpDown.Value; i++)
                    {
                        dr.GetLevel(ref error);
                        reflectogram = dr.GetReflectogram(ref error);
                        delays = ACalculate.CalculateDelays(reflectogram, twoZond ? ADelayCalculationType.TwoZond : ADelayCalculationType.OneZond);
                        level = ACalculate.Level(delays, twoZond ? ACalcType.TwoZond : ACalcType.OneZond);
                        sum += level;
                        List<double> temp = new List<double>();
                        temp.Add(sum);
                        temp.Add(i + 1);
                        bw.ReportProgress(0, temp);
                    }
                }
                dr.ClosePort();
            }
        }

        private void CalibRowValidated(object sender, DataGridViewCellEventArgs e)
        {
            //if (channel_combobox.SelectedItem != null && channel_combobox.SelectedItem.ToString() != "")
            //{
            //    if (dataGridView.Rows[e.RowIndex].Cells[0].Value != null && dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString() != "")
            //    {
            //        if (!bw.IsBusy)
            //        {
            //            progressBar.Value = 0;
            //            bw.RunWorkerAsync(channel_combobox.SelectedItem.ToString());
            //            dataGridView.Enabled = false;                       
            //        }
            //    }
            //}
        }
    }
}
