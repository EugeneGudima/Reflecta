﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reflecta2._0
{
    public partial class OpenCalibration : Form
    {
        bool twoZond;
        Dictionary<string, Dictionary<string, byte>> devices;

        public OpenCalibration(bool _twoZond, Dictionary<string, Dictionary<string, byte>> _devices, List<List<string>> data)
        {
            InitializeComponent();

            twoZond = _twoZond;
            devices = _devices;

            dataGridView.Rows.Clear();

            foreach (List<string> line in data)
            {
                if(line.Count == 5)
                    dataGridView.Rows.Add(false, line[0], line[2], line[1], line[4], line[3]);
            }
        }

        private void DeleteCalibrationGroup_btn_Click(object sender, EventArgs e)
        {

        }

        private void OpenCalibrationGroup_btn_Click(object sender, EventArgs e)
        {
            TestCalibration tc = new TestCalibration(twoZond, devices);
            tc.Show();
        }
    }
}
