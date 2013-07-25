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
    public partial class ExportWindow : Form
    {
        private List<string> experimentsList;

        private List<int> experimentIDs = new List<int>();

        public ExportWindow(List<string> experimentsList)
        {
            InitializeComponent();

            this.experimentsList = experimentsList;

            foreach (string experiment in experimentsList)
                checkedListBox.Items.Add(experiment);
        }

        public List<int> ExperimentIDs
        {
            get { return experimentIDs; }
        }

        private void OK_btn_Click(object sender, EventArgs e)
        {
            experimentIDs = new List<int>();           
            foreach(string experiment in checkedListBox.CheckedItems)
            {
                string value = experiment.Substring(3, experiment.IndexOf(" - ") - 3);
                experimentIDs.Add(Convert.ToInt32(value));
            }
            this.Close();
        }
        
    }
}
