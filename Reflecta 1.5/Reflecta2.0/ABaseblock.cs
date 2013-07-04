using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reflecta2._0
{    

    class ABaseblock
    {
        public int channelscount;
        public string comport;
        public string baseblockSerial;
        public List<byte> channelsWithSensors;
        public int baseblockID;
        public List<int> sysChannelsIDs;

        public static List<ABaseblock> WriteBaseblockInfo(Dictionary<string, List<byte>> devicesForExperiment)
        {
            List<ABaseblock> result = new List<ABaseblock>();
            int count = 0;

            foreach (KeyValuePair<string, List<byte>> device in devicesForExperiment)
            {
                result.Add(new ABaseblock());
                result[count].comport = device.Key;
                result[count].channelsWithSensors = device.Value;
                count++;
            }

            return result;

        }
    }
}
