using System;

namespace USBLDC.Helpers
{
    /// <summary>
    ///   Gps类，处理Gps数据，自定义对象
    /// </summary>
    class GpsHelper
    {
        static ulong CRC32_POLYNOMIAL = 0xEDB88320L;
        /* --------------------------------------------------------------------------
        Calculate a CRC value to be used by CRC calculation functions.
        -------------------------------------------------------------------------- */
        public static UInt64 CRC32Value(int i)
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
        public static string CalculateBlockCRC32(string buffer)
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

        public static string CalculateCheckSum(string buffer)
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
    }
}
