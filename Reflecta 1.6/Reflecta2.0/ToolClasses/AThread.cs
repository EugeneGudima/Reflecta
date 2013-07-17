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
                string error = "";
                Start();

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
                    driver.OpenPort(ref error);
                    if (error != "")
                    {
                    }
                    bw.RunWorkerAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
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
                    if (driver.ClosePort())
                        return !bw.IsBusy;
                    else
                        return false;
                }
                else
                    return false;
            }
            catch
            {
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

        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ADatabaseWorker.WriteSensorReflectogram(db, e.UserState as Sensor, device, experimentGroup, e.ProgressPercentage);
            ADatabaseWorker.WriteSensorStatistic(db, e.UserState as Sensor, device, experimentGroup, e.ProgressPercentage);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            while (running)
            {
                for (byte i = 0; i < device.channelsWithSensors.Count; i++)
                {

                    if (driver.SetChannel(device.channelsWithSensors[i]))
                    {
                        sensorsData[i].SensorCalcLevel = driver.GetLevel(ref error);
                        //sensorsData[i].SensorCalcLevelWithCorrection = driver.GetLevelWithCorrection(ref error);
                        if (getReflectogram)
                        {
                            sensorsData[i].Reflectogram = driver.GetReflectogram(ref error);
                            sensorsData[i].ComputerDelays = ACalculate.CalculateDelays(sensorsData[i].Reflectogram, ADelayCalculationType.TwoZond);
                            sensorsData[i].ComputerCalcLevel = ACalculate.Level(sensorsData[i].ComputerDelays, twoZond ? ACalcType.TwoZond : ACalcType.OneZond);
                            //sensorsData[i].ComputerCalcLevelWithCorrection = ACalculate.LevelWithCorrection(sensorsData[i].ComputerDelays, ACorrectionType.Easy);
                        }
                        double[] temperature = driver.GetTemperature(ref error);
                        if (temperature != null)
                        {
                            sensorsData[i].SensorTemperature = temperature[0];
                            if (temperature.Length > 1)
                                temperature.CopyTo(sensorsData[i].TemperatureArray, 1);
                            else
                                sensorsData[i].TemperatureArray = null;
                        }
                        else
                        {
                            sensorsData[i].SensorTemperature = -273.0;
                            sensorsData[i].TemperatureArray = null;
                        }

                        sensorsData[i].SensorDelays = driver.GetDelays(ref error);

                        bw.ReportProgress(i, sensorsData[i]);


                    }
                    else
                    {
                    }

                    if (driver.ResetChannel(ref error))
                    {
                        System.Threading.Thread.Sleep(commDelay);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(commDelay);
                        //driver.ClosePort();
                        //driver.OpenPort(ref error);
                    }

                }
            }

        }
    }
}
