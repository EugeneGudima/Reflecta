using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Management;

namespace Reflecta2._0
{
    class ASearch
    {
        private const byte maxChannelsInBaseblock = 16;

        public static List<string> SearchAmicoComports()
        {
            try
            {
                List<string> result = new List<string>();
                using (var searcher = new ManagementObjectSearcher
                   ("SELECT * FROM WIN32_SerialPort"))
                {
                    string[] portnames = SerialPort.GetPortNames();
                    var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();
                    var tList = (from n in portnames
                                 join p in ports on n equals p["DeviceID"].ToString()
                                 where p["Caption"].ToString().Contains("CP210x")
                                 select n);
                    result = tList.ToList();

                    return result;
                }
            }
            catch(Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASearch", "SearchAmicoComports", ex.Message);
                return null;
            }

        }
        public static List<string> CheckComportsForThermoPlate(List<string> comportsWithCp210x)
        {
            try
            {
                List<string> result = new List<string>();
                foreach (string comport in comportsWithCp210x)
                {
                    if (ThermoPlate.CheckThermoPlate(comport))
                    {                                              
                        result.Add(comport);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASearch", "CheckComportsForBaseblock", ex.Message);
                return new List<string>();
            }
        }

        public static List<string> CheckComportsForBaseblock(List<string> comportsWithCp210x, int powerDelay)
        {
            try
            {
                List<string> result = new List<string>();
                foreach (string comport in comportsWithCp210x)
                {
                    string error = "";
                    ADriver d = new ADriver(comport, powerDelay);
                    if (d.OpenPort(ref error))
                    {
                        if (d.CheckBaseblock(ref error))
                            result.Add(comport);
                        //else
                        //{
                        //    if(d.CheckSensor(ref error))
                        //        result.Add(comport);
                        //}

                    }
                    else
                    {
                        error = "Не удалось открыть порт: " + comport;
                        FileWorker.WriteEventFile(DateTime.Now, "ASearch", "CheckComportsForBaseblock", error);
                    }
                    if (!d.ClosePort())
                    {
                        error = "Не удалось закрыть порт:" + comport;
                        FileWorker.WriteEventFile(DateTime.Now, "ASearch", "CheckComportsForBaseblock", error);
                    }
                }
                return result;
            }
            catch(Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASearch", "CheckComportsForBaseblock", ex.Message);
                return new List<string>();
            }
        }       


        internal static Dictionary<string, Dictionary<string, byte>> SearchSensors(List<string> comportsWithBaseblocks, int powerDelay)
        {
            try
            {
                Dictionary<string, Dictionary<string, byte>> result = new Dictionary<string, Dictionary<string, byte>>();
                Random r = new Random();
                foreach (string comport in comportsWithBaseblocks)
                {
                    string error = "";
                    string serial = "";
                    Dictionary<string, byte> channels = new Dictionary<string, byte>();
                    ADriver d = new ADriver(comport, powerDelay);
                    if (d.OpenPort(ref error))
                    {
                        //if (d.CheckSensor(ref error))
                        //{
                        //    result.Add(r.Next(0, 65532).ToString() + " (" + comport + ')', null);
                        //}
                        //else
                        //{

                            for (byte i = 0; i < maxChannelsInBaseblock; i++)
                            {
                                if (d.CheckSensor(i, ref error))
                                {
                                    serial = d.GetSerial(ref error);
                                    //serial = "Sensor ";
                                    channels.Add(serial + i.ToString(), i);
                                }
                                d.ResetChannel(ref error);
                            }
                            d.ResetChannel(ref error);
                            if (channels.Count > 0)
                            {
                                result.Add(r.Next(0, 65532).ToString() + " (" + comport + ')', channels);
                            }


                            if (!d.ClosePort())
                            {
                                error = "Не удалось закрыть порт: " + comport;
                                FileWorker.WriteEventFile(DateTime.Now, "ASearch", "SearchSensors", error);
                            }
                        //}
                    }
                    else
                    {
                        error = "Не удалось открыть порт: " + comport;
                        FileWorker.WriteEventFile(DateTime.Now, "ASearch", "SearchSensors", error);
                    }
                }
                return result;
            }
            catch(Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASearch", "SearchSensors", ex.Message);
                return null;
            }

        }
    }
}
