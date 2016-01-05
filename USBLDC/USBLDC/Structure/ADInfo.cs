using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USBLDC.Structure
{
    public class ADInfo : StructureInterface
    {
        public uint Id { get; set; }
        public uint Index { get; set; }
        public uint Length { get; set; }
        public uint[] HeadReserved = new uint[5];
        public uint PingNum { get; set; }
        public uint PingSecond { get; set; }
        public uint PingMicSecond { get; set; }
        public uint PkgIndex { get; set; }
        public uint Samples { get; set; }
        public uint channels { get; set; }
        public uint[] Reserved1 = new uint[2];
        public uint EpochSecond { get; set; }
        public uint EpochMicSecond { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Heading { get; set; }
        public float Heave { get; set; }
        public uint[] Reserved2 = new uint[2];
        public short[] AdRawData = new short[256*36];
        public ADInfo()
        {
            Id = 0x0004;
        }
        public bool Parse(byte[] bytes)
        {
            var length = ReadHead(bytes);
            if (length < 1 || Id != 0x0004)
            {
                Id = 0x0004;
                return false;
            }
            int offset = 32;//head length
            PingNum = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            PingSecond = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            PingMicSecond = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            PkgIndex = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            Samples = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            channels = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            offset += 8;
            EpochSecond = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            EpochMicSecond = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            Pitch = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            Roll = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            Heading = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            Heave = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            offset += 8;
            Buffer.BlockCopy(bytes,offset,AdRawData,0,256*36*2);
            return true;
        }

        private uint ReadHead(byte[] bytes)
        {
            Id = BitConverter.ToUInt32(bytes, 0);
            Index = BitConverter.ToUInt32(bytes, 4);
            Length = BitConverter.ToUInt32(bytes, 8);
            return Length;
        }
        public byte[] SavePackage()//没有包头
        {
            byte[] bytes = new byte[18480];
            int offset = 0;
            Buffer.BlockCopy(BitConverter.GetBytes(PkgIndex), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Samples), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(channels), 0, bytes, offset, 4);
            offset += 4;
            offset += 4;//reserved
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
            offset += 4;
            offset += 8;//reserved
            Buffer.BlockCopy(AdRawData, 0, bytes, offset, 256*36*2);
            return bytes;
        }
    }
}
