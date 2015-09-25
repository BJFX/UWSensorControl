using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USBLDC.Events
{
    public class DataEventArgs : EventArgs
    {
        public int Id { get; set; }
        public string info { get; set; }
        public byte[] Bytes;
        public DataEventArgs(int id, string outstring, byte[] buf)
        {
            Id = id;
            info = outstring;
            Bytes = buf;
        }
    }
}
