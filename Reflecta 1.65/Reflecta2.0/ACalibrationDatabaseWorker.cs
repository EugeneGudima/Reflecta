using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Reflecta2._0
{
    class ACalibrationDatabaseWorker
    {
        internal static int WriteCalibrationGroup(ADatabase db, int average, double firmware, string serial)
        {
            NumberFormatInfo formatInfo = new NumberFormatInfo();
            formatInfo.NumberDecimalSeparator = ".";

            string sql = "INSERT INTO CalibrationGroup (NumOfAverage, Datetime, SensorSerial, SensorFirmware) VALUES (" + average.ToString() + ",'" + DateTime.Now.ToString("dd.MM.yy HH:mm:ss.ff") + "', '" + serial + "', " + firmware.ToString(formatInfo) + ")";
            FbCommand command = new FbCommand(sql, db.Connection);
            command.ExecuteNonQuery();

            

            sql = "select gen_id(generator_CalibrationGroupID, 0) from rdb$Database;";
            command = new FbCommand(sql, db.Connection);
            FbDataReader myreader = command.ExecuteReader();
            if (myreader.Read())
                return Convert.ToInt32(myreader[0].ToString());
            else
                return -1;
        }                

        internal static void WriteCalibrationData(ADatabase db, int calibrationGroup, string realValue, string measureValue)
        {
            string sql = "INSERT INTO Calibration (RealValue , MeasureValue, CalibrationGroup) VALUES ("
                + realValue.ToString().Replace(",", ".") + "," + realValue.ToString().Replace(",", ".")
                + ", " + calibrationGroup.ToString() + ")";
            FbCommand command = new FbCommand(sql, db.Connection);
            command.ExecuteNonQuery();

        }

        internal static List<List<string>> GetAllCalibrationGroup(ADatabase db)
        {
            List<List<string>> result = new List<List<string>>();
            List<string> line = new List<string>();
            string sql = "select * from CalibrationGroup";
            FbCommand command = new FbCommand(sql, db.Connection);
            FbDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                line = new List<string>();
                if(reader.FieldCount == 5)
                {
                    line.Add(reader.GetString(0));
                    line.Add(reader.GetString(1));
                    line.Add(reader.GetString(2));
                    line.Add(reader.GetString(3));
                    line.Add(reader.GetString(4));
                    result.Add(line);
                }
            }

            return result;
        }
    }
}
