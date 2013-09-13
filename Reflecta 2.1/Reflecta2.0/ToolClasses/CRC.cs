using System;


namespace Reflecta2._0
{
    class CRC
    {
        public static bool CheckResponse(byte[] response)
        {
            try
            {
                //Perform a basic CRC check:
                byte[] CRC = new byte[2];
                GetCRC(response, ref CRC);
                if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "CRC", "CheckResponse", ex.Message);
                return false;
            }
        }

        public static void GetCRC(byte[] message, ref byte[] CRC)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:
            try
            {
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
            }
            catch (Exception ex)
            {
                FileWorker.WriteEventFile(DateTime.Now, "CRC", "GetCRC", ex.Message);
            }
        }
    }
}
