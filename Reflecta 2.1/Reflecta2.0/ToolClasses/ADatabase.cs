using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using System.IO;
using System.Net;


namespace Reflecta2._0
{
    public enum ParametersID
    {
        PlateTemperature = 1,
        ZondPositionSensor, 
        OtrPositionSensor,
        SecondZondPositionSensor,
        LevelSensor,
        ZondPositionComputer,
        OtrPositionComputer,
        SecondZondPositionComputer,
        LevelComputer,
        FirstZondAmplitude,
        OtrAmplitude,
        SecondZondAmplitude,       
    }

    class ADatabase
    {
        string connectionInfo = "";
        NpgsqlConnection con;
        static string[] parameters = { "температура платы, °С", "позиция ЗИ (датч.), ед. ПНВ", "позиция ОИ (датч.), ед. ПНВ", "позиция ЗИ №2 (датч.), ед. ПНВ", 
                                "уровень (датч.), мм", "позиция ЗИ (комп.), ед. ПНВ", "позиция ОИ (комп.), ед. ПНВ", "позиция ЗИ №2 (комп.), ед. ПНВ", 
                                "уровень (комп.), мм", "амплитуда ЗИ, дискр. напр.", "амплитуда ОИ, дискр. напр.", "амплитуда ЗИ №2, дискр. напр." };

        public static string[] Parameters
        {
            get { return parameters; }
        }

        public ADatabase(string databaseName, string ip = "127.0.0.1")
        {
            connectionInfo = "Server=" + ip + ";Port=5432;User Id=postgres;Password=2410;Database=" + databaseName + ";";
        }

        public bool Connect()
        {
            try
            {
                con = new NpgsqlConnection(connectionInfo);
                con.Open();
                if (con.State == System.Data.ConnectionState.Open)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ADatabase", "Connect", ex.Message);
                return false;
            }
        }        

        public void CreateDBStructure()
        {
            try
            {
                string sql = "";
                NpgsqlCommand command = new NpgsqlCommand();
                CreateTables(ref sql, ref command);
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ADatabase", "CreateDBStructure", ex.Message);                
            }
        }

        

        private void CreateTables(ref string sql, ref NpgsqlCommand command)
        {
            try
            {
                NpgsqlTransaction t = con.BeginTransaction();

                sql = "CREATE TABLE Baseblocks (BaseblockID BIGSERIAL PRIMARY KEY NOT NULL, Comport VARCHAR(10) NOT NULL, BaseblockSerial VARCHAR(20) NOT NULL UNIQUE)";
                command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                sql = "CREATE TABLE Channels (SysChannelID BIGSERIAL PRIMARY KEY NOT NULL, BaseblockID INT8 NOT NULL, BaseblockChannel SMALLINT  NOT NULL,  FOREIGN KEY (BaseblockID) REFERENCES Baseblocks(BaseblockID))";
                command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                sql = "ALTER TABLE Channels ADD CONSTRAINT UQ_BaseblockID_BaseblockChannel UNIQUE (BaseblockID,BaseblockChannel)";
                command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                sql = "CREATE TABLE ExperimentGroups (ExperimentGroupsID BIGSERIAL PRIMARY KEY NOT NULL, DateStart TIMESTAMP NOT NULL, DateStop TIMESTAMP, PowerDelay INT NOT NULL, CommDelay INT NOT NULL, Description VARCHAR(255))";
                command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                sql = "CREATE TABLE Experiments (ExperimentID BIGSERIAL PRIMARY KEY NOT NULL, SysChannelID INT8 NOT NULL, ExperimentGroupsID INT8 NOT NULL, SensorSerial VARCHAR(20) NOT NULL, SensorFirmware FLOAT4 NOT NULL, CalibrationValue FLOAT8, FOREIGN KEY (SysChannelID) REFERENCES Channels(SysChannelID),  FOREIGN KEY (ExperimentGroupsID) REFERENCES ExperimentGroups(ExperimentGroupsID))";
                command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                sql = "CREATE TABLE Parameters (ParametersID BIGSERIAL PRIMARY KEY NOT NULL, ParamName VARCHAR(50) NOT NULL, Description VARCHAR(255))";
                command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                sql = "CREATE TABLE ParametersValues (ParametersValuesID BIGSERIAL PRIMARY KEY NOT NULL, ParametersID INT8 NOT NULL, ExperimentID INT8 NOT NULL, Datetime TIMESTAMP NOT NULL, ParameterValue FLOAT8 NOT NULL, FOREIGN KEY (ParametersID) REFERENCES Parameters(ParametersID), FOREIGN KEY (ExperimentID) REFERENCES Experiments(ExperimentID))";
                command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                sql = "CREATE TABLE Reflectograms (ReflID BIGSERIAL PRIMARY KEY NOT NULL, ExperimentID INT8 NOT NULL, Datetime TIMESTAMP NOT NULL, ReflData FLOAT[4096],  FOREIGN KEY (ExperimentID) REFERENCES Experiments(ExperimentID))";
                command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                sql = "CREATE TABLE temperaturehangers (temperaturehangerid bigserial  PRIMARY KEY NOT NULL,  experimentid bigint NOT NULL,  datetime timestamp NOT NULL, temperaturedata double precision[] NOT NULL,  FOREIGN KEY (ExperimentID) REFERENCES Experiments (ExperimentID))";
                command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                sql = "CREATE INDEX fki_temperature_fk ON temperaturehangers USING btree (experimentid);";
                command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                sql = "CREATE INDEX parametersvalues_datetime_index ON parametersvalues  USING btree (datetime);";
                command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                sql = "CREATE INDEX parametersvalues_parameterid_index ON parametersvalues USING btree(parametersid);";
                command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                t.Commit();
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ADatabase", "CreateTables", ex.Message);
            }
        }

        public void WriteParameters()
        {
            try
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    string sql = "INSERT INTO PARAMETERS (ParametersID, ParamName) VALUES (" + (i + 1).ToString() + ",'" + parameters[i] + "')";
                    NpgsqlCommand command = new NpgsqlCommand(sql, con);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ADatabase", "WriteParameters", ex.Message);
            }
        }

        public NpgsqlConnection Connection { get { return con; } }

        internal static int GetParameterID(string parameterName)
        {
            try
            {
                for (int i = 0; i < parameters.Length; i++)
                    if (parameters[i] == parameterName)
                        return i + 1;
                return -1;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ADatabase", "GetParameterID", ex.Message);
                return -1;
            }
        }

        internal void CreateCalibrationDBStructure()
        {
            try
            {
                NpgsqlTransaction t = con.BeginTransaction();

                string sql = "CREATE TABLE CalibrationGroup (CalibrationGroupID BIGSERIAL PRIMARY KEY NOT NULL, NumOfAverage INT2 NOT NULL, Datetime TIMESTAMP NOT NULL, SensorFirmware FLOAT NOT NULL, SensorSerial VARCHAR(20) NOT NULL )";
                NpgsqlCommand command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                sql = "CREATE TABLE Calibration (CalibrationID BIGSERIAL PRIMARY KEY NOT NULL, RealValue FLOAT NOT NULL, MeasureValue FLOAT NOT NULL, CalibrationGroup INT8 NOT NULL,  FOREIGN KEY (CalibrationGroup) REFERENCES CalibrationGroup(CalibrationGroupID))";
                command = new NpgsqlCommand(sql, con);
                command.ExecuteNonQuery();

                t.Commit();
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ADatabase", "CreateCalibrationDBStructure", ex.Message);
            }
        }

        internal static bool MainDatabaseStructureIsOK(NpgsqlConnection con)
        {
            try
            {
                string sql = "select count(*) as table_count from information_schema.tables where table_schema = 'public';";
                NpgsqlCommand command = new NpgsqlCommand(sql, con);
                NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int numOfTables = Convert.ToInt32(reader["table_count"].ToString());
                    if (numOfTables == 8)
                    {
                        reader.Close();
                        return true;
                    }
                    else
                    {
                        reader.Close();
                        return false;
                    }
                }
                else
                {
                    reader.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ADatabase", "MainDatabaseStructureIsOK", ex.Message);
                return false;
            }            
        }

        internal static bool CalibrationDatabaseStructureIsOK(NpgsqlConnection con)
        {
            try
            {

                string sql = "select count(*) as table_count from information_schema.tables where table_schema = 'public';";
                NpgsqlCommand command = new NpgsqlCommand(sql, con);
                NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int numOfTables = Convert.ToInt32(reader["table_count"].ToString());
                    if (numOfTables == 2)
                    {
                        reader.Close();
                        return true;
                    }
                    else
                    {
                        reader.Close();
                        return false;
                    }
                }
                else
                {
                    reader.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ADatabase", "CalibrationDatabaseStructureIsOK", ex.Message);
                return false;
            }
            
        }
    }
}
