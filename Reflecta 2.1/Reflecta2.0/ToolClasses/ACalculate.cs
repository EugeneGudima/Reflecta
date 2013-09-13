using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reflecta2._0
{
    enum ADelayCalculationType
    {
        OneZond,
        TwoZond
    }

    enum ACorrectionType
    {
        Easy,
        Hard
    }

    enum AFindType
    {        
        Zond,
        Reflex,
        End,
        Pulse
    }

     public enum ACalcType
        {
            OneZond,
            TwoZond
        }

    class ACalculate
    {
        static double[] amplitudes = new double[3];

        public static double[] Amplitudes
        {
            get { return amplitudes; }
        }
        public static double Level(double[] delays, ACalcType calcType)
        {
            try
            {
                double result = -1;
                switch (calcType)
                {
                    case (ACalcType.OneZond):
                        break;
                    case (ACalcType.TwoZond):
                        double zondOtrDelay = delays[1] - delays[0];
                        double zondZondDelay = delays[2] - delays[0];
                        result = ((zondOtrDelay * 299792458 * 1000) / (2 * (double)2000021.0031867027 * zondZondDelay)) - (double)83.8446268766;	
                        break;
                }

                return result;
            }
            catch (Exception ex)
            {
                //FileWorker.WriteEventFile(DateTime.Now, "ACalculate", "Level", ex.Message);//16.08 - т.к. не видно отраженного
                return -1.0;
            }
        }

        public static double LevelWithCorrection(double[] delays, ACorrectionType type)
        {
            try
            {
                double result = 0;
                if (delays != null)
                {
                }
                //result = AMath.GetLevelFromDelays(delays, 0, 3); //сменить на переменные
                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ACalculate", "LevelWithCorrection", ex.Message);
                return -1.0;
            }
        }

        public static double[] CalculateDelays(double[] reflectogram, ADelayCalculationType calcType)
        {
            try
            {
                double[] result = new double[3];
                if (reflectogram != null)
                {
                    switch (calcType)
                    {
                        case ADelayCalculationType.OneZond:
                            result = CalculateDelaysWithEasyCalculationType(reflectogram, 40, 40, 0.2, 0.2, 0.5);
                            break;
                        case ADelayCalculationType.TwoZond:
                            result = CalculateDelaysWithHardCalculationType(reflectogram, 40, 40, 0.2, 0.2, 0.5);
                            break;
                    }
                    return result;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                //FileWorker.WriteEventFile(DateTime.Now, "ACalculate", "CalculateDelays", ex.Message);//16.08 - т.к. не видно отраженного
                return null;
            }
           
        }

        private static double[] CalculateDelaysWithEasyCalculationType(double[] reflectogram, int right, int left, double porogE, double porogS, double front_aprox)
        {
            double[] result = new double[3];
            try
            {
                int[] pulses = FindAllPulses(reflectogram, right, left, porogE, porogS);
                amplitudes[0] = reflectogram[pulses[0]];
                amplitudes[1] = reflectogram[pulses[1]];
                amplitudes[2] = reflectogram[pulses[2]];
                double[] derivation = Derivative(reflectogram);

                //аппроксимация зонда и др. импульсов
                //если найден зонд
                int zondmaxpr = 0;
                double tnzond = 0; //начало зонда по аппроксимации произв. в макс..                
                if (pulses[0] > 0)
                {
                    zondmaxpr = Find(derivation, pulses[0] - left, pulses[0], AFindType.Pulse);
                    double point_value = front_aprox * reflectogram[pulses[0]];                    
                    tnzond = LineApprox(zondmaxpr, reflectogram, point_value);                    
                }
                //поиск максимума произв. отраж.
                int otrmaxpr = 0;
                double tnotr = 0;               
                if (pulses[1] > 0)
                {
                    otrmaxpr = Find(derivation, pulses[1] - left, pulses[1], AFindType.Pulse);
                    double point_value = front_aprox * reflectogram[pulses[1]];                    
                    tnotr = LineApprox(otrmaxpr, reflectogram, point_value);
                }

                //поиск максимума произв. конца
                int endmaxpr = 0;
                double tnend = 0; //начало конца по аппроксимации                
                if (pulses[2] > 0)
                {
                    endmaxpr = Find(derivation, pulses[2] - left, pulses[2], AFindType.Pulse);
                    double point_value = front_aprox * reflectogram[pulses[2]];                    
                    tnend = LineApprox(endmaxpr, reflectogram, point_value);
                }
                 
                result[0] = tnzond;                
                result[1] = tnotr;                
                result[2] = tnend;

                return result;
                
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ACalculate", "CalculateDelaysWithEasyCalculationType", ex.Message);
                return null;
            }
            
        }

        private static int[] FindAllPulses(double[] reflectogram, int right, int left, double porogE, double porogS)
        {
            try
            {
                int[] result = { -1, -1, -1 };
                result[0] = Find(reflectogram, 0, reflectogram.Length - 1);

                result[2] = Find(reflectogram, result[0] + 100, reflectogram.Length - 1, AFindType.End, reflectogram[result[0]] * porogE);


                if (result[2] > 0) //если найден конец
                    result[1] = Find(reflectogram, result[0] + right, result[2] - left, AFindType.Reflex, reflectogram[result[0]] * -porogS);
                else
                    result[1] = Find(reflectogram, result[0] + right, reflectogram.Length - 1, AFindType.Reflex, reflectogram[result[0]] * -porogS);

                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ACalculate", "FindAllPulses", ex.Message);
                return null;
            }
        }

        private static int[] FindAllPulsesTwoZond(double[] reflectogram, int right, int left, double porogE, double porogS)
        {
            try
            {
                int[] result = { -1, -1, -1 };
                result[0] = Find(reflectogram, 0, reflectogram.Length - 2056);

                result[2] = Find(reflectogram, result[0] + 2056, reflectogram.Length - 1, AFindType.Zond, reflectogram[result[0]] * porogE);


                if (result[2] > 0) //если найден конец
                    result[1] = Find(reflectogram, result[0] + right, result[2] - left, AFindType.Reflex, reflectogram[result[0]] * -porogS);
                else
                    result[1] = Find(reflectogram, result[0] + right, reflectogram.Length - 1, AFindType.Reflex, reflectogram[result[0]] * -porogS);

                return result;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ACalculate", "FindAllPulsesTwoZond", ex.Message);
                return null;
            }
        }

        private static int Find(double[] data, int begin, int end, AFindType type = AFindType.Zond, double porog = 0)
        {
            try
            {
                int index = 0; // индекс макс. элемента массива
                double maxelement = data[index]; //макс. элемент массива
                for (int i = begin; i < end; i++)
                {
                    if (maxelement < data[i])
                    {
                        maxelement = data[i];
                        index = i;
                    }
                }
                switch (type)
                {
                    case AFindType.Reflex:
                        index = 0; // индекс мин. элемента массива
                        double minelement = data[index]; //мин. элемент массива
                        for (int i = begin; i < end; i++)
                        {
                            if (minelement > data[i])
                            {
                                minelement = data[i];
                                index = i;
                            }
                        }
                        if (data[index] > porog) index = -1;
                        break;
                    case AFindType.End:
                        if (data[index] < porog) index = -1;
                        break;
                    case AFindType.Pulse:
                        if (begin < 0) begin = 0;
                        if (end > data.Length - 1) end = data.Length - 1;
                        index = 0; // индекс макс. элемента массива
                        maxelement = data[index]; //макс. элемент массива
                        for (int i = begin; i < end; i++)
                        {
                            if (maxelement < Math.Abs(data[i]))
                            {
                                maxelement = Math.Abs(data[i]);
                                index = i;
                            }
                        }
                        break;
                }
                return index;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ACalculate", "Find", ex.Message);
                return -1;
            }
           
        }

        private static double LineApprox(int zondmaxpr, double[] refl, double point_value)
        {
            try
            {
                double b = 0;
                double k = 0;
                double zad = 0;
                double[] der = Derivative(refl);
                b = refl[zondmaxpr] - der[zondmaxpr] * zondmaxpr;
                k = der[zondmaxpr];
                zad = (point_value - b) / k;
                return zad;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ACalculate", "LineApprox", ex.Message);
                return -1.0;
            }
        } // rewrite

        private static double[] Derivative(double[] data)
        {
            try
            {
                double[] derivative = new double[data.Length];
                for (int i = 1; i < data.Length; i++)
                {
                    derivative[i] = data[i] - data[i - 1];
                }
                derivative[0] = derivative[1];
                return derivative;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "ACalculate", "Derivative", ex.Message);
                return null;
            }
        } //rewrite       

        private static double[] CalculateDelaysWithHardCalculationType(double[] reflectogram, int right, int left, double porogE, double porogS, double front_aprox)
        {            
            double[] result = new double[3];
            try
            {
                int[] pulses = FindAllPulsesTwoZond(reflectogram, right, left, porogE, porogS);
                amplitudes[0] = reflectogram[pulses[0]];
                amplitudes[1] = reflectogram[pulses[1]];
                amplitudes[2] = reflectogram[pulses[2]];
                double[] derivation = Derivative(reflectogram);

                //аппроксимация зонда и др. импульсов
                //если найден зонд
                int zondmaxpr = 0;
                double tnzond = 0; //начало зонда по аппроксимации произв. в макс..                
                if (pulses[0] > 0)
                {
                    zondmaxpr = Find(derivation, pulses[0] - left, pulses[0], AFindType.Pulse);
                    double point_value = front_aprox * reflectogram[pulses[0]];
                    tnzond = LineApprox(zondmaxpr, reflectogram, point_value);
                }
                //поиск максимума произв. отраж.
                int otrmaxpr = 0;
                double tnotr = 0;
                if (pulses[1] > 0)
                {
                    otrmaxpr = Find(derivation, pulses[1] - left, pulses[1], AFindType.Pulse);
                    double point_value = front_aprox * reflectogram[pulses[1]];
                    tnotr = LineApprox(otrmaxpr, reflectogram, point_value);
                }

                //поиск максимума произв. конца
                int endmaxpr = 0;
                double tnend = 0; //начало конца по аппроксимации                
                if (pulses[2] > 0)
                {
                    endmaxpr = Find(derivation, pulses[2] - left, pulses[2], AFindType.Pulse);
                    double point_value = front_aprox * reflectogram[pulses[2]];
                    tnend = LineApprox(endmaxpr, reflectogram, point_value);
                }

                result[0] = tnzond;
                result[1] = tnotr;
                result[2] = tnend;
                 return result;

            }
            catch (Exception ex)
            {
                //FileWorker.WriteEventFile(DateTime.Now, "ACalculate", "CalculateDelaysWithHardCalculationType", ex.Message); //16.08 - т.к. не видно отраженного
                return null;
            }
           
        }
    }
}
