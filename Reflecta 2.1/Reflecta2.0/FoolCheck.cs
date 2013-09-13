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
    public partial class FoolCheck : Form
    {
        bool flag;
        int num1, num2;
        public FoolCheck(int _num1, int _num2)
        {
            InitializeComponent();

            flag = false;
            num1 = _num1;
            num2 = _num2;
            label1.Text = num1.ToString() + " + " + num2.ToString() + " = ";
        }
        public bool Flag
        {
            get { return flag; }
        }

        private void button_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt16(answer_textBox.Text) == num1 + num2)
                    flag = true;
                this.Close();
            }
            catch
            {
                flag = false;
            }
        }
    }
}
