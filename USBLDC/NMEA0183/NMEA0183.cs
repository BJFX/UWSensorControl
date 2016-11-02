using System;
using System.Collections.Generic;
using System.Text;

namespace NMEA0183
{
    public class GPS
    {
        public static DateTime UTCTime;
        public static float Longitude;
        public static float Latitude;
        public static float Height;
        public static uint SatNum;
        public static float Speed;
        public static float Heading;
        public static uint Status;//0:未定位1:无差分2:有差分.97:checksum error,98:invalid data,99:parse exception
        /// <summary>
        /// 解析NMEA0183格式的GPS信息GPRMC GPGSV
        /// </summary>
        /// <returns></returns>
        public static bool Parse(string msg)
        {
            bool ret = true;
            string newcomming = msg;
                char[] charSeparators = new char[] {',','*','#','$',';'};
            try
            {
                string[] gpsinfo = newcomming.Split(charSeparators, StringSplitOptions.None);
                if (CalculateCheckSum(newcomming).ToUpper() != gpsinfo[gpsinfo.Length - 1])
                {
                    Status = 97;
                    ret = false;
                    return ret;
                }
                
                switch (gpsinfo[1])
                {
                    case "GPRMC":
                        if (gpsinfo[3] == "A") //数据有效
                        {
                           
                            if (gpsinfo[10] != "")
                            {
                                int microSenconds = 0;
                                if (gpsinfo[2].Length > 7) //ms
                                    microSenconds = UInt16.Parse(gpsinfo[2].Substring(7));
                                UTCTime = new DateTime(UInt16.Parse(gpsinfo[10].Substring(4)) + 2000,
                                UInt16.Parse(gpsinfo[10].Substring(2, 2)),
                                UInt16.Parse(gpsinfo[10].Substring(0, 2)),
                                UInt16.Parse(gpsinfo[2].Substring(0, 2)),
                                UInt16.Parse(gpsinfo[2].Substring(2, 2)),
                                UInt16.Parse(gpsinfo[2].Substring(4, 2)), microSenconds);
                            }
                            if (gpsinfo[5] == "S")
                                Latitude =
                                    -(UInt16.Parse(gpsinfo[4].Substring(0, 2)) + float.Parse(gpsinfo[4].Substring(2))/60);
                            else
                            {
                                Latitude = UInt16.Parse(gpsinfo[4].Substring(0, 2)) +
                                           float.Parse(gpsinfo[4].Substring(2))/60;
                            }
                            if (gpsinfo[7] == "W")
                                Longitude =
                                    -(UInt16.Parse(gpsinfo[6].Substring(0, 2)) + float.Parse(gpsinfo[6].Substring(2)) / 60);
                            else
                            {
                                Longitude = UInt16.Parse(gpsinfo[6].Substring(0, 2)) +
                                           float.Parse(gpsinfo[6].Substring(2)) / 60;
                            }
                            //刷新航向、航速信息
                            Speed = float.Parse(gpsinfo[8]);
                            Heading = float.Parse(gpsinfo[9]);

                        }
                        else
                        {
                            Status = 98;
                        }
                        break;
                    case "GPGSV":
                        SatNum = UInt16.Parse(gpsinfo[4]);
                        break;
                    case "GPZDA":
                        if (gpsinfo[2] != "")
                        {
                            int microSenconds = 0;
                            if (gpsinfo[2].Length > 7) //ms
                                microSenconds = UInt16.Parse(gpsinfo[2].Substring(7));
                            UTCTime = new DateTime(UInt16.Parse(gpsinfo[5]),
                            UInt16.Parse(gpsinfo[4]),
                            UInt16.Parse(gpsinfo[3]),
                            UInt16.Parse(gpsinfo[2].Substring(0, 2)),
                            UInt16.Parse(gpsinfo[2].Substring(2, 2)),
                            UInt16.Parse(gpsinfo[2].Substring(4, 2)), microSenconds);
                        }
                        break;
                    case "GPGGA":                        
                        if (gpsinfo[7] == "1" || gpsinfo[7] == "2") //数据有效
                        {                           
                            if (gpsinfo[4] == "S")
                                Latitude =
                                    -(UInt16.Parse(gpsinfo[3].Substring(0, 2)) + float.Parse(gpsinfo[3].Substring(2)) / 60);
                            else
                            {
                                Latitude = UInt16.Parse(gpsinfo[3].Substring(0, 2)) +
                                           float.Parse(gpsinfo[3].Substring(2)) / 60;
                            }
                            if (gpsinfo[6] == "W")
                                Longitude =
                                    -(UInt16.Parse(gpsinfo[5].Substring(0, 3)) + float.Parse(gpsinfo[5].Substring(3)) / 60);
                            else
                            {
                                Longitude = UInt16.Parse(gpsinfo[5].Substring(0, 3)) +
                                           float.Parse(gpsinfo[5].Substring(3)) / 60;
                            }
                            Height = float.Parse(gpsinfo[12]);

                        }
                        else
                        {
                            Status = 98;
                        }
                        Status = uint.Parse(gpsinfo[7]);
                        break;
                    default:
                        break;

                }
            }
            catch
            {
                Status = 99;
                ret = false;
            }
            return ret;
        }

        #region checksum
        static ulong CRC32_POLYNOMIAL = 0xEDB88320L;
        /* --------------------------------------------------------------------------
        Calculate a CRC value to be used by CRC calculation functions.
        -------------------------------------------------------------------------- */
        private static UInt64 CRC32Value(int i)
        {
            int j;
            UInt64 ulCRC;
            ulCRC = (UInt64)i;
            for (j = 8; j > 0; j--)
            {

                if ((ulCRC & 1) == 1)
                    ulCRC = (ulCRC >> 1) ^ CRC32_POLYNOMIAL;
                else
                    ulCRC >>= 1;
            }
            return ulCRC;
        }
        /* --------------------------------------------------------------------------
        Calculates the CRC-32 of a block of data all at once
        -------------------------------------------------------------------------- */
        private static string CalculateBlockCRC32(string buffer)
        {
            ulong ulTemp1;
            ulong ulTemp2;
            ulong ulCRC = 0;
            int uCount = -1;
            char[] ucBuffer = new char[buffer.Length];
            ucBuffer = buffer.ToCharArray();
            while (uCount++ != buffer.Length - 1)
            {
                ulTemp1 = (ulCRC >> 8) & 0x00FFFFFFL;
                ulTemp2 = CRC32Value(((int)ulCRC ^ ucBuffer[uCount]) & 0xff);
                ulCRC = ulTemp1 ^ ulTemp2;
            }
            string CRC32 = ulCRC.ToString("x8");
            return CRC32;
        }

        private static string CalculateCheckSum(string buffer)
        {
            int startindex = buffer.IndexOf("$");
            int endindex = buffer.IndexOf("*");
            UInt16 uCheckSum = 0;
            string newstring = buffer.Substring(startindex + 1, endindex - startindex - 1);
            char[] ucBuffer = new char[newstring.Length];
            ucBuffer = newstring.ToCharArray();
            foreach (char c in ucBuffer)
            {
                uCheckSum ^= c;
            }
            return (uCheckSum.ToString("x2"));

        }
    #endregion
    }
}
