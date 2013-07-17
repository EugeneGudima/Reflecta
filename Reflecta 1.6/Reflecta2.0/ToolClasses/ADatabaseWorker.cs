using FirebirdSql.Data.FirebirdClient;
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
                        FbCommand command = new FbCommand(sql, db.Connection);
                        command.ExecuteNonQuery();
                        sql = "select gen_id(generator_basebloksID, 0) from rdb$Database;";
                        FbCommand readCommand = new FbCommand(sql, db.Connection);
                        FbDataReader myreader = readCommand.ExecuteReader();
                        myreader.Read();
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
            }
        }

        private static int GetBaseBlockID(ADatabase db, string baseblockSerial)
        {            
            string sql = "SELECT BaseblockID FROM Baseblocks Where BaseblockSerial='" + baseblockSerial + "'";
            FbCommand readCommand = new FbCommand(sql, db.Connection);
            FbDataReader myreader = readCommand.ExecuteReader();
            if(myreader.Read())
            {
                // load the combobox with the names of the people inside.
                // myreader[0] reads from the 1st Column
                int result = Convert.ToInt32(myreader[0]);
                myreader.Close(); // we are done with the reader
                return result;
            }
            else
                return -1;
        }


        public static List<string> GetAllParameters(ADatabase db)
        {
            List<string> result = new List<string>();
            string sql = "SELECT ParamName FROM PARAMETERS";
            FbCommand readCommand = new FbCommand(sql, db.Connection);
            FbDataReader myreader = readCommand.ExecuteReader();
            while (myreader.Read())
            {
                // load the combobox with the names of the people inside.
                // myreader[0] reads from the 1st Column
                result.Add(myreader[0].ToString());
            }
            myreader.Close(); // we are done with the reader
            return result;
        }

        public static List<List<string>> GetAllExperimentGroups(ADatabase db, bool getPointsCount)
        {
            List<List<string>> result = new List<List<string>>();
            string sql = "SELECT  experimentgroups.experimentgroupsid, experimentgroups.datestart, abs(datediff(minute, experimentgroups.datestop, experimentgroups.datestart)), experimentgroups.powerdelay, experimentgroups.commdelay, experimentgroups.description, (select Count(*) from experiments where experimentgroups.experimentgroupsid = experiments.experimentgroupsid) as cn FROM ExperimentGroups order by experimentgroups.experimentgroupsid"; 
            FbCommand readCommand = new FbCommand(sql, db.Connection);
            FbDataReader myreader = readCommand.ExecuteReader();
            List<string> line;            
            while (myreader.Read())
            {               
                line = new List<string>();
                // load the combobox with the names of the people inside.
                // myreader[0] reads from the 1st Column
                for (int i = 0; i < myreader.FieldCount; i++)
                {
                    if (i == 0 && getPointsCount)
                        line.Add(GetPointCountForExperimentgroup(db, myreader[i].ToString()));
                    line.Add(myreader[i].ToString());
                }

                result.Add(line);
            }
            myreader.Close(); // we are done with the reader
            return result;
        }

        private static string GetPointCountForExperimentgroup(ADatabase db, string experimentGroup)
        {
            string sql = "select Count(*) from parametersvalues where  parametersvalues.experimentid = (select first 1 experiments.experimentid from experiments where experiments.experimentgroupsid = "
                + experimentGroup + ") and parametersvalues.parametersid = 1";
            FbCommand readCommand = new FbCommand(sql, db.Connection);
            FbDataReader myreader = readCommand.ExecuteReader();
            if (myreader.Read())
            {
                // load the combobox with the names of the people inside.
                // myreader[0] reads from the 1st Column
                string result = myreader[0].ToString();
                myreader.Close(); // we are done with the reader
                return result;
            }
            else
                return null;
        }

        internal static void WriteChannels(ADatabase db, ref List<ABaseblock> baseblockInfo)
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
                        FbCommand command = new FbCommand(sql, db.Connection);
                        command.ExecuteNonQuery();

                        sql = "select gen_id(generator_channelsID, 0) from rdb$Database;";
                        FbCommand readCommand = new FbCommand(sql, db.Connection);
                        FbDataReader myreader = readCommand.ExecuteReader();
                        myreader.Read();
                        listSysChannelIDs.Add(Convert.ToInt32(myreader[0].ToString()));
                    }
                    else
                        listSysChannelIDs.Add(channelID);
                }
                baseblockInfo[count].sysChannelsIDs = listSysChannelIDs;
                count++;
            }
        }

        private static int GetSysChannelID(ADatabase db, int baseblockID, byte channel)
        {
            string sql = "SELECT SysChannelID FROM Channels Where BaseblockID=" + baseblockID.ToString() + " AND BaseblockChannel=" + channel;
            FbCommand readCommand = new FbCommand(sql, db.Connection);
            FbDataReader myreader = readCommand.ExecuteReader();
            if (myreader.Read())
            {
                // load the combobox with the names of the people inside.
                // myreader[0] reads from the 1st Column
                int result = Convert.ToInt32(myreader[0]);                
                myreader.Close(); // we are done with the reader
                return result;
            }
            else
                return -1;
        }

        internal static void WriteExperiments(ADatabase db, ref int experimentGroup, List<ABaseblock> baseblockInfo, DataGridViewRowCollection calibvalues, int p1, int p2, string p3)
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

        private static int WriteExperiment(ADatabase db, int groupID, int sysChannel, string comport, List<byte> channelsWithSensors, object calibValue, int i)
        {
            ADriver dr = new ADriver(comport, 50);
            string error = "";
            if (dr.OpenPort(ref error))
            {
                if (dr.SetChannel(channelsWithSensors[i]))
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
                        calibValueString = calibValue.ToString();
                    else
                        calibValueString = "null";

                    string sql = "INSERT INTO Experiments (ExperimentGroupsID, SysChannelID, SensorSerial, SensorFirmware, CalibrationValue) VALUES (" + groupID + "," 
                        + sysChannel + "," + serial + "," + firmware + "," + calibValueString + ")";
                    FbCommand command = new FbCommand(sql, db.Connection);
                    command.ExecuteNonQuery();

                    sql = "select gen_id(generator_experimentID, 0) from rdb$Database;";
                    FbCommand readCommand = new FbCommand(sql, db.Connection);
                    FbDataReader myreader = readCommand.ExecuteReader();
                    myreader.Read();
                    return Convert.ToInt32(myreader[0].ToString());
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

        private static int WriteExperimentGroup(ADatabase db,  int p1, int p2, string p3)
        {
            string sql = "INSERT INTO ExperimentGroups (DateStart, PowerDelay, CommDelay, Description) VALUES ('" + DateTime.Now.ToString("dd.MM.yy HH:mm:ss.ff") + "'," + p1 + "," + p2 + ",'" + p3 + "')";
            FbCommand command = new FbCommand(sql, db.Connection);
            command.ExecuteNonQuery();

            sql = "select gen_id(generator_experimentsGroupID, 0) from rdb$Database;";
            FbCommand readCommand = new FbCommand(sql, db.Connection);
            FbDataReader myreader = readCommand.ExecuteReader();
            myreader.Read();
            return Convert.ToInt32(myreader[0].ToString());

        }        
        
        private static void WriteParameter(ADatabase db, int expID, double p, ParametersID parametersID)
        {
            NumberFormatInfo formatInfo = new NumberFormatInfo();
            formatInfo.NumberDecimalSeparator = ".";
            string sql = "INSERT INTO ParametersValues (ParametersID, ExperimentID, Datetime, ParameterValue) VALUES (" + ((int)parametersID).ToString() + "," + expID + ",'" + DateTime.Now.ToString("dd.MM.yy HH:mm:ss.ff") + "'," + p.ToString(formatInfo) + ")";
            FbCommand command = new FbCommand(sql, db.Connection);
            command.ExecuteNonQuery();
        }

        private static void WriteReflectogram(ADatabase db, double[] reflectogram, int expID)
        {
            string sql = "INSERT INTO Reflectograms (ExperimentID, Datetime, ReflData) VALUES (" + expID + ",'" + DateTime.Now.ToString("dd.MM.yy HH:mm:ss.ff") + "','" + ConvertToString(reflectogram) + "')";
            FbCommand command = new FbCommand(sql, db.Connection);
            command.ExecuteNonQuery();
        }

        private static string ConvertToString(double[] reflectogram)
        {
            if (reflectogram != null)
            {
                string result = "";
                foreach (double number in reflectogram)
                    result += Math.Round(number, 2).ToString() + ' ';
                return result;
            }
            else
                return null;
        }

        private static int GetExperimentsID(ADatabase db, int experimentGroup, int sysChannel)
        {
            string sql = "SELECT ExperimentID FROM Experiments  Where SysChannelID=" + sysChannel + " AND ExperimentGroupsID=" + experimentGroup;             
            FbCommand readCommand = new FbCommand(sql, db.Connection);
            FbDataReader myreader = readCommand.ExecuteReader();
            if (myreader.Read())
                return Convert.ToInt32(myreader[0].ToString());
            else
                return -1;
        }

        internal static int GetExperimentIDForDisplay(ADatabase db, int experimentGroupID, KeyValuePair<string, byte> channel1)
        {
            string sql = "Select ExperimentId FROM experiments Where experiments.experimentgroupsid ="+ experimentGroupID +" AND experiments.syschannelid =  (select sysChannelID  From channels Where baseblockChannel="+channel1.Value+")";
            FbCommand readCommand = new FbCommand(sql, db.Connection);
            FbDataReader myreader = readCommand.ExecuteReader();
            myreader.Read();
            return Convert.ToInt32(myreader[0].ToString());
        }

        internal static ZedGraph.PointPairList GetData(ADatabase db, int experimentGroup, string key, int experimentChannel, int parameterID)
        {
            int comStart = key.IndexOf('(') + 1;
            int comEnd = key.IndexOf(')') - comStart;
            string comport = key.Substring(comStart, comEnd);
            string baseblockSerial = key.Substring(0, comStart - 1);

            ZedGraph.PointPairList result = new ZedGraph.PointPairList();
            string sql = "select parametersvalues.datetime, parametersvalues.parametervalue from parametersvalues where parametersvalues.parametersid="
                          + parameterID.ToString() +" and parametersvalues.experimentid=(select experiments.experimentid from experiments where experiments.experimentgroupsid="
                          + experimentGroup.ToString() +" and experiments.syschannelid=(select channels.syschannelid from channels where channels.baseblockchannel="
                          + experimentChannel.ToString() + "and channels.baseblockid=(select baseblocks.baseblockid from baseblocks where baseblocks.comport='" + comport + "' and baseblocks.baseblockserial=" + baseblockSerial + ")))" + " order by parametersvalues.datetime";              
            FbCommand readCommand = new FbCommand(sql, db.Connection);
            FbDataReader myreader = readCommand.ExecuteReader();
            while (myreader.Read())
            {
                result.Add(new ZedGraph.PointPair(  Convert.ToDateTime(myreader[0]).ToOADate(),Convert.ToDouble(myreader[1])));
            }
            return result;
        }

        internal static void WriteSensorStatistic(ADatabase db, Sensor sensor, ABaseblock device, int experimentGroup, int count)
        {
            int expID = GetExperimentsID(db, experimentGroup, device.sysChannelsIDs[count]);           
            WriteParameter(db, expID, sensor.SensorCalcLevel, ParametersID.LevelSensor);
            WriteParameter(db, expID, sensor.SensorTemperature, ParametersID.PlateTemperature);
            if (sensor.SensorDelays != null)
            {
                WriteParameter(db, expID, sensor.SensorDelays[0], ParametersID.ZondPositionSensor);
                WriteParameter(db, expID, sensor.SensorDelays[1], ParametersID.OtrPositionSensor);
                WriteParameter(db, expID, sensor.SensorDelays[2], ParametersID.SecondZondPositionSensor);
            }
            WriteParameter(db, expID, sensor.ComputerCalcLevel, ParametersID.LevelComputer);
            if (sensor.ComputerDelays != null)
            {
                WriteParameter(db, expID, sensor.ComputerDelays[0], ParametersID.ZondPositionComputer);
                WriteParameter(db, expID, sensor.ComputerDelays[1], ParametersID.OtrPositionComputer);
                WriteParameter(db, expID, sensor.ComputerDelays[2], ParametersID.SecondZondPositionComputer);
            }
            else
            {
            }
        }

        internal static void WriteSensorReflectogram(ADatabase db, Sensor sensor, ABaseblock device, int experimentGroup, int count)
        {
            int expID = GetExperimentsID(db, experimentGroup, device.sysChannelsIDs[count]);
            WriteReflectogram(db, sensor.Reflectogram, expID);
        }

        internal static List<string> GetAllReflectograms(ADatabase db, int experimentGroup, string key, byte experimentChannel)
        {
            int comStart = key.IndexOf('(') + 1;
            int comEnd = key.IndexOf(')') - comStart;
            string comport = key.Substring(comStart, comEnd);
            string baseblockSerial = key.Substring(0, comStart - 1);

            List<string> result = new List<string>();
            string sql = "select reflectograms.datetime from reflectograms where reflectograms.experimentid=(select experiments.experimentid from experiments where experiments.experimentgroupsid="
                + experimentGroup.ToString() + " and experiments.syschannelid=(select channels.syschannelid from channels where channels.baseblockchannel="
                + experimentChannel.ToString() + " and channels.baseblockid=(select baseblocks.baseblockid from baseblocks where baseblocks.comport='" + comport + "' and baseblocks.baseblockserial=" + baseblockSerial + ")))" + " order by reflectograms.datetime";                 
            FbCommand readCommand = new FbCommand(sql, db.Connection);
            FbDataReader myreader = readCommand.ExecuteReader();
            while (myreader.Read())
            {
                DateTime date = Convert.ToDateTime(myreader[0]);
                result.Add(date.ToString("dd.MM.yy HH:mm:ss.ff"));
            }
            return result;
        }

        internal static ZedGraph.PointPairList GetReflectogramData(ADatabase db, int experimentGroup, string key, byte experimentChannel, string time)
        {
            int comStart = key.IndexOf('(') + 1;
            int comEnd = key.IndexOf(')') - comStart;
            string comport = key.Substring(comStart, comEnd);           
            string baseblockSerial = key.Substring(0, comStart - 1);

            string sql = "select reflectograms.refldata from reflectograms where abs(datediff( millisecond, reflectograms.datetime, timestamp '" + time + "')) < 1000"  
                        + " and reflectograms.experimentid=(select experiments.experimentid from experiments where experiments.experimentgroupsid="
                        + experimentGroup.ToString() + " and experiments.syschannelid=(select channels.syschannelid from channels where channels.baseblockchannel="
                        + experimentChannel.ToString() + "and channels.baseblockid=(select baseblocks.baseblockid from baseblocks where baseblocks.comport='" + comport + "' and baseblocks.baseblockserial=" + baseblockSerial + ")))";      
            FbCommand readCommand = new FbCommand(sql, db.Connection);
            FbDataReader myreader = readCommand.ExecuteReader();
            if (myreader.Read())
            {
                string refldata = myreader[0].ToString();

                return ParseReflectogramData(refldata);
            }
            else
                return null;
        }

        private static ZedGraph.PointPairList ParseReflectogramData(string refldata)
        {
            ZedGraph.PointPairList result = new ZedGraph.PointPairList();  
            string[] data = refldata.Split(' ');
            int count = 0;
            foreach (string point in data)
            {
                if(point != "")
                    result.Add(count, ConvertToDoubleWithCheck(point, -1));
                count++;
            }
            return result;
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
                return errorValue;
            }
        }

        internal static double GetTemperature(ADatabase db, int experimentGroup, string key, byte experimentChannel, string time)
        {
            if (key != null)
            {
                int comStart = key.IndexOf('(') + 1;
                int comEnd = key.IndexOf(')') - comStart;
                string comport = key.Substring(comStart, comEnd);
                string baseblockSerial = key.Substring(0, comStart - 1);

                string sql = "select parametersvalues.parametervalue from  parametersvalues where abs(datediff( millisecond,  parametersvalues.datetime, timestamp '" + time + "')) < 1000 "
                            + "and parametersvalues.parametersid=" + ((int)ParametersID.PlateTemperature).ToString() + " and  parametersvalues.experimentid=(select experiments.experimentid from experiments where experiments.experimentgroupsid="
                            + experimentGroup.ToString() + " and experiments.syschannelid=(select channels.syschannelid from channels where channels.baseblockchannel="
                            + experimentChannel.ToString() + "and channels.baseblockid=(select baseblocks.baseblockid from baseblocks where baseblocks.comport='" + comport + "' and baseblocks.baseblockserial=" + baseblockSerial + ")))";
                FbCommand readCommand = new FbCommand(sql, db.Connection);
                FbDataReader myreader = readCommand.ExecuteReader();
                if (myreader.Read())
                {
                    return ConvertToDoubleWithCheck(myreader[0].ToString(), -1);
                }
                else
                    return -1;
            }
            else
                return -1;
        }

        internal static void DeleteExperimentData(ADatabase db, List<string> list)
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

        private static void DeleteExperimentGroup(ADatabase db, int experiment)
        {
            string sql = "DELETE  FROM ExperimentGroups WHERE ExperimentGroups.ExperimentGroupsID=" + experiment.ToString();
            FbCommand command = new FbCommand(sql, db.Connection);
            command.ExecuteNonQuery();
        }

        private static List<int> GetExperimentIDs(ADatabase db, int experimentGroup)
        {
            List<int> result = new List<int>();

            string sql = "SELECT ExperimentID FROM Experiments  Where ExperimentGroupsID=" + experimentGroup;
            FbCommand readCommand = new FbCommand(sql, db.Connection);
            FbDataReader myreader = readCommand.ExecuteReader();

            while (myreader.Read())
            {                
                result.Add(Convert.ToInt32(myreader[0].ToString()));
            }

            return result;
        }

        private static void DeleteExperimentReflectogramsAndData(ADatabase db, int experiment)
        {
            string sql = "DELETE  FROM Reflectograms WHERE Reflectograms.ExperimentID=" + experiment.ToString();
            FbCommand command = new FbCommand(sql, db.Connection);
            command.ExecuteNonQuery();

            sql = "DELETE  FROM ParametersValues WHERE ParametersValues.ExperimentID=" + experiment.ToString();
            command = new FbCommand(sql, db.Connection);
            command.ExecuteNonQuery();

            sql = "DELETE  FROM Experiments WHERE Experiments.ExperimentID="+ experiment.ToString();
            command = new FbCommand(sql, db.Connection);
            command.ExecuteNonQuery();

            
        }

        internal static List<string> GetBaseblockAndChannelsInfo(ADatabase db, int experimentGroup)
        {
            List<string> result = new List<string>();

            string sql = "select baseblocks.baseblockserial, baseblocks.comport,  channels.baseblockchannel From baseblocks, channels, experiments where experiments.experimentgroupsid ="
            + experimentGroup + " and channels.syschannelid=experiments.syschannelid and baseblocks.baseblockid=channels.baseblockid";
            FbCommand readCommand = new FbCommand(sql, db.Connection);
            FbDataReader myreader = readCommand.ExecuteReader();
            while (myreader.Read())
            {
                if (myreader.FieldCount == 3)
                    result.Add(myreader[0] + " (" + myreader[1] + ") - Канал " + myreader[2]);
            }
            return result;
        }

        internal static List<string> GetExperimentGroupInfo(ADatabase db, int experimentGroup)
        {
            List<string> result = new List<string>();

            string sql = "SELECT  experimentgroups.experimentgroupsid, experimentgroups.datestart, abs(datediff(minute, experimentgroups.datestop, experimentgroups.datestart)), experimentgroups.powerdelay, experimentgroups.commdelay, experimentgroups.description, (select Count(*) from experiments where experimentgroups.experimentgroupsid = experiments.experimentgroupsid) as cn FROM ExperimentGroups where experimentgroups.experimentgroupsid = "
            + experimentGroup.ToString();             
            FbCommand readCommand = new FbCommand(sql, db.Connection);
            FbDataReader myreader = readCommand.ExecuteReader();
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
            return result;
        }

        internal static bool WriteGroupExperimentEndDate(ADatabase db, int experimentGroup)
        {
            try
            {
                string sql = "UPDATE experimentgroups SET datestop='" + DateTime.Now.ToString("dd.MM.yy HH:mm:ss.ff") +"' where  experimentgroups.experimentgroupsid=" + experimentGroup.ToString();
                FbCommand command = new FbCommand(sql, db.Connection);
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
