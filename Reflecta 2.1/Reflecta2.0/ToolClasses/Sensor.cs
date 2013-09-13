using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reflecta2._0
{
    class Sensor
    {
        double[] reflectogram;
        double[] temperatureArray;
        double[] sensorDelays;
        double[] computerDelays;
        double[] computerAmplitudes;
        double sensorTemperature;
        double sensorCalcLevel;
        double sensorCalcLevelWithCorrection;
        double computerCalcLevel;
        double computerCalcLevelWithCorrection;

        public double[] Reflectogram
        {
            set
            {
                if (value != null)
                {
                    if (value.Length == 4096)
                        reflectogram = value;
                    else
                        reflectogram = null;
                }
                else
                    reflectogram = null;
            }
            get { return reflectogram; }
        }

        public double[] TemperatureArray
        {
            set
            {
                if(temperatureArray == null)
                    temperatureArray = new double[1];
                if (value != null)
                    temperatureArray = value;
                else
                    temperatureArray = null;

            }
            get { return temperatureArray; }
        }

        public double[] SensorDelays
        {
            set
            {
                if (value != null)
                {
                    if (value.Length == 3)
                        sensorDelays = value;
                    else
                        sensorDelays = null;
                }
                else
                    sensorDelays = null;
            }
            get { return sensorDelays; }
        }

        public double[] ComputerDelays
        {
            set
            {
                if (value != null)
                {
                    if (value.Length == 3)
                        computerDelays = value;
                    else
                        computerDelays = null;
                }
                else
                    computerDelays = null;
            }
            get { return computerDelays; }
        }

        public double[] ComputerAmplitudes
        {
            set
            {
                if (value != null)
                {
                    if (value.Length == 3)
                        computerAmplitudes = value;
                    else
                        computerAmplitudes = null;
                }
                else
                    computerAmplitudes = null;
            }
            get { return computerAmplitudes; }
        }

        public double SensorTemperature
        {
            set { sensorTemperature = value; }
            get { return sensorTemperature; }
        }

        public double SensorCalcLevel
        {
            set { sensorCalcLevel = value; }
            get { return sensorCalcLevel; }
        }

        public double SensorCalcLevelWithCorrection
        {
            set { sensorCalcLevelWithCorrection = value; }
            get { return sensorCalcLevelWithCorrection; }
        }

        public double ComputerCalcLevel
        {
            set { computerCalcLevel = value; }
            get { return computerCalcLevel; }
        }

        public double ComputerCalcLevelWithCorrection
        {
            set { computerCalcLevelWithCorrection = value; }
            get { return computerCalcLevelWithCorrection; }
        }
       
    }
}
