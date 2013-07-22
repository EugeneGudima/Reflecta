using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reflecta2._0
{
    class Export
    {
        public static async void ExportData(ADatabase db, List<int> experiments)
        {
            List<int> test = ADatabaseWorker.GetAllParametersIDs(db);
            foreach (int experiment in experiments)
            {               
                List<string> writeData = ADatabaseWorker.GetAllDataForExport(db, experiment, test);
                if (!Directory.Exists("export"))
                    Directory.CreateDirectory("export");
                using (StreamWriter outfile = new StreamWriter("export\\ExperimentID" + experiment.ToString() +".csv", false, Encoding.UTF8))
                {
                    string header = "experimentID; Дата; ";
                    foreach (string parameterName in ADatabase.Parameters)
                    {
                        header += parameterName+"; ";
                    }
                    await outfile.WriteAsync(header + Environment.NewLine);

                    foreach (string line in writeData)
                        await outfile.WriteAsync(line);
                }

                MessageBox.Show("Export complete!");
            }
        }
    }
}
