using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reflecta2._0
{
    public partial class Calibration : Form
    {
        private bool twoZond;
        Dictionary<string, List<byte>> devices;
        private static ADatabase db;
        private BackgroundWorker bw;   
    

        public Calibration(bool _twoZond, Dictionary<string, List<byte>> _devices)
        {
            InitializeComponent();

            db = new ADatabase("calibration");
            db.Connect();
            if (!ADatabase.CalibrationDatabaseStructureIsOK(db.Connection))
            { 
                db.CreateCalibrationDBStructure();
            }
            

            twoZond = _twoZond;
            devices = _devices;

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
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView.Enabled = true;
            save_btn.Enabled = true;
            Open_btn.Enabled = true;
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            List<double> temp = e.UserState as List<double>;
            if (temp != null && temp.Count > 3)
            {
                dataGridView.Rows[dataGridView.RowCount - 2].Cells[1].Value = Math.Round(temp[0] / temp[3], 3);
                dataGridView.Rows[dataGridView.RowCount - 2].Cells[2].Value = Math.Round(temp[1] / temp[3], 3);
                if(temp.Count == 4)
                    dataGridView.Rows[dataGridView.RowCount - 2].Cells[3].Value = Math.Round(temp[2] / temp[3], 1);
            }

            int step = 100 / (int)average_numericUpDown.Value;
            progressBar.Value =(int) (step * temp[3] );
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
                    double[] reflectogram, delays, temperature;
                    double level = 0.0, sum = 0.0, delaySum = 0.0, tempSum = 0.0;
                    for (int i = 0; i < (int)average_numericUpDown.Value; i++)
                    {
                        dr.GetLevel(ref error);
                        reflectogram = dr.GetReflectogram(ref error);
                        temperature = dr.GetTemperature(ref error);
                        delays = ACalculate.CalculateDelays(reflectogram, twoZond ? ADelayCalculationType.TwoZond : ADelayCalculationType.OneZond);
                        level = ACalculate.Level(delays, twoZond ? ACalcType.TwoZond : ACalcType.OneZond);
                        sum += level;
                        delaySum += delays[1];
                        if (temperature != null && temperature.Length > 0)
                            tempSum += temperature[0];
                        List<double> temp = new List<double>();
                        temp.Add(sum);
                        temp.Add(delaySum);
                        temp.Add(tempSum);
                        temp.Add(i+1);                      
                           
                        bw.ReportProgress(0, temp);
                    }
                }
                dr.ClosePort();
            }

        }   

        private void save_btn_Click(object sender, EventArgs e)
        {
            channel_combobox.Enabled = true;
            average_numericUpDown.Enabled = true; 

            string device = channel_combobox.SelectedItem.ToString();
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
                    double firmware = dr.GetFirmware(ref error);
                    string serial = dr.GetSerial(ref error);
                    int calibrationGroup =  ACalibrationDatabaseWorker.WriteCalibrationGroup(db, (int)average_numericUpDown.Value, firmware, serial);  
                    for(int i = 0; i < dataGridView.RowCount; i++)
                    {
                        if (dataGridView.Rows[i].Cells[0].Value != null && dataGridView.Rows[i].Cells[0].Value.ToString() != "" &&
                            dataGridView.Rows[i].Cells[1].Value != null && dataGridView.Rows[i].Cells[1].Value.ToString() != "")
                        {
                            ACalibrationDatabaseWorker.WriteCalibrationData(db, calibrationGroup,
                                dataGridView.Rows[i].Cells[0].Value.ToString(), dataGridView.Rows[i].Cells[1].Value.ToString());
                        }
                    }
                }
                dr.ClosePort();
            }
            
        }  
      
        private void StatRowValidated(object sender, DataGridViewCellEventArgs e)
        {
            channel_combobox.Enabled = false;
            average_numericUpDown.Enabled = false;
            if (channel_combobox.SelectedItem != null && channel_combobox.SelectedItem.ToString() != "")
            {
                if (dataGridView.Rows[e.RowIndex].Cells[0].Value != null && dataGridView.Rows[e.RowIndex].Cells[0].Value.ToString() != "")
                {
                    if (!bw.IsBusy)
                    {
                        progressBar.Value = 0;
                        bw.RunWorkerAsync(channel_combobox.SelectedItem.ToString());
                        dataGridView.Enabled = false;
                        save_btn.Enabled = false;
                        Open_btn.Enabled = false;
                    }                    
                }
            }
        }

        private void Open_btn_Click(object sender, EventArgs e)
        {
            channel_combobox.Enabled = true;
            average_numericUpDown.Enabled = true;
            List<List<string>> data = ACalibrationDatabaseWorker.GetAllCalibrationGroup(db);
            OpenCalibration opw = new OpenCalibration(twoZond, devices, data);
            opw.Show();
        }

        private void Unlock_btn_Click(object sender, EventArgs e)
        {
            channel_combobox.Enabled = true;
            average_numericUpDown.Enabled = true;
        }
        
    }
}
