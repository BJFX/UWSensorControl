using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USBLDC.Structure
{
    public class PosetureInfo : StructureInterface
    {
        public uint Status { get; set; }
        public uint EpochSecond { get; set; }
        public uint EpochMicSecond { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Heading { get; set; }
        public float Heave { get; set; }
        public uint[] Reserved = new uint[10];
        public bool Parse(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    
        public byte[] SavePakage()
        {
            byte[] bytes = new byte[64];
            int offset = 0;
            
            Buffer.BlockCopy(BitConverter.GetBytes(EpochSecond), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(EpochMicSecond), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Pitch), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Roll), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Heading), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Heave), 0, bytes, offset, 4);
            
            return bytes;
        }
    }
}
