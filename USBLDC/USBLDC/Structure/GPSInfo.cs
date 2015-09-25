using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyMetroWpfLibrary.Utility;
namespace USBLDC.Structure
{

    public class GPSInfo : StructureInterface
    {
        public uint GPSSecond { get; set; }
        public uint GPSMicSecond { get; set; }
        public float Long { get; set; }
        public float Lat { get; set; }
        public float Height { get; set; }
        public float Velocity { get; set; }
        public float Heading { get; set; }
        public uint SatNum { get; set; }
        public uint GPSStatus { get; set; }
        public uint[] Reserved = new uint[7];
        public bool Parse(byte[] bytes)
        {
            BitConverter.ToString(bytes);
            return false;
        }

        public byte[] SavePakage()
        {
            byte[] bytes = new byte[64];
            int offset = 0;
            Buffer.BlockCopy(BitConverter.GetBytes(GPSSecond), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(GPSMicSecond), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Long), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Lat), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Height), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Velocity), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Heading), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(SatNum), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(GPSStatus), 0, bytes, offset, 4);
            offset += 4;
            return bytes;
        }
    }
}
