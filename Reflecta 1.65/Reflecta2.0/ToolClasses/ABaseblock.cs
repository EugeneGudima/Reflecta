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
                int comStart = device.Key.IndexOf('(') + 1;
                int comEnd = device.Key.IndexOf(')') - comStart;
                result[count].comport = device.Key.Substring(comStart, comEnd);
                result[count].channelsWithSensors = device.Value;
                result[count].baseblockSerial = device.Key.Substring(0, comStart-1);
                count++;
            }

            return result;

        }
    }
}
