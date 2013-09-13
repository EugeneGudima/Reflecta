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
        public static /*async*/ void ExportData(ADatabase db, List<int> experiments, bool showMessagebox = true)
        {
            try
            {
                
                foreach (int experiment in experiments)
                {
                    List<int> test = ADatabaseWorker.GetParametersForExport(db, experiment);
                    List<string> writeData = ADatabaseWorker.GetAllDataForExport(db, experiment, test);
                    if (!Directory.Exists("export"))
                        Directory.CreateDirectory("export");
                    using (StreamWriter outfile = new StreamWriter("export\\ExperimentID" + experiment.ToString() + ".csv", false, Encoding.UTF8))
                    {
                        string header = "experimentID; Серийный номер датчика; Параметр калибровки; Дата; ";
                        foreach (string parameterName in ADatabase.Parameters)
                        {
                            header += parameterName + "; ";
                        }
                        //await outfile.WriteAsync(header + Environment.NewLine);
                        outfile.Write(header + Environment.NewLine);

                        List<string> experimentInfo = ADatabaseWorker.GetExperimentIDInfo(db, experiment);
                        foreach (string line in writeData)
                        {
                            string lineToWrite = "";
                            if (experimentInfo.Count == 3)
                                lineToWrite = experimentInfo[0] + "; " + experimentInfo[1] + "; " + experimentInfo[2] + "; ";
                            else
                                lineToWrite = "null; null; null; ";
                            lineToWrite += line;
                            //await outfile.WriteAsync(lineToWrite);
                            outfile.Write(lineToWrite);
                        }
                    }
                    if (showMessagebox)
                        MessageBox.Show("Export complete!");
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "Export", "ExportData", ex.Message);
            }

        }
    }
}
