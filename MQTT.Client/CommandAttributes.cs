using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.MQTT.Client
{
    public class CommandAttributes
    {
        public int MByte1CommandId { get; set; }
        public int MByte2ChannelId { get; set; }
        public int MByte3Data0 { get; set; }
        private int MByte4Data1 { get; set; }
        private int MByte5Data2 { get; set; }
        private int _mByte6Data3;

        public int MByte0VersionAndDirection { get; set; }


        public CommandAttributes()
        {
            this.MByte0VersionAndDirection = -1;
            this.MByte1CommandId = -1;
            this.MByte2ChannelId = -1;
            this.MByte3Data0 = -1;
            this.MByte4Data1 = -1;
            this.MByte5Data2 = -1;
        }

        public CommandAttributes(int mByte0VersionAndDirection, int mByte1CommandId, int mByte2ChannelId,
                                 int mByte3Data0, int mByte4Data1, int mByte5Data2, int mByte6Data3)
        {
            this.MByte0VersionAndDirection = mByte0VersionAndDirection;
            this.MByte1CommandId = mByte1CommandId;
            this.MByte2ChannelId = mByte2ChannelId;
            this.MByte3Data0 = mByte3Data0;
            this.MByte4Data1 = mByte4Data1;
            this.MByte5Data2 = mByte5Data2;
            this._mByte6Data3 = mByte6Data3;
        }

        //public override String toString()
        //{
        //    return "CommandAttributes{" +
        //            "mByte0VersionAndDirection=" + MByte0VersionAndDirection +
        //            ", mByte1CommandId=" + MByte1CommandId +
        //            ", mByte2ChannelId=" + MByte2ChannelId +
        //            ", mByte3Data0=" + MByte3Data0 +
        //            ", mByte4Data1=" + MByte4Data1 +
        //            ", mByte5Data2=" + MByte5Data2 +
        //            ", mByte6Data3=" + _mByte6Data3 +
        //            '}';
        //}

    }

}
