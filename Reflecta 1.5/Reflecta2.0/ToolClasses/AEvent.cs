using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflecta2._0
{
    class AEvent
    {
        public static void WriteWarning(DateTime timestamp, string user, string code_error, string descriphion_en, string description_ru)
        {
            try
            {
                //FileWorker.WriteEventFile(timestamp, user, code_error, descriphion_en, description_ru);
            }
            catch 
            {
                //FileWorker.WriteEventFile(timestamp, user, code_error, descriphion_en, description_ru);
            }
        }

        public static void WriteError(DateTime timestamp, string user, string code_error, string descriphion_en, string description_ru)
        {
            try
            {               
                //FileWorker.WriteEventFile(timestamp, user, code_error, descriphion_en, description_ru);
            }
            catch 
            {
                //FileWorker.WriteEventFile(timestamp, user, code_error, descriphion_en, description_ru);
            }
        }
    }
}
