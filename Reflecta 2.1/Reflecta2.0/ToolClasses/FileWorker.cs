using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Windows;

namespace Reflecta2._0
{
    class FileWorker
    { 
        public static  void WriteEventFile(DateTime timestamp, string user, string code_error, string descriphion_en, string description_ru = "")
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(timestamp.ToString());
                sb.Append(" ");
                sb.Append(user);
                sb.Append(" ");
                sb.Append(code_error);
                sb.Append(" ");
                sb.Append(descriphion_en);
                sb.Append(" ");
                sb.Append(description_ru);
                sb.AppendLine();

                using (StreamWriter outfile = new StreamWriter(Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory) + "\\Events.lg", true))
                {
                    outfile.Write(sb.ToString());
                }

            }
            catch 
            {
               
            }
        }

    }
}
