using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reflecta2._0
{
    class ADatabaseWorker
    {
        static int limitForPointView = 6000; //количество точек для отображения на Zedgraph
        public static void WriteBaseblocks(ADatabase db, ref List<ABaseblock> baseblocks)
        {
            try
            {
                for (int i = 0; i < baseblocks.Count; i++)
                {
                    int baseblockID = ADatabaseWorker.GetBaseBlockID(db, baseblocks[i].baseblockSerial);
                    if (baseblockID == -1)
                    {
                        string sql = "INSERT INTO BASEBLOCKS (Comport, BaseblockSerial) VALUES ('" + baseblocks[i].comport + "','" + baseblocks[i].baseblockSerial + "')";
                        NpgsqlCommand command = new NpgsqlCommand(sql, db.Connection);
                        command.ExecuteNonQuery();
                        sql = "SELECT currval(pg_get_serial_sequence('BASEBLOCKS', 'baseblockid'));";
                        NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                        NpgsqlDataReader myreader = readCommand.ExecuteReader();
                        if(myreader.Read())
                            baseblocks[i].baseblockID = Convert.ToInt32(myreader[0].ToString());
                        myreader.Close();
                    }
                    else
                    {
                        baseblocks[i].baseblockID = baseblockID;
                    }
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "WriteBaseblocks", ex.Message);
            }
        }

        private static int GetBaseBlockID(ADatabase db, string baseblockSerial)
        {
            try
            {
                string sql = "SELECT BaseblockID FROM Baseblocks Where BaseblockSerial='" + baseblockSerial + "'";
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();
                if (myreader.Read())
                {
                    // load the combobox with the names of the people inside.
                    // myreader[0] reads from the 1st Column
                    int result = Convert.ToInt32(myreader[0]);
                    myreader.Close(); // we are done with the reader
                    return result;
                }
                else
                {
                    myreader.Close();
                    return -1;
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetBaseBlockID", ex.Message);
                return -1;
            }
        }


        public static List<string> GetAllParameters(ADatabase db)
        {
            try
            {
                List<string> result = new List<string>();
                string sql = "SELECT ParamName FROM PARAMETERS";
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();
                while (myreader.Read())
                {
                    // load the combobox with the names of the people inside.
                    // myreader[0] reads from the 1st Column
                    result.Add(myreader[0].ToString());
                }
                myreader.Close(); // we are done with the reader
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetAllParameters", ex.Message);
                return null;
            }
        }

        public static List<List<string>> GetAllExperimentGroups(ADatabase db, bool getNPoints = false)
        {
            try
            {
                List<List<string>> result = new List<List<string>>();
                string countQuery = "";
                if(getNPoints)
                    countQuery = "(select Count(*) as pointcount from parametersvalues where  parametersvalues.experimentid = (select experimentid from experiments where experiments.experimentgroupsID = experimentgroups.experimentgroupsID FETCH FIRST ROW ONLY)  and parametersvalues.parametersid = 1),";


                string sql = "SELECT  experimentgroups.experimentgroupsid, experimentgroups.datestart, experimentgroups.datestop - experimentgroups.datestart,(select Count(*) from experiments where experimentgroups.experimentgroupsid = experiments.experimentgroupsid) as cn , " +countQuery +" experimentgroups.powerdelay, experimentgroups.commdelay, experimentgroups.description FROM ExperimentGroups order by experimentgroups.experimentgroupsid";
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();
                List<string> line;
                while (myreader.Read())
                {
                    line = new List<string>();
                    // load the combobox with the names of the people inside.
                    // myreader[0] reads from the 1st Column
                    for (int i = 0; i < myreader.FieldCount; i++)
                    {
                        line.Add(myreader[i].ToString());
                    }

                    result.Add(line);
                }
                myreader.Close(); // we are done with the reader
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetAllExperimentGroups", ex.Message);
                return null;
            }
        }       

        internal static void WriteChannels(ADatabase db, ref List<ABaseblock> baseblockInfo)
        {
            try
            {
                int count = 0;
                foreach (ABaseblock baseblock in baseblockInfo)
                {
                    List<int> listSysChannelIDs = new List<int>();
                    foreach (byte channel in baseblock.channelsWithSensors)
                    {
                        int channelID = GetSysChannelID(db, baseblock.baseblockID, channel);
                        if (channelID == -1)
                        {
                            string sql = "INSERT INTO Channels (BaseblockID, BaseblockChannel) VALUES (" + baseblock.baseblockID + "," + channel + ")";
                            NpgsqlCommand command = new NpgsqlCommand(sql, db.Connection);
                            command.ExecuteNonQuery();

                            sql = "SELECT currval(pg_get_serial_sequence('Channels', 'syschannelid'));";
                            NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                            NpgsqlDataReader myreader = readCommand.ExecuteReader();
                            myreader.Read();
                            listSysChannelIDs.Add(Convert.ToInt32(myreader[0].ToString()));
                            myreader.Close();
                        }
                        else
                            listSysChannelIDs.Add(channelID);
                    }
                    baseblockInfo[count].sysChannelsIDs = listSysChannelIDs;
                    count++;
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "WriteChannels", ex.Message);
            }
        }

        private static int GetSysChannelID(ADatabase db, int baseblockID, byte channel)
        {
            try
            {
                string sql = "SELECT SysChannelID FROM Channels Where BaseblockID=" + baseblockID.ToString() + " AND BaseblockChannel=" + channel;
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();
                if (myreader.Read())
                {
                    // load the combobox with the names of the people inside.
                    // myreader[0] reads from the 1st Column
                    int result = Convert.ToInt32(myreader[0]);
                    myreader.Close(); // we are done with the reader
                    return result;
                }
                else
                {
                    myreader.Close();
                    return -1;
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetSysChannelID", ex.Message);
                return -1;
            }
        }

        internal static void WriteExperiments(ADatabase db, ref int experimentGroup, List<ABaseblock> baseblockInfo, DataGridViewRowCollection calibvalues, int p1, int p2, string p3)
        {
            try
            {
                int groupID = WriteExperimentGroup(db, p1, p2, p3);
                foreach (ABaseblock baseblock in baseblockInfo)
                {
                    int count = 0;
                    foreach (int sysChannel in baseblock.sysChannelsIDs)
                    {
                        int experimentID = WriteExperiment(db, groupID, sysChannel, baseblock.comport, baseblock.channelsWithSensors, calibvalues[count].Cells[1].Value, count);
                        if (experimentID <= 0)
                        {
                        }
                        count++;
                    }

                }
                experimentGroup = groupID;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "WriteExperiments", ex.Message);
            }
        }

        private static int WriteExperiment(ADatabase db, int groupID, int sysChannel, string comport, List<byte> channelsWithSensors, object calibValue, int i)
        {
            try
            {
                ADriver dr = new ADriver(comport, 50);
                string error = "";
                if (dr.OpenPort(ref error))
                {
                    if (dr.SetChannel(channelsWithSensors[i], ref error))
                    {
                        string serial = dr.GetSerial(ref error);
                        if (error != "")
                        {
                            return -1;
                        }
                        double firmware = dr.GetFirmware(ref error);
                        if (error != "")
                        {
                            return -2;
                        }

                        if (!dr.ClosePort())
                        {
                            return -3;
                        }

                        string calibValueString;
                        if (calibValue != null)
                            calibValueString = ConvertToDoubleWithCheck(calibValue.ToString(), -1).ToString();
                        else
                            calibValueString = "null";

                        string sql = "INSERT INTO Experiments (ExperimentGroupsID, SysChannelID, SensorSerial, SensorFirmware, CalibrationValue) VALUES (" + groupID + ","
                            + sysChannel + "," + serial + "," + firmware + "," + calibValueString + ")";
                        NpgsqlCommand command = new NpgsqlCommand(sql, db.Connection);
                        command.ExecuteNonQuery();

                        sql = "SELECT currval(pg_get_serial_sequence('Experiments', 'experimentid'));";
                        NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                        NpgsqlDataReader myreader = readCommand.ExecuteReader();
                        myreader.Read();
                        int result = Convert.ToInt32(myreader[0].ToString());
                        myreader.Close();
                        return result;
                    }
                    else
                    {
                        return -4;
                    }

                }
                else
                {
                    return -5;
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "WriteExperiment", ex.Message);
                return -1;
            }

        }

        private static int WriteExperimentGroup(ADatabase db, int p1, int p2, string p3)
        {
            try
            {
                string sql = "INSERT INTO ExperimentGroups (DateStart, PowerDelay, CommDelay, Description) VALUES ('" + DateTime.Now.ToString("dd.MM.yy HH:mm:ss.ff") + "'," + p1 + "," + p2 + ",'" + p3 + "')";
                NpgsqlCommand command = new NpgsqlCommand(sql, db.Connection);
                command.ExecuteNonQuery();

                sql = "SELECT currval(pg_get_serial_sequence('ExperimentGroups', 'experimentgroupsid'));";
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();
                myreader.Read();
                int result = Convert.ToInt32(myreader[0].ToString());
                myreader.Close();
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "WriteExperimentGroup", ex.Message);
                return -1;
            }

        }

        private static void WriteParameter(ADatabase db, DateTime dt, int expID, double p, ParametersID parametersID)
        {
            try
            {
                NumberFormatInfo formatInfo = new NumberFormatInfo();
                formatInfo.NumberDecimalSeparator = ".";
                string sql = "INSERT INTO ParametersValues (ParametersID, ExperimentID, Datetime, ParameterValue) VALUES (" + ((int)parametersID).ToString() + "," + expID + ",'" + dt.ToString("dd.MM.yy HH:mm:ss.ff") + "'," + p.ToString(formatInfo) + ")";
                NpgsqlCommand command = new NpgsqlCommand(sql, db.Connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "WriteParameter", ex.Message);
            }
        }

        private static void WriteReflectogram(ADatabase db, DateTime dt, double[] reflectogram, int expID)
        {
            try
            {
                string sql = "INSERT INTO Reflectograms (ExperimentID, Datetime, ReflData) VALUES (" + expID + ",'" + dt.ToString("dd.MM.yy HH:mm:ss.ff") + "', '" + ConvertToArrayString(reflectogram) + "')";
                NpgsqlCommand command = new NpgsqlCommand(sql, db.Connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "WriteReflectogram", ex.Message);
            }
        }

        private static string ConvertToArrayString(double[] reflectogram)
        {
            try
            {
                if (reflectogram != null)
                {
                    string result = "{";
                    int count = 0;
                    foreach (double number in reflectogram)
                    {
                        if (count == reflectogram.Length - 1)
                            result += Math.Round(number, 2).ToString().Replace(',', '.');
                        else
                            result += Math.Round(number, 2).ToString().Replace(',', '.') + ", ";
                        count++;
                    }
                    result += "}";
                    return result;
                }
                else
                    return "{}";
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "ConvertToArrayString", ex.Message);
                return "{}";
            }
        }

        private static int GetExperimentsID(ADatabase db, int experimentGroup, int sysChannel)
        {
            try
            {
                string sql = "SELECT ExperimentID FROM Experiments  Where SysChannelID=" + sysChannel + " AND ExperimentGroupsID=" + experimentGroup;
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();
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
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetExperimentsID", ex.Message);
                return -1;
            }
        }

        internal static int GetExperimentIDForDisplay(ADatabase db, int experimentGroupID, KeyValuePair<string, byte> channel1)
        {
            try
            {
                string sql = "Select ExperimentId FROM experiments Where experiments.experimentgroupsid =" + experimentGroupID + " AND experiments.syschannelid =  (select sysChannelID  From channels Where baseblockChannel=" + channel1.Value + ")";
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();
                myreader.Read();
                int result = Convert.ToInt32(myreader[0].ToString());
                myreader.Close();
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetExperimentIDForDisplay", ex.Message);
                return -1;
            }
        }

        internal static void GetData(ADatabase db, ref ZedGraph.PointPairList data, int experimentGroup, string key, int experimentChannel, int parameterID, ref double min, ref double max, bool limitFlag)
        {
            try
            {
                if( data == null)
                    data = new ZedGraph.PointPairList();

                int comStart = key.IndexOf('(') + 1;
                int comEnd = key.IndexOf(')') - comStart;
                string comport = key.Substring(comStart, comEnd);
                string baseblockSerial = key.Substring(0, comStart - 2);

                string lastDate = "";
                if (data.Count > 0)
                    lastDate = DateTime.FromOADate(data.Last().X).ToString("dd.MM.yy HH:mm:ss.ff");
                else
                    lastDate = DateTime.MinValue.ToString("dd.MM.yy HH:mm:ss.ff");

                
                string sql = "select parametersvalues.datetime, parametersvalues.parametervalue from parametersvalues where parametersvalues.parametersid="
                              + parameterID.ToString() + " and parametersvalues.experimentid=(select experiments.experimentid from experiments where experiments.experimentgroupsid="
                              + experimentGroup.ToString() + " and experiments.syschannelid=(select channels.syschannelid from channels where channels.baseblockchannel="
                              + experimentChannel.ToString() + "and channels.baseblockid=(select baseblocks.baseblockid from baseblocks where baseblocks.comport='" + comport + "' and baseblocks.baseblockserial='" + baseblockSerial + "')))"
                              + " and parametersvalues.datetime > '" + lastDate + "'::timestamp" + " order by parametersvalues.datetime desc";
              
                if(!limitFlag)
                   sql += " limit " + limitForPointView.ToString();

                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();               
                while (myreader.Read())
                {
                    if (Convert.ToDouble(myreader[1]) != -1 && myreader.FieldCount == 2)
                    {
                        data.Add(Convert.ToDateTime(myreader[0]).ToOADate(), Convert.ToDouble(myreader[1]));                       
                    }
                      
                }
                myreader.Close();
                data.Sort(ZedGraph.SortType.XValues);
                if (!limitFlag)
                {
                    if (data.Count > limitForPointView)
                        data.RemoveRange(0, data.Count - limitForPointView);
                }
                min = data.Min(value => value.X);
                max = data.Max(value => value.X);               
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetData", ex.Message);               
            }
        }

        internal static void WriteSensorStatistic(ADatabase db, DateTime dt, Sensor sensor, ABaseblock device, int experimentGroup, int count)
        {
            try
            {
                int expID = GetExperimentsID(db, experimentGroup, device.sysChannelsIDs[count]);
                WriteParameter(db, dt, expID, sensor.SensorCalcLevel, ParametersID.LevelSensor);
                WriteParameter(db, dt, expID, sensor.SensorTemperature, ParametersID.PlateTemperature);
                if (sensor.SensorDelays != null)
                {
                    WriteParameter(db, dt, expID, sensor.SensorDelays[0], ParametersID.ZondPositionSensor);
                    WriteParameter(db, dt, expID, sensor.SensorDelays[1], ParametersID.OtrPositionSensor);
                    WriteParameter(db, dt, expID, sensor.SensorDelays[2], ParametersID.SecondZondPositionSensor);
                }
                WriteParameter(db, dt, expID, sensor.ComputerCalcLevel, ParametersID.LevelComputer);
                if (sensor.ComputerDelays != null)
                {
                    WriteParameter(db, dt, expID, sensor.ComputerDelays[0], ParametersID.ZondPositionComputer);
                    WriteParameter(db, dt, expID, sensor.ComputerDelays[1], ParametersID.OtrPositionComputer);
                    WriteParameter(db, dt, expID, sensor.ComputerDelays[2], ParametersID.SecondZondPositionComputer);
                }
                if (sensor.ComputerAmplitudes != null)
                {
                    WriteParameter(db, dt, expID, sensor.ComputerAmplitudes[0], ParametersID.FirstZondAmplitude);
                    WriteParameter(db, dt, expID, sensor.ComputerAmplitudes[1], ParametersID.OtrAmplitude);
                    WriteParameter(db, dt, expID, sensor.ComputerAmplitudes[2], ParametersID.SecondZondAmplitude);
                }
                if (sensor.TemperatureArray != null)
                    WriteTemperatureHanger(db, dt, expID, sensor.TemperatureArray);
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "WriteSensorStatistic", ex.Message);
            }
        }

        private static void WriteTemperatureHanger(ADatabase db, DateTime dt, int expID, double[] temperatureHanger)
        {
            try
            {
                NumberFormatInfo formatInfo = new NumberFormatInfo();
                formatInfo.NumberDecimalSeparator = ".";
                string sql = "INSERT INTO temperaturehangers (ExperimentID, Datetime, temperaturedata) VALUES (" + expID.ToString() + ",'" + dt.ToString("dd.MM.yy HH:mm:ss.ff") + "', '" + ConvertToArrayString(temperatureHanger) + "')";
                NpgsqlCommand command = new NpgsqlCommand(sql, db.Connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "WriteParameter", ex.Message);
            }
        }

        internal static void WriteSensorReflectogram(ADatabase db, DateTime dt, Sensor sensor, ABaseblock device, int experimentGroup, int count)
        {
            try
            {
                int expID = GetExperimentsID(db, experimentGroup, device.sysChannelsIDs[count]);
                WriteReflectogram(db, dt, sensor.Reflectogram, expID);
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "WriteSensorReflectogram", ex.Message);
            }
        }

        internal static List<string> GetAllReflectograms(ADatabase db, int experimentGroup, string key, byte experimentChannel)
        {
            try
            {
                int comStart = key.IndexOf('(') + 1;
                int comEnd = key.IndexOf(')') - comStart;
                string comport = key.Substring(comStart, comEnd);
                string baseblockSerial = key.Substring(0, comStart - 2);

                List<string> result = new List<string>();
                string sql = "select reflectograms.datetime from reflectograms where reflectograms.experimentid=(select experiments.experimentid from experiments where experiments.experimentgroupsid="
                    + experimentGroup.ToString() + " and experiments.syschannelid=(select channels.syschannelid from channels where channels.baseblockchannel="
                    + experimentChannel.ToString() + " and channels.baseblockid=(select baseblocks.baseblockid from baseblocks where baseblocks.comport='" + comport + "' and baseblocks.baseblockserial='" + baseblockSerial + "')))" + " order by reflectograms.datetime";
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();
                while (myreader.Read())
                {
                    DateTime date = Convert.ToDateTime(myreader[0]);
                    result.Add(date.ToString("dd.MM.yy HH:mm:ss.ff"));
                }
                myreader.Close();
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetAllReflectograms", ex.Message);
                return null;
            }
        }


        internal static List<string> GetLastReflectograms(ADatabase db, int experimentGroup, string key, byte experimentChannel)
        {
            try
            {
                int comStart = key.IndexOf('(') + 1;
                int comEnd = key.IndexOf(')') - comStart;
                string comport = key.Substring(comStart, comEnd);
                string baseblockSerial = key.Substring(0, comStart - 2);

                List<string> result = new List<string>();
                string sql = "select reflectograms.datetime from reflectograms where reflectograms.experimentid=(select experiments.experimentid from experiments where experiments.experimentgroupsid="
                    + experimentGroup.ToString() + " and experiments.syschannelid=(select channels.syschannelid from channels where channels.baseblockchannel="
                    + experimentChannel.ToString() + " and channels.baseblockid=(select baseblocks.baseblockid from baseblocks where baseblocks.comport='" + comport + "' and baseblocks.baseblockserial='" + baseblockSerial + "')))" + " order by reflectograms.datetime desc limit 1";
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();
                while (myreader.Read())
                {
                    DateTime date = Convert.ToDateTime(myreader[0]);
                    result.Add(date.ToString("dd.MM.yy HH:mm:ss.ff"));
                }
                myreader.Close();
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetLastReflectograms", ex.Message);
                return null;
            }
        }

        internal static ZedGraph.PointPairList GetReflectogramData(ADatabase db, int experimentGroup, string key, byte experimentChannel, string time)
        {
            try
            {
                int comStart = key.IndexOf('(') + 1;
                int comEnd = key.IndexOf(')') - comStart;
                string comport = key.Substring(comStart, comEnd);
                string baseblockSerial = key.Substring(0, comStart - 2);

                string sql = "select reflectograms.refldata from reflectograms where reflectograms.datetime='" + time + "'"
                            + " and reflectograms.experimentid=(select experiments.experimentid from experiments where experiments.experimentgroupsid="
                            + experimentGroup.ToString() + " and experiments.syschannelid=(select channels.syschannelid from channels where channels.baseblockchannel="
                            + experimentChannel.ToString() + "and channels.baseblockid=(select baseblocks.baseblockid from baseblocks where baseblocks.comport='" + comport + "' and baseblocks.baseblockserial='" + baseblockSerial + "')))";
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();
                if (myreader.Read())
                {
                    double[] refldata = myreader[0] as double[];
                    myreader.Close();
                    return ReflectogramDataToPointPairList(refldata);
                }
                else
                {
                    myreader.Close();
                    return null;
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetReflectogramData", ex.Message);
                return null;
            }
        }

        private static ZedGraph.PointPairList ReflectogramDataToPointPairList(double[] refldata)
        {
            try
            {
                ZedGraph.PointPairList result = new ZedGraph.PointPairList();
                int count = 0;
                foreach (double point in refldata)
                {
                    result.Add(count, point);
                    count++;
                }
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "ReflectogramDataToPointPairList", ex.Message);
                return null;
            }
        }

        private static double ConvertToDoubleWithCheck(string str, double errorValue)
        {
            try
            {
                NumberFormatInfo formatInfo = (NumberFormatInfo)CultureInfo.GetCultureInfo("en-US").NumberFormat.Clone();
                if (str.Contains('.'))
                    formatInfo.NumberDecimalSeparator = ".";
                else
                    formatInfo.NumberDecimalSeparator = ",";

                if (str != null)
                {
                    double result = 0.0;
                    try
                    {
                        result = Convert.ToDouble(str, formatInfo);
                        return result;
                    }
                    catch
                    {

                        return errorValue;
                    }
                }
                else
                {
                    return errorValue;
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "ConvertToDoubleWithCheck", ex.Message);
                return errorValue;
            }
        }

        internal static double GetTemperature(ADatabase db, int experimentGroup, string key, byte experimentChannel, string time)
        {
            try
            {
                if (key != null)
                {
                    int comStart = key.IndexOf('(') + 1;
                    int comEnd = key.IndexOf(')') - comStart;
                    string comport = key.Substring(comStart, comEnd);
                    string baseblockSerial = key.Substring(0, comStart - 2);

                    string sql = "select parametersvalues.parametervalue from  parametersvalues where parametersvalues.datetime= '" + time + "' "
                                + " and parametersvalues.parametersid=" + ((int)ParametersID.PlateTemperature).ToString() + " and  parametersvalues.experimentid=(select experiments.experimentid from experiments where experiments.experimentgroupsid="
                                + experimentGroup.ToString() + " and experiments.syschannelid=(select channels.syschannelid from channels where channels.baseblockchannel="
                                + experimentChannel.ToString() + "and channels.baseblockid=(select baseblocks.baseblockid from baseblocks where baseblocks.comport='" + comport + "' and baseblocks.baseblockserial='" + baseblockSerial + "')))";
                    NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                    NpgsqlDataReader myreader = readCommand.ExecuteReader();
                    if (myreader.Read())
                    {
                        double result = ConvertToDoubleWithCheck(myreader[0].ToString(), -1);
                        myreader.Close();
                        return result;
                    }
                    else
                    {
                        myreader.Close();
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetTemperature", ex.Message);
                return -1;
            }
        }

        internal static void DeleteExperimentData(ADatabase db, List<string> list)
        {
            try
            {
                foreach (string experimentgroup in list)
                {
                    List<int> experiments = ADatabaseWorker.GetExperimentIDs(db, Convert.ToInt32(experimentgroup));
                    foreach (int experiment in experiments)
                    {
                        ADatabaseWorker.DeleteExperimentReflectogramsAndData(db, experiment);
                    }
                    ADatabaseWorker.DeleteExperimentGroup(db, Convert.ToInt32(experimentgroup));
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "DeleteExperimentData", ex.Message);
            }
        }

        private static void DeleteExperimentGroup(ADatabase db, int experiment)
        {
            try
            {
                string sql = "DELETE  FROM ExperimentGroups WHERE ExperimentGroups.ExperimentGroupsID=" + experiment.ToString();
                NpgsqlCommand command = new NpgsqlCommand(sql, db.Connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "DeleteExperimentGroup", ex.Message);
            }
        }

        public static List<int> GetExperimentIDs(ADatabase db, int experimentGroup)
        {
            try
            {
                List<int> result = new List<int>();

                string sql = "SELECT ExperimentID FROM Experiments  Where ExperimentGroupsID=" + experimentGroup;
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();

                while (myreader.Read())
                {
                    result.Add(Convert.ToInt32(myreader[0].ToString()));
                }
                myreader.Close();
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetExperimentIDs", ex.Message);
                return null;
            }
        }

        private static void DeleteExperimentReflectogramsAndData(ADatabase db, int experiment)
        {
            try
            {
                NpgsqlTransaction t = db.Connection.BeginTransaction();

                string sql = "DELETE  FROM Reflectograms WHERE Reflectograms.ExperimentID=" + experiment.ToString();
                NpgsqlCommand command = new NpgsqlCommand(sql, db.Connection);
                command.ExecuteNonQuery();

                sql = "DELETE  FROM ParametersValues WHERE ParametersValues.ExperimentID=" + experiment.ToString();
                command = new NpgsqlCommand(sql, db.Connection);
                command.ExecuteNonQuery();

                sql = "DELETE  FROM TemperatureHangers WHERE TemperatureHangers.ExperimentID=" + experiment.ToString();
                command = new NpgsqlCommand(sql, db.Connection);
                command.ExecuteNonQuery();

                sql = "DELETE  FROM Experiments WHERE Experiments.ExperimentID=" + experiment.ToString();
                command = new NpgsqlCommand(sql, db.Connection);
                command.ExecuteNonQuery();

                t.Commit();
                
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "DeleteExperimentReflectogramsAndData", ex.Message);
            }


        }

        internal static List<string> GetBaseblockAndChannelsInfo(ADatabase db, int experimentGroup)
        {
            try
            {
                List<string> result = new List<string>();

                string sql = "select baseblocks.baseblockserial, baseblocks.comport,  channels.baseblockchannel, experiments.sensorserial From baseblocks, channels, experiments where experiments.experimentgroupsid ="
                + experimentGroup + " and channels.syschannelid=experiments.syschannelid and baseblocks.baseblockid=channels.baseblockid";
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();
                while (myreader.Read())
                {
                    if (myreader.FieldCount == 4)
                        result.Add(myreader[0] + " (" + myreader[1] + ") - CH " + myreader[2] + " Д:" + myreader[3].ToString());
                }
                myreader.Close();
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetBaseblockAndChannelsInfo", ex.Message);
                return null;
            }
        }

        internal static List<string> GetExperimentGroupInfo(ADatabase db, int experimentGroup)
        {
            try
            {
                List<string> result = new List<string>();

                string sql = "SELECT  experimentgroups.experimentgroupsid, experimentgroups.datestart, experimentgroups.datestop-experimentgroups.datestart, experimentgroups.powerdelay, experimentgroups.commdelay, experimentgroups.description, (select Count(*) from experiments where experimentgroups.experimentgroupsid = experiments.experimentgroupsid) as cn FROM ExperimentGroups where experimentgroups.experimentgroupsid = "
                + experimentGroup.ToString();
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();
                if (myreader.Read())
                {
                    if (myreader.FieldCount == 7)
                    {
                        result.Add(myreader[0].ToString());
                        result.Add(myreader[1].ToString());
                        result.Add(myreader[2].ToString());
                        result.Add(myreader[3].ToString());
                        result.Add(myreader[4].ToString());
                        result.Add(myreader[5].ToString());
                        result.Add(myreader[6].ToString());
                    }
                }
                myreader.Close();
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetExperimentGroupInfo", ex.Message);
                return null;
            }
        }

        internal static bool WriteGroupExperimentEndDate(ADatabase db, int experimentGroup)
        {
            try
            {
                string sql = "UPDATE experimentgroups SET datestop='" + DateTime.Now.ToString("dd.MM.yy HH:mm:ss.ff") + "' where  experimentgroups.experimentgroupsid=" + experimentGroup.ToString();
                NpgsqlCommand command = new NpgsqlCommand(sql, db.Connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "WriteGroupExperimentEndDate", ex.Message);
                return false;
            }
        }

        internal static List<string> GetExperimentIDsInfo(ADatabase db, int experimentGroup)
        {
            try
            {
                List<string> result = new List<string>();

                string sql = "SELECT ExperimentID, baseblocks.baseblockserial, SensorSerial, SensorFirmware FROM Experiments, baseblocks, channels Where baseblocks.baseblockid = channels.baseblockid and experiments.syschannelid = channels.syschannelid and ExperimentGroupsID=" + experimentGroup;
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();

                while (myreader.Read())
                {
                    if (myreader.FieldCount == 4)
                        result.Add("ID: " + myreader[0].ToString() + " - ББ: " + myreader[1].ToString() +
                            "; Серийный номер датч.: " + myreader[2].ToString() + "; Прошивка датч.: " + myreader[3].ToString());
                }
                myreader.Close();
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetExperimentIDsInfo", ex.Message);
                return null;
            }
        }

        internal static List<string> GetExperimentIDInfo(ADatabase db, int experiment)
        {
            try
            {
                List<string> result = new List<string>();

                string sql = "SELECT ExperimentID, SensorSerial, CalibrationValue FROM Experiments Where ExperimentID=" + experiment;
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();

                if (myreader.Read())
                {
                    if (myreader.FieldCount == 3)
                    {
                        result.Add(myreader[0].ToString());
                        result.Add(myreader[1].ToString());
                        result.Add(myreader[2].ToString());
                    }
                }
                myreader.Close();
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetExperimentIDInfo", ex.Message);
                return null;
            }
        }

        internal static List<string> GetAllDataForExport(ADatabase db, int experiment, List<int> parametersIDs)
        {
            try
            {
                List<string> result = new List<string>();

                string sql = "select ";
                if (parametersIDs.Count > 0)
                {
                    for (int i = 1; i <= parametersIDs.Count; i++)
                    {
                        if (i == 1)
                        {
                            sql += "T" + i.ToString() + ".datetime, T" + i.ToString() + ".parametervalue,";
                        }
                        else if (i == parametersIDs.Count)
                        {
                            sql += "T" + i.ToString() + ".parametervalue";
                        }
                        else
                        {
                            sql += "T" + i.ToString() + ".parametervalue,";
                        }

                    }
                    sql += " from ";
                    int count = 1;
                    foreach (int parameterID in parametersIDs)
                    {
                        if (count != parametersIDs.Count)
                            sql += "(select parametersvalues.datetime, parametersvalues.parametervalue from parametersvalues where parametersvalues.parametersid ="
                             + parameterID + " and parametersvalues.experimentid =" + experiment + ") T" + count.ToString() + ",";
                        else
                            sql += "(select parametersvalues.datetime, parametersvalues.parametervalue from parametersvalues where parametersvalues.parametersid ="
                             + parameterID + " and parametersvalues.experimentid =" + experiment + ") T" + count.ToString();
                        count++;
                    }
                    sql += " where ";
                    for (int i = 1; i <= parametersIDs.Count - 1; i++)
                    {
                        if (i == parametersIDs.Count - 1)

                            sql += "T" + i.ToString() + ".datetime = T" + (i + 1).ToString() + ".datetime";

                        else

                            sql += "T" + i.ToString() + ".datetime = T" + (i + 1).ToString() + ".datetime and ";
                    }
                    NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                    NpgsqlDataReader myreader = readCommand.ExecuteReader();
                    string line = "";
                    while (myreader.Read())
                    {
                        line = "";
                        if (myreader.FieldCount == parametersIDs.Count + 1)
                        {
                            for (int i = 0; i < myreader.FieldCount; i++)
                            {
                                line += myreader[i].ToString() + "; ";
                            }
                            result.Add(line.Replace(',', '.') + Environment.NewLine);
                        }
                    }
                    myreader.Close();
                    return result;
                }
                else
                    return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetAllDataForExport", ex.Message);
                return null;
            }
        }

        internal static List<int> GetAllParametersIDs(ADatabase db)
        {
            try
            {
                List<int> result = new List<int>();

                string sql = "select parameters.parametersid from parameters order by  parameters.parametersid";
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();

                while (myreader.Read())
                {
                    if (myreader.FieldCount > 0)
                        result.Add(Convert.ToInt32(myreader[0].ToString()));
                }
                myreader.Close();
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetAllParametersIDs", ex.Message);
                return null;
            }
        }

        internal static List<int> GetParametersForExport(ADatabase db, int experimentID)
        {
            try
            {
                List<int> result = new List<int>();

                string sql = "select distinct parametersid from parametersvalues where experimentid=" + experimentID.ToString() + " order by parametersid";
                NpgsqlCommand readCommand = new NpgsqlCommand(sql, db.Connection);
                NpgsqlDataReader myreader = readCommand.ExecuteReader();

                while (myreader.Read())
                {
                    if (myreader.FieldCount > 0)
                        result.Add(Convert.ToInt32(myreader[0].ToString()));
                }
                myreader.Close();
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "DatabaseWorker", "GetParametersForExport", ex.Message);
                return null;
            }
        }
    }
}
