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
    public partial class Search : Form
    {
        Timer t;
        long count = 0;

        public Search()
        {
            InitializeComponent();

            //t = new Timer();
            //t.Interval = 500;
            //t.Enabled = true;
            //t.Tick += t_Tick;
            //t.Start();
        }

        //void t_Tick(object sender, EventArgs e)
        //{
        //    label.Text = "Поиск";
        //    switch (count % 4)
        //    {               

        //        case 1:
        //            label.Text += ".";
        //            break;

        //        case 2:
        //            label.Text += "..";
        //            break;

        //        case 3:
        //            label.Text += "...";
        //            break;
        //    }
        //}

        private void SearchClosing(object sender, FormClosingEventArgs e)
        {
            //t.Stop();
            //t.Dispose();
        }
    }
}
