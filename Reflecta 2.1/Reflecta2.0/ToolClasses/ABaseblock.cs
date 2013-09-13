using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace Reflecta2._0
{
    enum ThermoPlateState
    {
        Frost,
        Heat
    }
    
    class ThermoPlate
    {
        string comport = "";
        string serial = "";
        SerialPort port;

        public ThermoPlate(string comport)
        {
            try
            {
                port = new SerialPort(comport, 9600);
                
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ThermoPlate", "ThermoPlate", ex.Message);
            }
        }

        public string ComPort
        {
            get { return comport; }          
        }

        public string Serial
        {
            get { return serial; }
            set
            {
                if (value != null)
                    serial = value;
            }
        }

        public bool SendCommand(ThermoPlateState state, byte power, byte gercon)
        {
            try
            {
                port.Open();
                if (power >= 0 && power <= 250 && gercon >= 0 && gercon <= 14)
                {
                    byte[] sendMessage = { (byte)state, power, gercon };
                    port.Write(sendMessage, 0, sendMessage.Length);
                    return true;
                }
                else
                    return false;

            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ThermoPlate", "SendCommand", ex.Message);
                return false;
            }
            finally
            {
                port.Close();
            }
        }

        public double GetTemperature()
        {
            try
            {
                port.Open();
                byte[] buffer = { 84, 0, 0 };
                port.Write(buffer, 0, 3);
                System.Threading.Thread.Sleep(1000);
                buffer = new byte[port.BytesToRead];
                port.Read(buffer, 0, buffer.Length);

                double? temp = 0;
                double tempH, tempL, tempFrac;
                if (buffer.Length == 2)
                {
                    if (buffer[0] <= 7)
                    {
                        tempH = ((byte)buffer[0] & 15) << 4;
                        tempL = ((byte)buffer[1] & 240) >> 4;
                        tempFrac = 0.0625 * ((byte)buffer[1] & 15);
                        temp = tempH + tempL + tempFrac;
                    }
                    else
                    {
                        tempH = ((byte)~buffer[0] & 15) << 4;
                        tempL = ((byte)~buffer[1] & 240) >> 4;
                        tempFrac = 0.0625 * ((byte)~buffer[1] & 15) + 0.0625;
                        temp = -(tempH + tempL + tempFrac);
                    }
                }
                return (double)Math.Round((double)temp, 1);

            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ThermoPlate", "SendCommand", ex.Message);
                return -273.0;
            }
            finally
            {
                port.Close();
            }
        }

        public string GetSerial()
        {
            try
            {
                port.Open();
                byte[] buffer = { 73, 0, 0 };
                port.Write(buffer, 0, 3);
                System.Threading.Thread.Sleep(80);
                buffer = new byte[port.BytesToRead];
                port.Read(buffer, 0, buffer.Length);
                string result = System.Text.Encoding.ASCII.GetString(buffer);
                if (result != null)
                    return result;
                else
                    return "";
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ThermoPlate", "SendCommand", ex.Message);
                return "";
            }
            finally
            {
                port.Close();
            }
        }

        public static bool CheckThermoPlate(string comPort)
        {
            try
            {
                SerialPort sport = new SerialPort(comPort, 9600);
                sport.Open();
                byte[] buffer = { 73, 0, 0 };
                sport.Write(buffer, 0, 3);
                System.Threading.Thread.Sleep(80);
                buffer = new byte[sport.BytesToRead];
                sport.Read(buffer, 0, buffer.Length);
               sport.Close();
                if (buffer.Length > 2)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ThermoPlate", "CheckThermoPlate", ex.Message);
                return false;
            }           
        }


    }

    class ABaseblock
    {        
        public string comport;
        public string baseblockSerial;
        public List<byte> channelsWithSensors;
        public int baseblockID;
        public List<int> sysChannelsIDs;

        public static List<ABaseblock> WriteBaseblockInfo(Dictionary<string, List<byte>> devicesForExperiment)
        {
            try
            {
                List<ABaseblock> result = new List<ABaseblock>();
                int count = 0;
                foreach (KeyValuePair<string, List<byte>> device in devicesForExperiment)
                {
                    result.Add(new ABaseblock());
                    int comStart = device.Key.IndexOf('(') + 1;
                    int comEnd = device.Key.IndexOf(')') - comStart;
                    result[count].comport = device.Key.Substring(comStart, comEnd);
                    result[count].channelsWithSensors = device.Value;
                    result[count].baseblockSerial = device.Key.Substring(0, comStart - 2);
                    count++;
                }

                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ABaseblock", "WriteBaseblockInfo", ex.Message);
                return null;
            }


        }
    }
}
