using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace PlaneS1
{
    public class FOSN
    {
        private static byte[] dat = new byte[32];
        public static bool Read(byte[] bytes)
        {
            if(bytes[0]!=0x99||bytes[1]!=0x66)
                return false;
            if(bytes[31]!=SumParity(bytes))
                return false;
            Buffer.BlockCopy(bytes,0,dat,0,32);
            return true;
        }

        public static byte SumParity(byte[] databytes)
        {
            byte ret = 0;
            UInt16 sum = 0;
            for (int i = 2; i < 30; i++)
            {
                sum += databytes[i];
            }
            ret = (byte)((sum <<8)>>8);
            return ret;
        }

        public static DateTime GetTime()
        {
            int year = BitConverter.ToInt16(dat, 3);
            int month = dat[5];
            int day = dat[6];
            int hour = (int)BitConverter.ToUInt32(dat,7)/10000/1000;
            int minute = (int)BitConverter.ToUInt32(dat, 7)/100/1000%100;
            int second = (int)BitConverter.ToUInt32(dat, 7)/1000%100;
            int milisecond = (int)BitConverter.ToUInt32(dat, 7) % 1000;
            return new DateTime(year, month, day, hour, minute, second, milisecond);
        }
        public static float GetHeading()
        {
            return (float)BitConverter.ToInt16(dat,17)/(float)180;
        }

        public static float GetPitch()
        {
            return (float)BitConverter.ToInt16(dat, 19) / (float)180;
        }

        public static float GetRoll()
        {
            return (float)BitConverter.ToInt16(dat, 15) / (float)180;

        }

        public static float GetHeave()
        {
            return (float)BitConverter.ToInt16(dat, 23) / (float)100;
        }
    }
}
