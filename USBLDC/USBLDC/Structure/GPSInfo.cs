using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyMetroWpfLibrary.Utility;
using USBLDC.Helpers;
using NMEA0183;
namespace USBLDC.Structure
{

    public class GPSInfo : StructureInterface
    {
        public int GPSSecond { get; set; }
        public int GPSMicSecond { get; set; }
        public float Long { get; set; }
        public float Lat { get; set; }
        public float Height { get; set; }
        public float Velocity { get; set; }
        public float Heading { get; set; }
        public uint SatNum { get; set; }
        public uint GPSStatus { get; set; }
        public uint[] Reserved = new uint[7];
        public static int count=0;
        public bool Parse(byte[] bytes)
        {
            string msg = Encoding.Default.GetString(bytes);
            char[] End = "\r\n".ToCharArray();
            if(GPS.Parse(msg.TrimEnd(End)))
            {
                GPSStatus = GPS.Status;
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                GPSSecond = (int)GPS.UTCTime.Subtract(epoch).TotalSeconds;
                //GPSSecond = 1470986158 + count;
                //count = count + 1;
                GPSMicSecond = GPS.UTCTime.Subtract(epoch).Milliseconds; ;
                if (GPS.Status == 1 || GPS.Status==2)
                {                    
                    Long = GPS.Longitude;
                    Lat = GPS.Latitude;
                    Height = GPS.Height;
                    Velocity = GPS.Speed;
                    Heading = GPS.Heading;
                    SatNum = GPS.SatNum;
                    return true;
                }

            }
            return false;
        }

        public byte[] SavePackage()
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
