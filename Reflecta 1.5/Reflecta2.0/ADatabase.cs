using System;
using System.Collections.Generic;
using System.Text;
using FirebirdSql.Data.FirebirdClient;
using System.IO;


namespace Reflecta2._0
{
    enum ParametersID
    {
        PlateTemperature = 1,
        ZondPositionSensor, 
        OtrPositionSensor,
        SecondZondPositionSensor,
        LevelSensor,
        ZondPositionComputer,
        OtrPositionComputer,
        SecondZondPositionComputer,
        LevelComputer

    }
    class ADatabase
    {
        FbConnectionStringBuilder csb = new FbConnectionStringBuilder();
        FbConnection con;
        static string[] parameters = { "температура платы, °С", "позиция ЗИ (датч.), ед. ПНВ", "позиция ОИ (датч.), ед. ПНВ", "позиция ЗИ №2 (датч.), ед. ПНВ", 
                                "уровень (датч.), мм", "позиция ЗИ (комп.), ед. ПНВ", "позиция ОИ (комп.), ед. ПНВ", "позиция ЗИ №2 (комп.), ед. ПНВ", 
                                "уровень (комп.), мм" };

        public ADatabase()
        {
            csb.ServerType = FbServerType.Embedded;
            csb.UserID = "SYSDBA";
            csb.Password = "masterkey";
            csb.Dialect = 3;
            System.IO.Directory.CreateDirectory(@"data");
            csb.Database = @"data\\database.fdb";
            csb.Charset = FbCharset.Utf8.ToString();
        }

        public bool Connect()
        {
            con = new FbConnection(csb.ToString());
            con.Open();
            if (con.State == System.Data.ConnectionState.Open)
                return true;
            else
                return false;
        }

        public void CreateDB()
        {
            FbConnection.CreateDatabase(csb.ToString()); 
        }

        public void CreateDBStructure()
        {
            string sql;
            FbCommand command;
            CreateTables(out sql, out command);
            CreateTriggers(ref sql, ref command);
        }

        private void CreateTriggers(ref string sql, ref FbCommand command)
        {
            sql = "CREATE GENERATOR generator_parametersID";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE TRIGGER parametersID_trigger FOR Parameters ACTIVE BEFORE INSERT POSITION 0 AS BEGIN if (new.ParametersID is null ) then new.ParametersID = gen_id (generator_parametersID, 1); END";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE GENERATOR generator_experimentID";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE TRIGGER experimentID_trigger FOR Experiments ACTIVE BEFORE INSERT POSITION 0 AS BEGIN if (new.ExperimentID is null ) then new.ExperimentID = gen_id (generator_experimentID, 1); END";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE GENERATOR generator_experimentsGroupID";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE TRIGGER experimentsGroupID_trigger FOR ExperimentGroups ACTIVE BEFORE INSERT POSITION 0 AS BEGIN if (new.ExperimentGroupsID is null ) then new.ExperimentGroupsID = gen_id (generator_experimentsGroupID, 1); END";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE GENERATOR generator_reflectogramsID";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE TRIGGER reflectogramsID_trigger FOR Reflectograms ACTIVE BEFORE INSERT POSITION 0 AS BEGIN if (new.ReflID is null ) then new.ReflID = gen_id (generator_reflectogramsID, 1); END";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE GENERATOR generator_parametersValuesID";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE TRIGGER parametersValuesID_trigger FOR ParametersValues ACTIVE BEFORE INSERT POSITION 0 AS BEGIN if (new.ParametersValuesID is null ) then new.ParametersValuesID = gen_id (generator_parametersValuesID, 1); END";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE GENERATOR generator_channelsID";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE TRIGGER channelsID_trigger FOR Channels ACTIVE BEFORE INSERT POSITION 0 AS BEGIN if (new.SysChannelID is null ) then new.SysChannelID = gen_id (generator_channelsID, 1); END";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE GENERATOR generator_basebloksID";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE TRIGGER basebloksID_trigger FOR Baseblocks ACTIVE BEFORE INSERT POSITION 0 AS BEGIN if (new.BaseblockID is null ) then new.BaseblockID = gen_id (generator_basebloksID, 1); END";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();
        }

        private void CreateTables(out string sql, out FbCommand command)
        {
            sql = "CREATE TABLE Baseblocks (BaseblockID INTEGER PRIMARY KEY NOT NULL, Comport VARCHAR(10) NOT NULL, BaseblockSerial VARCHAR(20) NOT NULL UNIQUE)";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE TABLE Channels (SysChannelID INTEGER PRIMARY KEY NOT NULL, BaseblockID INTEGER NOT NULL, BaseblockChannel SMALLINT  NOT NULL,  FOREIGN KEY (BaseblockID) REFERENCES Baseblocks(BaseblockID))";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "ALTER TABLE Channels ADD CONSTRAINT UQ_BaseblockID_BaseblockChannel UNIQUE (BaseblockID,BaseblockChannel)";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE TABLE ExperimentGroups (ExperimentGroupsID INTEGER PRIMARY KEY NOT NULL, DateStart TIMESTAMP NOT NULL, DateStop TIMESTAMP, PowerDelay INT NOT NULL, CommDelay INT NOT NULL, Description VARCHAR(255))";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE TABLE Experiments (ExperimentID INTEGER PRIMARY KEY NOT NULL, SysChannelID INTEGER NOT NULL, ExperimentGroupsID INTEGER NOT NULL, SensorSerial VARCHAR(20) NOT NULL, SensorFirmware DECIMAL(3,3) NOT NULL, FOREIGN KEY (SysChannelID) REFERENCES Channels(SysChannelID),  FOREIGN KEY (ExperimentGroupsID) REFERENCES ExperimentGroups(ExperimentGroupsID))";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();            

            sql = "CREATE TABLE Parameters (ParametersID INTEGER PRIMARY KEY NOT NULL, ParamName VARCHAR(50) NOT NULL, Description VARCHAR(255))";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE TABLE ParametersValues (ParametersValuesID INTEGER PRIMARY KEY NOT NULL, ParametersID INTEGER NOT NULL, ExperimentID INTEGER NOT NULL, Datetime TIMESTAMP NOT NULL, ParameterValue FLOAT NOT NULL, FOREIGN KEY (ParametersID) REFERENCES Parameters(ParametersID), FOREIGN KEY (ExperimentID) REFERENCES Experiments(ExperimentID))";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();

            sql = "CREATE TABLE Reflectograms (ReflID INTEGER PRIMARY KEY NOT NULL, ExperimentID INTEGER NOT NULL, Datetime TIMESTAMP NOT NULL, ReflData BLOB SUB_TYPE TEXT NOT NULL,  FOREIGN KEY (ExperimentID) REFERENCES Experiments(ExperimentID))";
            command = new FbCommand(sql, con);
            command.ExecuteNonQuery();
        }

        public void WriteParameters()
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                string sql = "UPDATE OR INSERT INTO PARAMETERS (ParametersID, ParamName) VALUES (" + (i + 1).ToString() + ",'" + parameters[i] + "')";
                FbCommand command = new FbCommand(sql, con);
                command.ExecuteNonQuery();
            }
        }

        public FbConnection Connection { get { return con; } }

        internal static int GetParameterID(string parameterName)
        {
            for (int i = 0; i < parameters.Length; i++)
                if (parameters[i] == parameterName)
                    return i+1;
            return -1;
        }
    }
}
