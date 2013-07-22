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
    public partial class OpenExperimentsGroupsDialog : Form
    {
        List<List<string>> experimentGroupsList;
        List<string> exGroupIDsForDelete;
        List<string> exGroupIDsForOpen;

        public OpenExperimentsGroupsDialog(List<List<string>> _experimentGroupsList)
        {
            InitializeComponent();
            experimentGroupsList = _experimentGroupsList;

            foreach (List<string> line in experimentGroupsList)
            {

                if (line.Count > 7)
                    dataGridView.Rows.Add(false, line[0], line[1], line[2], line[3], line[4], line[5], line[6], line[7]);               
                
            }
        }

        public List<string> ExperimentGroupIDsForDelete
        {
            get { return exGroupIDsForDelete; }
        }

        public List<string> ExperimentGroupIDsForOpen
        {
            get { return exGroupIDsForOpen; }
        }

        private void OpenExperimentGroup_btn_Click(object sender, EventArgs e)
        {
            List<string> list = GetSelectedExperimentGroups();
            if(exGroupIDsForOpen != null)
                exGroupIDsForOpen.AddRange(list);
            else
                exGroupIDsForOpen = list;
            this.Close();
        }

        private void DeleteExperimentGroup_btn_Click(object sender, EventArgs e)
        {
            List<string> list = GetSelectedExperimentGroups();
            if (exGroupIDsForDelete != null)
                exGroupIDsForDelete.AddRange(list);
            else
                exGroupIDsForDelete = list;
            this.Close();
        }

        private List<string> GetSelectedExperimentGroups()
        {
            if (dataGridView.Rows != null)
            {
                List<string> result = new List<string>();
                for (int i = 0; i < dataGridView.Rows.Count; i++)
                {
                    if ((Convert.ToBoolean(dataGridView.Rows[i].Cells["Select"].Value) == true) && experimentGroupsList[0].Count > 7)
                        result.Add(experimentGroupsList[i][0]);
                    
                }
                return result;
            }
            else return null;
        }

    }
}
