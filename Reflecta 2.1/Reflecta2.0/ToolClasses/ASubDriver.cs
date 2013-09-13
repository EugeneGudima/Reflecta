using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Reflecta2._0
{
    class ASubDriver
    {
        public static bool ClearBuffer(SerialPort port)
        {
            try
            {
                port.DiscardOutBuffer();
                port.DiscardInBuffer();
                return true;
            }
            catch(Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASubDriver", "ClearBuffer", ex.Message);
                return false;
            }
        }
        public static bool WriteChannel(SerialPort port, byte channel)
        {
            try
            {
                byte[] snddata = new byte[4];
                snddata[0] = 255;
                snddata[1] = channel;
                byte[] CRC = GetCRC(snddata);
                snddata[2] = CRC[0];
                snddata[3] = CRC[1];
                if (Write(port, snddata))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASubDriver", "WriteChannel", ex.Message);
                return false;
            }
        }

        public static bool WriteCommand(SerialPort port, ACommands command)
        {
            try
            {
                byte[] snddata = new byte[4];
                snddata[0] = (byte)command;
                snddata[1] = 0;
                byte[] CRC = GetCRC(snddata);
                snddata[2] = CRC[0];
                snddata[3] = CRC[1];
                if (Write(port, snddata))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASubDriver", "WriteCommand", ex.Message);
                return false;
            }
        }

        public static bool WriteCommand(SerialPort port, ACommands command, Average average)
        {
            try
            {
                byte[] snddata = new byte[4];
                snddata[0] = (byte)command;
                snddata[1] = (byte)average;
                byte[] CRC = GetCRC(snddata);
                snddata[2] = CRC[0];
                snddata[3] = CRC[1];
                if (Write(port, snddata))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASubDriver", "WriteCommand", ex.Message);
                return false;
            }
        }

        private static bool Write(SerialPort port, byte[] dataWrite)
        {
            try
            {
                port.Write(dataWrite, 0, 4);
                ADriver.messageReceived.Reset();
                ADriver.reflReceived.Reset();
                return true;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASubDriver", "Write", ex.Message);
                return false;
            }
        }

        private static bool Read(SerialPort port, ref byte[] dataRead)
        {
            try
            {
                if (port.BytesToRead > 0)
                {
                    dataRead = new byte[port.BytesToRead];
                    port.Read(dataRead, 0, port.BytesToRead);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASubDriver", "Read", ex.Message);
                return false;
            }
        }

        private static bool ReadReflectogram(SerialPort port, ref byte[] dataRead)
        {
            try
            {
                if (port.BytesToRead == 8192)
                {
                    dataRead = new byte[8192];
                    port.Read(dataRead, 0, 8192);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASubDriver", "ReadReflectogram", ex.Message);
                return false;
            }
        }

        public static byte[] ReadReflectogramWithTimeout(SerialPort port, int timeout = 3000)
        {
            try
            {
                byte[] data = new byte[1];
                if (ADriver.reflReceived.WaitOne(timeout))
                {
                    if (ReadReflectogram(port, ref data))
                    {
                        return data;
                    }
                    else
                        return null;
                }
                else
                {
                    if (ReadReflectogram(port, ref data))
                    {
                        return data;
                    }
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASubDriver", "ReadReflectogramWithTimeout", ex.Message);
                return null;
            }
        }

        public static byte[] ReadWithTimeout(SerialPort port, int timeout = 2000)
        {
            try
            {
                byte[] data = new byte[1];
                if (ADriver.messageReceived.WaitOne(timeout))
                {
                    if (Read(port, ref data))
                    {
                        return data;
                    }
                    else
                        return null;
                }
                else
                {
                    if (Read(port, ref data))
                    {
                        return data;
                    }
                    else
                        return null;
                }                
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASubDriver", "ReadWithTimeout", ex.Message);
                return null;
            }
        }

        public static byte[] GetCRC(byte[] message)
        {
            try
            {
                //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
                //return the CRC values:
                byte[] CRC = new byte[2];
                ushort CRCFull = 0xFFFF;
                byte CRCHigh = 0xFF, CRCLow = 0xFF;
                char CRCLSB;

                for (int i = 0; i < (message.Length) - 2; i++)
                {
                    CRCFull = (ushort)(CRCFull ^ message[i]);

                    for (int j = 0; j < 8; j++)
                    {
                        CRCLSB = (char)(CRCFull & 0x0001);
                        CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                        if (CRCLSB == 1)
                            CRCFull = (ushort)(CRCFull ^ 0xA001);
                    }
                }
                CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
                CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
                return CRC;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASubDriver", "GetCRC", ex.Message);
                return null;
            }
        }

        public static bool CheckCRC(byte[] buffer)
        {
            try
            {
                if (buffer != null)
                {
                    if (buffer.Length >= 4)
                    {
                        byte[] CRC = GetCRC(buffer);
                        if (CRC[0] == buffer[buffer.Length - 2] && CRC[1] == buffer[buffer.Length - 1])
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ASubDriver", "CheckCRC", ex.Message);
                return false;
            }
        }
    }
}
