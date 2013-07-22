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
            catch
            {
                return null;
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
                    }
                    else
                    {
                    }
                    if (!d.ClosePort())
                    {
                        //error
                    }
                }
                return result;
            }
            catch
            {
                return new List<string>();
            }
        }

        //static List<object> SensorSearch( ADriver.ADriver drv)
        //{
        //    try
        //    {
        //        List<object> buffer = new List<object>();
        //        buffer.Add(drv.PortName);
        //        byte[] dataReceived = new byte[1];
        //        drv.Read(ref dataReceived);
        //        if (dataReceived[0] == 213)
        //        {
        //            for (byte i = 0; i < 16; i++)
        //            {
        //                double? version = drv.GetVersion(i);
        //                if(version != null)
        //                    buffer.Add(i);
        //            }
        //        }
        //        else
        //            buffer.Add(-1);
        //        return buffer;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}   


        internal static Dictionary<string, List<byte>> SearchSensors(List<string> comportsWithBaseblocks, int powerDelay)
        {
            try
            {
                Dictionary<string, List<byte>> result = new Dictionary<string,List<byte>>();
                Random r = new Random();
                foreach (string comport in comportsWithBaseblocks)
                {
                    string error = "";
                    List<byte> channels = new List<byte>();
                    ADriver d = new ADriver(comport, powerDelay);
                    if (d.OpenPort(ref error))
                    {

                        for (byte i = 0; i < maxChannelsInBaseblock; i++)
                        {
                            if (d.CheckSensor(i, ref error))
                            {
                                channels.Add(i);
                            }
                            d.ResetChannel(ref error);
                        }
                        d.ResetChannel(ref error);
                        if (channels.Count > 0)
                        {                            
                            result.Add(r.Next(0,65532).ToString() + " (" + comport + ')', channels);
                        }


                        if (!d.ClosePort())
                        {
                            //error
                        }
                    }
                    else
                    {

                    }
                }
                return result;
            }
            catch
            {
                return null;
            }

        }
    }
}
