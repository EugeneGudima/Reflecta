using Npgsql;
using NpgsqlTypes;
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
            NpgsqlCommand command = new NpgsqlCommand(sql, db.Connection);
            command.ExecuteNonQuery();

            sql = "SELECT currval(pg_get_serial_sequence('CalibrationGroup', 'calibrationgroupid'));";
            command = new NpgsqlCommand(sql, db.Connection);
            NpgsqlDataReader myreader = command.ExecuteReader();
            if (myreader.Read())
            {
                int result = Convert.ToInt32(myreader[0].ToString());
                myreader.Close();
                return result;
            }
            else
            {
                myreader.Close();
                return -1;
            }
        }                

        internal static void WriteCalibrationData(ADatabase db, int calibrationGroup, string realValue, string measureValue)
        {
            string sql = "INSERT INTO Calibration (RealValue , MeasureValue, CalibrationGroup) VALUES ("
                + realValue.ToString().Replace(",", ".") + "," + realValue.ToString().Replace(",", ".")
                + ", " + calibrationGroup.ToString() + ")";
            NpgsqlCommand command = new NpgsqlCommand(sql, db.Connection);
            command.ExecuteNonQuery();

        }

        internal static List<List<string>> GetAllCalibrationGroup(ADatabase db)
        {
            List<List<string>> result = new List<List<string>>();
            List<string> line = new List<string>();
            string sql = "select * from CalibrationGroup";
            NpgsqlCommand command = new NpgsqlCommand(sql, db.Connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                line = new List<string>();
                if(reader.FieldCount == 5)
                {
                    line.Add(reader[0].ToString());
                    line.Add(reader[1].ToString());
                    line.Add(reader[2].ToString());
                    line.Add(reader[3].ToString());
                    line.Add(reader[4].ToString());
                    result.Add(line);
                }
            }
            reader.Close();
            return result;
        }
    }
}
