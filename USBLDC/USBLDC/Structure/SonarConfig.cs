using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace USBLDC.Structure
{
    public class SonarConfig:StructureInterface
    {
        public uint Id { get; set; }
        public uint Index { get; set; }
        public uint Length { get; set; }
        public uint[] HeadReserved = new uint[5];
        public uint Cmd{ get; set;}
        public uint VelCmd{ get; set;}
        public float SurVel{ get; set;}
        public float AvgVel{ get; set;}
        public float FixedGain{ get; set;}
        public uint TVGCmd{ get; set;}
        public float FixedTVG{ get; set;}
        public float TVGSampling{ get; set;}
        public uint TVGSamples{ get; set;}
        public float TVGA1{ get; set;}
        public float TVGA2{ get; set;}
        public float TVGA3{ get; set;}
        public float PingPeriod{ get; set;}
        public uint ADSaved{ get; set;}
        public uint PoseSaved{ get; set;}
        public uint PosSaved{ get; set;}
        public float SonarDepth{ get; set;}
        public float SonarGPSx{ get; set;}
        public float SonarGPSy{ get; set;}
        public float SonarGPSz{ get; set;}
        public float Pitchfixed { get; set; }
        public float Rollfixed { get; set; }
        public float Headingfixed { get; set; }

        public uint[] ReservedInfo =new uint[41];
        public SonarConfig()
        {
            Id = 0x0001;
            Index = 0;
            Length = 288;
            Cmd = 0;
            VelCmd = 0;
            SurVel = 1500;
            AvgVel = 1500;
            FixedGain = 20;
            TVGCmd = 1;
            FixedTVG = 20;
            TVGSampling = 0;
            TVGSamples = 0;
            TVGA1 = 0;
            TVGA2 = 0;
            TVGA3 = 0;
            PingPeriod = 12;
            ADSaved = 1;
            PoseSaved = 1;
            PosSaved = 1;
            SonarDepth = 10;
            SonarGPSx = 10;
            SonarGPSy = 10;
            SonarGPSz = 10;
            Pitchfixed = 0;
            Rollfixed = 0;
            Headingfixed = 0;
        }
        public bool Parse(byte[] bytes)
        {
            var length = ReadHead(bytes);
            if (length < 1 || Id != 0x0001)
            {
                Id = 0x0001;
                return false;
            }
            int offset = 32;//head length
            Cmd = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            VelCmd = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            SurVel = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            AvgVel = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            FixedGain = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            TVGCmd = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            FixedTVG = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            TVGSampling = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            TVGSamples = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            TVGA1 = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            TVGA2 = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            TVGA3 = BitConverter.ToSingle(bytes,offset);
            offset += 4;
            PingPeriod = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            ADSaved = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            PoseSaved = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            PosSaved = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            SonarDepth = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            SonarGPSx = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            SonarGPSy = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            SonarGPSz = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            Pitchfixed = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            Rollfixed = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            Headingfixed = BitConverter.ToSingle(bytes, offset);
            return true;
        }

        private uint ReadHead(byte[] bytes)
        {
            Id = BitConverter.ToUInt32(bytes, 0);
            Index = BitConverter.ToUInt32(bytes, 4);
            Length = BitConverter.ToUInt32(bytes, 8);
            return Length;
        }

        public byte[] SavePackage()//带head
        {
            byte[] bytes = new byte[288];
            int offset = 0;
            Buffer.BlockCopy(BitConverter.GetBytes(Id),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Index),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Length),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(HeadReserved,0,bytes,offset,20);
            offset += 20;
            Buffer.BlockCopy(BitConverter.GetBytes(Cmd),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(VelCmd),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(SurVel),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(AvgVel),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(FixedGain),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(TVGCmd),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(FixedTVG),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(TVGSampling),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(TVGSamples),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(TVGA1),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(TVGA2),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(TVGA3),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(PingPeriod),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(ADSaved),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(PoseSaved),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(PosSaved),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(SonarDepth),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(SonarGPSx),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(SonarGPSy),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(SonarGPSz),0,bytes,offset,4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Pitchfixed), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Rollfixed), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Headingfixed), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(ReservedInfo, 0, bytes, offset, 164);
            return bytes;
        }
    }
}
