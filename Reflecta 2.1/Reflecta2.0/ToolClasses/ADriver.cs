﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reflecta2._0
{
    public enum Average
    {
        None = 42,
        TwoReflectogram = 43,
        FourReflectogram = 44,
        EightReflectogram = 45,
        SixteenReflectogram = 46
    }

    enum ACommands
    {
        Version = 30,
        Serial = 31,
        StartReflectogramConvertation = 40,
        SendReflectogram = 41,
        SendDelays = 47,
        SendLevel = 48,
        SendLevelWithCorrection = 49,
        StartTemperatureConvertation = 50,
        SendTemperature = 51
    }

    class ADriver
    {
        SerialPort _serialPort = new SerialPort();
        private int _powerDelay = 50;

        public static ManualResetEvent reflReceived =
           new ManualResetEvent(false);
        public static ManualResetEvent messageReceived =
            new ManualResetEvent(false);

        public ADriver(string portName, int powerDelay)
        {
            try
            {
                _serialPort.PortName = portName;
                _serialPort.BaudRate = 115200; //Скорость передачи 115200 бит/с               

                _serialPort.RtsEnable = true; // включаем сигнал запроса передачи (RTS) в сеансе последовательной связи
                _serialPort.DtrEnable = true; // включаем поддержку сигнала готовности терминала (DTR) в сеансе последовательной связи

                _serialPort.ReadTimeout = 1000; //срок ожидания в миллисекундах для завершения операции чтения
                _serialPort.WriteTimeout = 1000; //срок ожидания в миллисекундах для завершения операции записи
                _serialPort.ReadBufferSize = 16300; //размер буфера чтения
                _serialPort.DataReceived += _serialPort_DataReceived;

                _powerDelay = powerDelay;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "ADriver", ex.Message);
            }
        }

        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (_serialPort.BytesToRead >= 4)
                {
                    messageReceived.Set();
                    if (_serialPort.IsOpen && _serialPort.BytesToRead == 4096)
                        reflReceived.Set();
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "_serialPort_DataReceived", ex.Message);
            }
        }

        public bool OpenPort(ref string errorMessage)
        {
            try
            {
                _serialPort.Open();
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "OpenPort", ex.Message);
                return false;
            }
        }

        public string GetSerial(ref string errorMessage)
        {
            try
            {
                if (ASubDriver.ClearBuffer(_serialPort))
                {
                    ASubDriver.WriteCommand(_serialPort, ACommands.Serial);
                    //System.Threading.Thread.Sleep(10);                            
                    //подсчет того что пришло

                    byte[] buffer = ASubDriver.ReadWithTimeout(_serialPort);
                    if (ASubDriver.CheckCRC(buffer))
                    {
                        byte[] errorMessageWrongCRC = { 13, 13, 196, 229 };
                        if (buffer != errorMessageWrongCRC)
                        {
                            byte[] errorMessageWrongCommand = { 24, 24, 11, 186 };
                            if (buffer != errorMessageWrongCommand)
                            {
                                return buffer[0].ToString() + buffer[1].ToString() + buffer[2].ToString() + buffer[3].ToString();
                            }
                            else
                            {
                                errorMessage = _serialPort.PortName + " Датчик получил сообщение состоящее не из 4-х байт: ";
                                foreach (byte dataByte in buffer)
                                    errorMessage += " " + dataByte.ToString();
                                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetSerial", errorMessage);
                                return null;
                            }
                        }
                        else
                        {
                            errorMessage = _serialPort.PortName + " Датчик получил сообщение c неправильным CRC: ";
                            foreach (byte dataByte in buffer)
                                errorMessage += " " + dataByte.ToString();
                            FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetSerial", errorMessage);
                            return null;
                        }
                    }
                    else
                    {
                        errorMessage = _serialPort.PortName + " Ответ от датчика пришел с неправильным CRC: ";
                        foreach (byte dataByte in buffer)
                            errorMessage += " " + dataByte.ToString();
                        FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetSerial", errorMessage);
                        return null;
                    }
                }
                else
                {
                    errorMessage = _serialPort.PortName + " Не удалось очистить буфер приема/передачи ";
                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetSerial", errorMessage);
                    return null;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetSerial", ex.Message);
                return null;
            }
        }

        public double GetFirmware(ref string errorMessage)
        {
            try
            {
                if (ASubDriver.ClearBuffer(_serialPort))
                {
                    ASubDriver.WriteCommand(_serialPort, ACommands.Version);
                    //System.Threading.Thread.Sleep(10);                            
                    //подсчет того что пришло

                    byte[] buffer = ASubDriver.ReadWithTimeout(_serialPort, 250);
                    if (buffer != null)
                    {
                        if (ASubDriver.CheckCRC(buffer))
                        {
                            byte[] errorMessageWrongCRC = { 13, 13, 196, 229 };
                            if (buffer != errorMessageWrongCRC)
                            {
                                byte[] errorMessageWrongCommand = { 24, 24, 11, 186 };
                                if (buffer != errorMessageWrongCommand)
                                {
                                    return (buffer[0] + buffer[1] / 10.0);
                                }
                                else
                                {
                                    errorMessage = _serialPort.PortName + " Датчик получил сообщение состоящее не из 4-х байт: ";
                                    foreach (byte dataByte in buffer)
                                        errorMessage += " " + dataByte.ToString();
                                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetFirmware", errorMessage);
                                    return -1.0;
                                }
                            }
                            else
                            {
                                errorMessage = _serialPort.PortName + " Датчик получил сообщение c неправильным CRC: ";
                                foreach (byte dataByte in buffer)
                                    errorMessage += " " + dataByte.ToString();
                                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetFirmware", errorMessage);
                                return -1.0;
                            }
                        }
                        else
                        {
                            errorMessage = _serialPort.PortName + " Ответ от датчика пришел с неправильным CRC: ";
                            foreach (byte dataByte in buffer)
                                errorMessage += " " + dataByte.ToString();
                            FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetFirmware", errorMessage);
                            return -1.0;
                        }
                    }
                    else
                    {
                        //errorMessage = _serialPort.PortName + " Нет данных для чтения. (Отсутствие датчика на линии)";                        
                        //FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetFirmware", errorMessage);
                        return -1.0;
                    }
                }
                else
                {
                    errorMessage = _serialPort.PortName + " Не удалось очистить буфер приема/передачи ";
                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetFirmware", errorMessage);
                    return -1.0;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetFirmware", errorMessage);
                return -1.0;
            }
        }

        public double[] GetDelays(ref string errorMessage)
        {
            try
            {
                double[] result = new double[3];

                if (ASubDriver.ClearBuffer(_serialPort))
                {

                    ASubDriver.WriteCommand(_serialPort, ACommands.SendDelays);
                    //System.Threading.Thread.Sleep(10);                            
                    //подсчет того что пришло

                    byte[] buffer = ASubDriver.ReadWithTimeout(_serialPort);

                    result[0] = Convert.ToSingle(buffer[0] + buffer[1] * 256 + buffer[2] * 65536 + buffer[3] * 16777216) / 1000.0;
                    result[1] = Convert.ToSingle(buffer[4] + buffer[5] * 256 + buffer[6] * 65536 + buffer[7] * 16777216) / 1000.0;
                    result[2] = Convert.ToSingle(buffer[8] + buffer[9] * 256 + buffer[10] * 65536 + buffer[11] * 16777216) / 1000.0;
                    return result;


                }
                else
                {
                    errorMessage = _serialPort.PortName + "Не удалось очистить буфер приема/передачи ";
                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetDelays", errorMessage);
                    return null;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetDelays", errorMessage);
                return null;
            }
        }

        public double[] GetReflectogram(ref string errorMessage)
        {
            try
            {
                if (ASubDriver.ClearBuffer(_serialPort))
                {

                    //System.Threading.Thread.Sleep(800); 
                    ASubDriver.WriteCommand(_serialPort, ACommands.SendReflectogram);
                    //ASubDriver.WriteCommand(_serialPort, ACommands.SendReflectogram);
                    System.Threading.Thread.Sleep(1000);
                    //подсчет того что пришло

                    byte[] buffer = ASubDriver.ReadReflectogramWithTimeout(_serialPort, 2000);

                    double[] refl = ByteToDoubleArray(buffer);
                    double[] reflectogramData = new double[4096];


                    //инвертируем рефлектограмму
                    for (int i = 0; i < refl.Length; i++)
                        refl[i] *= -1;

                    //double[] testMass = new double[4096];
                    ////делаем сглаживание
                    //int m = 2;
                    //for (int i = 0; i < refl.Length; i++)
                    //{
                    //    double sum = 0;
                    //    for (int j = i - m; j < i + m; j++)
                    //    {
                    //        if (j < 0)
                    //            sum += refl[0];
                    //        else if (j >= refl.Length)
                    //            sum += refl[refl.Length - 1];
                    //        else
                    //            sum += refl[j];
                    //    }
                    //    double test = (double)(2.0 * m + 1.0);
                    //    testMass[i] = sum / test;
                    //}

                    //for (int i = 0; i < testMass.Length; i++)
                    //    refl[i] = testMass[i];

                    ////первые значения пропуск и осреднение.., вычитаем средн.
                    //double mnim_nol = 0;
                    //int nachalo = 50;
                    //int konec = 130;
                    //int tochek = konec - nachalo;
                    //double treb_ampl = 1000;//нормируем к этому значению
                    //for (int i = nachalo; i < konec; i++)
                    //    mnim_nol = mnim_nol + refl[i];

                    //mnim_nol = mnim_nol / tochek;//ноль, который вычитаем из каждого значения

                    ////поиск макс.
                    //int max_point = FindMax(refl, 0, refl.Length - 1);
                    //double old_ampl = refl[max_point]; //старая амплитуда зонда
                    //double coeff = (old_ampl - mnim_nol) / treb_ampl;
                    //for (int i = 0; i < refl.Length; i++)
                    //    reflectogramData[i] = (refl[i] - mnim_nol) / coeff;


                    //return reflectogramData;

                    return refl;

                }
                else
                {
                    errorMessage = _serialPort.PortName + " Не удалось очистить буфер приема/передачи ";
                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetReflectogram", errorMessage);
                    return null;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetReflectogram", errorMessage);
                return null;
            }

        }

        public double[] GetTemperature(ref string errorMessage)
        {
            try
            {
                double[] ChipTemps = new double[1];
                if (ASubDriver.ClearBuffer(_serialPort))
                {
                    ASubDriver.WriteCommand(_serialPort, ACommands.SendTemperature);
                    System.Threading.Thread.Sleep(10);
                    //подсчет того что пришло

                    byte[] buffer = ASubDriver.ReadWithTimeout(_serialPort);

                    if (ASubDriver.CheckCRC(buffer))
                    {
                        byte[] errorMessageWrongCRC = { 13, 13, 196, 229 };
                        if (buffer != errorMessageWrongCRC)
                        {
                            byte[] errorMessageWrongCommand = { 24, 24, 11, 186 };
                            if (buffer != errorMessageWrongCommand)
                            {
                                ChipTemps = new double[(buffer.Length - 2) / 2];
                                for (int j = 0; j < buffer.Length - 2; j += 2)
                                {
                                    double? temp = 0;
                                    double tempH, tempL, tempFrac;

                                    if (buffer[j + 1] <= 7)
                                    {
                                        tempH = ((byte)buffer[j + 1] & 15) << 4;
                                        tempL = ((byte)buffer[j] & 240) >> 4;
                                        tempFrac = 0.0625 * ((byte)buffer[j] & 15);
                                        temp = tempH + tempL + tempFrac;
                                    }
                                    else
                                    {
                                        tempH = ((byte)~buffer[j + 1] & 15) << 4;
                                        tempL = ((byte)~buffer[j] & 240) >> 4;
                                        tempFrac = 0.0625 * ((byte)~buffer[j] & 15) + 0.0625;
                                        temp = -(tempH + tempL + tempFrac);
                                    }

                                    ChipTemps[j / 2] = (double)Math.Round((double)temp, 1);
                                    //FileWorker.WriteFileParams("temperature" + channels[channelCount].ToString() + ".t", buffer);
                                }
                                return ChipTemps;
                            }
                            else
                            {
                                errorMessage = _serialPort.PortName + " Датчик получил сообщение состоящее не из 4-х байт:";
                                foreach (byte dataByte in buffer)
                                    errorMessage += " " + dataByte.ToString();
                                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetTemperature", errorMessage);
                                return null;
                            }
                        }
                        else
                        {
                            errorMessage = _serialPort.PortName + " Датчик получил сообщение c неправильным CRC: ";
                            foreach (byte dataByte in buffer)
                                errorMessage += " " + dataByte.ToString();
                            FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetTemperature", errorMessage);
                            return null;
                        }
                    }
                    else
                    {
                        errorMessage = _serialPort.PortName + " Ответ от датчика пришел с неправильным CRC: ";
                        foreach (byte dataByte in buffer)
                            errorMessage += " " + dataByte.ToString();
                        FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetTemperature", errorMessage);
                        return null;
                    }
                }
                else
                {
                    errorMessage = _serialPort.PortName + " Не удалось очистить буфер приема/передачи ";
                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetTemperature", errorMessage);
                    return null;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetTemperature", errorMessage);
                return null;
            }
        }

        public double GetLevel(ref string errorMessage)
        {
            try
            {
                if (ASubDriver.ClearBuffer(_serialPort))
                {
                    //System.Threading.Thread.Sleep(20);

                    ASubDriver.WriteCommand(_serialPort, ACommands.StartReflectogramConvertation, Average.SixteenReflectogram);
                    //System.Threading.Thread.Sleep(10);

                    byte[] subBuffer = ASubDriver.ReadWithTimeout(_serialPort, 3000);

                    if (ASubDriver.CheckCRC(subBuffer))
                    {
                        byte[] errorMessageWrongCRC = { 13, 13, 196, 229 };
                        if (subBuffer != errorMessageWrongCRC)
                        {
                            byte[] errorMessageWrongCommand = { 24, 24, 11, 186 };
                            if (subBuffer != errorMessageWrongCommand)
                            {
                                ASubDriver.ClearBuffer(_serialPort);
                                ASubDriver.WriteCommand(_serialPort, ACommands.SendLevel);
                                //System.Threading.Thread.Sleep(10);                            
                                //подсчет того что пришло

                                byte[] buffer = ASubDriver.ReadWithTimeout(_serialPort);

                                double temp = Convert.ToSingle(buffer[0] + buffer[1] * 256 + buffer[2] * 65536 + buffer[3] * 16777216);
                                double lvl = temp / (double)1000.0;
                                return lvl;

                            }
                            else
                            {
                                errorMessage = _serialPort.PortName + " Датчик получил сообщение состоящее не из 4-х байт(на этапе отправки команды)";
                                foreach (byte dataByte in subBuffer)
                                    errorMessage += " " + dataByte.ToString();
                                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetLevel", errorMessage);
                                return -1.0;
                            }
                        }
                        else
                        {
                            errorMessage = _serialPort.PortName + " Датчик получил сообщение c неправильным CRC(на этапе отправки команды)";
                            foreach (byte dataByte in subBuffer)
                                errorMessage += " " + dataByte.ToString();
                            FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetLevel", errorMessage);
                            return -1.0;
                        }
                    }
                    else
                    {
                        errorMessage = _serialPort.PortName + " Ответ от датчика пришел с неправильным CRC(на этапе отправки команды)";
                        foreach (byte dataByte in subBuffer)
                            errorMessage += " " + dataByte.ToString();
                        FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetLevel", errorMessage);
                        return -1.0;
                    }
                }
                else
                {
                    errorMessage = _serialPort.PortName + "Не удалось очистить буфер приема/передачи ";
                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetLevel", errorMessage);
                    return -1.0;
                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetLevel", errorMessage);
                return -1.0;
            }
        }

        public double GetLevelWithCorrection(ref string errorMessage)
        {
            try
            {
                if (ASubDriver.ClearBuffer(_serialPort))
                {

                    ASubDriver.WriteCommand(_serialPort, ACommands.SendLevelWithCorrection);
                    //System.Threading.Thread.Sleep(10);                            
                    //подсчет того что пришло


                    byte[] buffer = ASubDriver.ReadWithTimeout(_serialPort);

                    double temp = Convert.ToSingle(buffer[0] + buffer[1] * 256 + buffer[2] * 65536 + buffer[3] * 16777216);
                    double lvl = temp / (double)1000.0;

                    return lvl;
                   
                }
                else
                {
                    errorMessage = _serialPort.PortName + " Не удалось очистить буфер приема/передачи ";
                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetLevelWithCorrection", errorMessage);
                    return -1.0;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "GetLevelWithCorrection", errorMessage);
                return -1.0;
            }
        }

        public bool SetChannel(byte channel, ref string errorMessage)
        {
            try
            {
                if (ASubDriver.ClearBuffer(_serialPort))
                {
                    if (ASubDriver.WriteChannel(_serialPort, channel))
                    {
                        System.Threading.Thread.Sleep(100);
                        byte[] data = ASubDriver.ReadWithTimeout(_serialPort, 250);
                        if (data != null)
                        {
                            if (ASubDriver.CheckCRC(data))
                            {
                                if (data[0] == 255 && data[1] == channel)
                                {
                                    System.Threading.Thread.Sleep(_powerDelay);                                    
                                    return true;
                                }
                                else
                                {
                                    errorMessage = _serialPort.PortName+ " CH:" + channel.ToString()+ " Неправильная последовательность 4-х байт: ";
                                    foreach (byte dataByte in data)
                                        errorMessage += " " + dataByte.ToString();
                                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "SetChannel", errorMessage);
                                    return false;
                                }
                            }
                            else
                            {
                                errorMessage = _serialPort.PortName + " CH:" + channel.ToString() + " Данные пришли с неверным СRC: ";
                                foreach (byte dataByte in data)
                                    errorMessage += " " + dataByte.ToString();
                                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "SetChannel", errorMessage);
                                return false;
                            }
                        }
                        else
                        {
                            errorMessage = _serialPort.PortName + " CH:" + channel.ToString() + " Нет данных для считывания с СОМ-порта:" + _serialPort.PortName;                           
                            FileWorker.WriteEventFile(DateTime.Now, "ADriver", "SetChannel", errorMessage);
                            return false;
                        }

                    }
                    else
                    {
                        errorMessage = _serialPort.PortName + " CH:" + channel.ToString() + " Не удалось установить канал базового блока";                       
                        FileWorker.WriteEventFile(DateTime.Now, "ADriver", "SetChannel", errorMessage);
                        return false;
                    }
                }
                else
                {
                    errorMessage = _serialPort.PortName + " Не удалось очистить буфер обмена";                    
                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "SetChannel", errorMessage);
                    return false;
                }
            }
            catch (Exception ex)
            {
                 errorMessage =  ex.Message;
                 FileWorker.WriteEventFile(DateTime.Now, "ADriver", "SetChannel", errorMessage);
                return false;
            }
        }

        private int FindMax(double[] data, int begin, int end)
        {
            try
            {
                int maxindex = 0; // индекс макс. элемента массива
                double maxelement = data[maxindex]; //макс. элемент массива
                for (int i = begin; i < end; i++)
                {
                    if (maxelement < data[i])
                    {
                        maxelement = data[i];
                        maxindex = i;
                    }
                }
                return maxindex;
            }
            catch(Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "FindMax", ex.Message);
                return -1;
            }
        }

        private double[] ByteToDoubleArray(byte[] inputMass)
        {
            try
            {
                double[] outputMass = new double[inputMass.Length / 2];
                for (int i = 0; i < inputMass.Length; i += 2)
                {
                    outputMass[i / 2] = inputMass[i] + inputMass[i + 1] * 256;
                    if (outputMass[i / 2] > 2047)
                        outputMass[i / 2] = outputMass[i / 2] - 65536;
                }

                return outputMass;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "ByteToDoubleArray", ex.Message);
                return null;
            }
        }

        public bool CheckBaseblock(ref string errorMessage)
        {
            try
            {
                if (ASubDriver.ClearBuffer(_serialPort))
                {
                    if (ASubDriver.WriteChannel(_serialPort, 20))
                    {
                        System.Threading.Thread.Sleep(10);
                        byte[] data = ASubDriver.ReadWithTimeout(_serialPort, 150);
                        if (data != null)
                        {
                            if (ASubDriver.CheckCRC(data))
                            {
                                if (data[0] == 255 && data[1] == 20)
                                    return true;
                                else
                                {
                                    errorMessage = _serialPort.PortName + " Получено неправильное сообщение от базового блока. Cообщение:";
                                    foreach (byte dataByte in data)
                                        errorMessage += " " + dataByte.ToString();
                                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "CheckBaseblock", errorMessage);
                                    return false;
                                }
                            }
                            else
                            {
                                errorMessage = _serialPort.PortName + " Получено сообщение с неправильным CRC. Cообщение:";
                                foreach (byte dataByte in data)
                                    errorMessage += " " + dataByte.ToString();
                                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "CheckBaseblock", errorMessage);
                                return false;
                            }
                        }
                        else
                        {
                            //errorMessage = _serialPort.PortName + " Нет данных для считывания с СОМ-порта. (Отсутствие базового блока на линии)";                            
                            //FileWorker.WriteEventFile(DateTime.Now, "ADriver", "CheckBaseblock", errorMessage);
                            return false;
                        }
                    }
                    else
                    {
                        errorMessage = _serialPort.PortName + " Не возможно отправить данные в " + _serialPort.PortName;
                        FileWorker.WriteEventFile(DateTime.Now, "ADriver", "CheckBaseblock", errorMessage);
                        return false;
                    }
                }
                else
                {
                    errorMessage = _serialPort.PortName + " Не удалось очистить буфер приема/передачи ";
                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "CheckBaseblock", errorMessage);
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "CheckBaseblock", errorMessage);
                return false;
            }
        }

        public bool ResetChannel(ref string errorMessage)
        {
            try
            {
                if (ASubDriver.ClearBuffer(_serialPort))
                {
                    if (ASubDriver.WriteChannel(_serialPort, 20))
                    {
                        System.Threading.Thread.Sleep(10);
                        byte[] data = ASubDriver.ReadWithTimeout(_serialPort, 250);
                        if (data != null)
                        {
                            if (ASubDriver.CheckCRC(data))
                            {
                                if (data[0] == 255 && data[1] == 20)
                                    return true;
                                else
                                {
                                    errorMessage = _serialPort.PortName + " Получено неправильное сообщение от базового блока. Cообщение:";
                                    foreach (byte dataByte in data)
                                        errorMessage += " " + dataByte.ToString();
                                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "ResetChannel", errorMessage);
                                    return false;
                                }
                            }
                            else
                            {
                                errorMessage = _serialPort.PortName + " Получено сообщение с неправильным CRC. Cообщение:";
                                foreach (byte dataByte in data)
                                    errorMessage += " " + dataByte.ToString();
                                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "ResetChannel", errorMessage);
                                return false;
                            }
                        }
                        else
                        {
                            errorMessage = _serialPort.PortName + " Нет данных для чтения";
                            FileWorker.WriteEventFile(DateTime.Now, "ADriver", "ResetChannel", errorMessage);
                            return false;
                        }
                    }
                    else
                    {
                        errorMessage = "Не возможно отправить данные в " + _serialPort.PortName;
                        FileWorker.WriteEventFile(DateTime.Now, "ADriver", "ResetChannel", errorMessage);
                        return false;
                    }
                }
                else
                {
                    errorMessage = _serialPort.PortName + " Не удалось очистить буфер приема/передачи ";
                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "ResetChannel", errorMessage);
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "ResetChannel", errorMessage);
                return false;
            }
        }

        public bool CheckSensor(byte channel, ref string errorMessage)
        {
            try
            {
                if (ASubDriver.ClearBuffer(_serialPort))
                {
                    if (SetChannel(channel, ref errorMessage))
                    {
                        double firmware = GetFirmware(ref errorMessage);
                        if (firmware > 0)
                            return true;
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        errorMessage = _serialPort.PortName + " Не удалось установить " + channel.ToString() + " канал базового блока";
                        FileWorker.WriteEventFile(DateTime.Now, "ADriver", "CheckSensor", errorMessage);
                        return false;
                    }
                }
                else
                {
                    errorMessage = _serialPort.PortName + " Не удалось очистить буфер приема/передачи ";
                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "CheckSensor", errorMessage);
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "CheckSensor", errorMessage);
                return false;
            }
        }

        public bool CheckSensor(ref string errorMessage)
        {
            try
            {
                if (ASubDriver.ClearBuffer(_serialPort))
                {
                    double firmware = GetFirmware(ref errorMessage);
                    if (firmware > 0)
                        return true;
                    else
                    {
                        errorMessage = _serialPort.PortName + " Неверное начение прошивки: " + firmware.ToString();
                        FileWorker.WriteEventFile(DateTime.Now, "ADriver", "CheckSensor", errorMessage);
                        return false;
                       
                    }
                }
                else
                {
                    errorMessage = _serialPort.PortName + " Не удалось очистить буфер приема/передачи ";
                    FileWorker.WriteEventFile(DateTime.Now, "ADriver", "CheckSensor", errorMessage);
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "CheckSensor", errorMessage);
                return false;
            }
        }

        internal bool ClosePort()
        {
            try
            {
                _serialPort.Close();
                return !_serialPort.IsOpen;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ADriver", "ClosePort", ex.Message);
                return false;
            }
        }

        internal static Dictionary<string, string> GetBaseblockInfo(Dictionary<string, List<byte>> devicesForExperiment)
        {
            throw new NotImplementedException();
        }
    }

}
