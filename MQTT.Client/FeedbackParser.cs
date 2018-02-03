using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Model.Enums;

namespace SmartHome.MQTT.Client
{
    public class CommandParser
    {
        private byte[] _command;
        public CommandId CommandId { get; set; }


        public ChannelConfigurationLoads ChannelConfigurationLoads { get; set; }

        public StatusType StatusType { get; set; }

        public int Initiator { get; set; }

        public int Length { get; set; }

        public int ThermalShutDownValue { get; set; }
        public List<ChannelValues> ChannelValueList { get; set; }

        public class ChannelValues
        {

            public ChannelId ChannelId { get; set; }

            public int CommandValue { get; set; }
            public int DimmingValue { get; set; }

            public int IndicatorValue { get; set; }


        }



        public CommandParser(byte[] command)
        {

            _command = command;
            ChannelValueList = new List<ChannelValues>();
            Parse(command);
        }

        private void Parse(byte[] command)
        {
            int loopLength;
            for (int i = 0; i < command.Length; i++)
            {
                if (i == 0)
                    Initiator = Convert.ToInt32((byte) command[i]);
                else if (i == 1)
                    CommandId = (CommandId) Enum.ToObject(typeof (CommandId), Convert.ToInt32((byte) command[i]));
                else if (i == 2)
                    Length = Convert.ToInt32((byte) command[i]);
                else if (i == 3)
                {
                    ThermalShutDownValue = Convert.ToInt32((byte) command[i]);
                    break;
                }
            }

            if (Length == 32)
                loopLength = 28;
            else
                loopLength = 8;


            ChannelValues cv;
            for (int i = 4; i < loopLength; i++)
            {
                cv = new ChannelValues();
                if (i%4 == 0)
                    cv.ChannelId = (ChannelId) Enum.ToObject(typeof (ChannelId), Convert.ToInt32((byte) command[i]));
                else if (i%5 == 0)
                    cv.CommandValue = Convert.ToInt32((byte) command[i]);
                else if (i%6 == 0)
                    cv.DimmingValue = Convert.ToInt32((byte) command[i]);
                else if (i%7 == 0)
                    cv.IndicatorValue = Convert.ToInt32((byte) command[i]);

                ChannelValueList.Add(cv);
            }
        }
    }
}
