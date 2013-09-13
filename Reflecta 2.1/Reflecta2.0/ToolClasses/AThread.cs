using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Reflecta2._0
{
    class AThread
    {
        BackgroundWorker bw = new BackgroundWorker();
        bool running = true;
        List<Sensor> sensorsData = new List<Sensor>();
        ADriver driver;
        ADatabase db;
        int experimentGroup;
        private ABaseblock device;
        private string error = "";
        private bool getReflectogram = true;
        private int powerDelay;
        private int commDelay;
        private bool twoZond = true;

        public AThread(ABaseblock _device, int _experimentGroup, ADatabase _db, int _powerDelay, int _commDelay, bool isStartWithInitialize = false)
        {
            try
            {
                // TODO: Complete member initialization
                device = _device;
                powerDelay = _powerDelay;
                commDelay = _commDelay;
                experimentGroup = _experimentGroup;
                db = _db;

                bw.DoWork += bw_DoWork;
                bw.ProgressChanged += bw_ProgressChanged;
                bw.RunWorkerCompleted += bw_RunWorkerCompleted;
                bw.WorkerReportsProgress = true;
                bw.WorkerSupportsCancellation = true;

                foreach (byte channel in device.channelsWithSensors)
                {
                    sensorsData.Add(new Sensor());
                    driver = new ADriver(device.comport, powerDelay);
                }

                if (isStartWithInitialize)
                {
                    Start();
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "AThread", "AThread", ex.Message);
            }
        }

        public bool GetReflectogram
        {
            get { return getReflectogram; }
            set { getReflectogram = value; }
        }

        public bool Start()
        {
            try
            {
                if (!bw.IsBusy && driver != null && device.channelsWithSensors != null && device.sysChannelsIDs != null && device.baseblockID > 0)
                {
                    running = true;
                   
                    if (driver.OpenPort(ref error))
                    {                       
                    }
                    else
                        FileWorker.WriteEventFile(DateTime.Now, "AThread", "Start", error);
                    bw.RunWorkerAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "AThread", "Start", ex.Message);
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                if (bw.IsBusy)
                {
                    running = false;
                    bw.CancelAsync();
                    return true;
                }
                else
                    return false;
            }
            catch(Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "AThread", "Stop", ex.Message);
                return false;
            }
        }

        public bool ThreadIsBusy
        {
            get { return bw.IsBusy; }
        }

        public bool TwoZond
        {
            get { return twoZond; }
            set { twoZond = value; }
        }

        public bool WrongIni
        {
            get { return driver == null || device.channelsWithSensors != null || device.sysChannelsIDs != null || device.baseblockID > 0; }
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!driver.ClosePort())          
            {
                FileWorker.WriteEventFile(DateTime.Now, "AThread", "Stop", "Не удалось закрыть порт");                
            }
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                DateTime time = DateTime.Now;
                ADatabaseWorker.WriteSensorReflectogram(db, time, e.UserState as Sensor, device, experimentGroup, e.ProgressPercentage);
                ADatabaseWorker.WriteSensorStatistic(db, time, e.UserState as Sensor, device, experimentGroup, e.ProgressPercentage);
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "AThread", "bw_ProgressChanged", ex.Message);
            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (running)
                {
                    for (byte i = 0; i < device.channelsWithSensors.Count && running; i++)
                    {
                        if (device.channelsWithSensors != null)
                        {
                            if (driver.SetChannel(device.channelsWithSensors[i], ref error))
                            {
                                sensorsData[i].SensorCalcLevel = driver.GetLevel(ref error);
                                sensorsData[i].SensorDelays = driver.GetDelays(ref error);

                                double[] temperature = driver.GetTemperature(ref error);
                                if (temperature != null)
                                {
                                    sensorsData[i].SensorTemperature = temperature[0];
                                    sensorsData[i].TemperatureArray = new double[temperature.Length - 1];
                                    if (temperature.Length > 1)
                                        Array.Copy(temperature, 1, sensorsData[i].TemperatureArray, 0, temperature.Length - 1);
                                    else
                                        sensorsData[i].TemperatureArray = null;
                                }
                                else
                                {
                                    sensorsData[i].SensorTemperature = -273.0;
                                    sensorsData[i].TemperatureArray = null;
                                }

                                //sensorsData[i].SensorCalcLevelWithCorrection = driver.GetLevelWithCorrection(ref error);
                                if (getReflectogram)
                                {
                                    sensorsData[i].Reflectogram = driver.GetReflectogram(ref error);
                                    sensorsData[i].ComputerDelays = ACalculate.CalculateDelays(sensorsData[i].Reflectogram, ADelayCalculationType.TwoZond);
                                    sensorsData[i].ComputerCalcLevel = ACalculate.Level(sensorsData[i].ComputerDelays, twoZond ? ACalcType.TwoZond : ACalcType.OneZond);
                                    sensorsData[i].ComputerAmplitudes = ACalculate.Amplitudes;
                                    //sensorsData[i].ComputerCalcLevelWithCorrection = ACalculate.LevelWithCorrection(sensorsData[i].ComputerDelays, ACorrectionType.Easy);
                                }


                                bw.ReportProgress(i, sensorsData[i]);


                                if (device.channelsWithSensors != null)
                                {
                                    if (!driver.ResetChannel(ref error))
                                    {
                                        FileWorker.WriteEventFile(DateTime.Now, "AThread", "bw_DoWork", error);
                                    }
                                }
                            }
                        }
                        

                    }
                }
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "AThread", "bw_DoWork", ex.Message);
            }

        }
    }
}
