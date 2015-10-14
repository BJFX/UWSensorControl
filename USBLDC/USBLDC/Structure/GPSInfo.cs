using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyMetroWpfLibrary.Utility;
using USBLDC.Helpers;
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
                string newcomming = BitConverter.ToString(bytes);
                char[] charSeparators = new char[] {',','*','#','$',';'};
                try
                {
                    string[] gpsinfo = newcomming.Split(charSeparators, StringSplitOptions.None);
                    switch (gpsinfo[1])
                    {
                        case "GPRMC":
                            if (GpsHelper.CalculateCheckSum(newcomming).ToUpper() == gpsinfo[gpsinfo.Length - 1])
                            {
                                if (gpsinfo[3] == "A")//数据有效
                                {
                                    
                                    int Year = UInt16.Parse(gpsinfo[10].Substring(4))+2000;
                                    int Month = UInt16.Parse(gpsinfo[10].Substring(2, 2));
                                    int Day = UInt16.Parse(gpsinfo[10].Substring(0, 2));
                                    int Hour = UInt16.Parse(gpsinfo[2].Substring(0, 2));
                                    int Minute = UInt16.Parse(gpsinfo[2].Substring(2, 2));
                                    int Second = UInt16.Parse(gpsinfo[2].Substring(4, 2));
                                    DateTime tm = new DateTime(Year, Month, Day, Hour, Minute, Second);
                                }
                                else
                                {
                                    
                                }
                                return false;
                            }
                            
                            break;
                        case "BESTPOSA":
                            if (GpsHelper.CalculateBlockCRC32(newcomming.Substring(1, newcomming.Length-10)) == gpsinfo[gpsinfo.Length - 1])
                            {
                                
                            }
                            return true;
                            break;

                        default:
                            return false;
                            break;

                    }
                    return false;
            }
            catch (Exception)
            {
                
                return false;
            }
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
