using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USBL.GuiWei;
using USBLDC.Helpers;

namespace USBLDC.Structure
{
    public class RawPositionInfo : StructureInterface
    {
        public uint Id { get; set; }
        public uint Index { get; set; }
        public uint Length { get; set; }
        public uint[] HeadReserved = new uint[5];
        public uint PingNum { get; set; }
        public uint PingSecond { get; set; }
        public uint PingMicSecond { get; set; }
        public uint Status { get; set; }
        public float TravelTime { get; set; }
        public float AvgVel { get; set; }
        public float SurVel { get; set; }
        public float XAngle { get; set; }
        public float YAngle { get; set; }
        public float Distance { get; set; }
        public float XDistance { get; set; }
        public float YDistance { get; set; }
        public float ZDistance { get; set; }
        public float BeamingAngle1 { get; set; }
        public float BeamingAngle2 { get; set; }
        public uint EpochSecond { get; set; }
        public uint EpochMicSecond { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Heading { get; set; }
        public float Heave { get; set; }
        public float Amp { get; set; }
        public uint Quality { get; set; }
        public float Noise { get; set; }
        public uint[] Reserved = new uint[40];
        public RawPositionInfo()
        {
            Id = 0x0007;
        }
        public bool Parse(byte[] bytes)
        {
            var length = ReadHead(bytes);
            if (length < 1 || Id != 0x0007)
            {
                Id = 0x0007;
                return false;
            }
            int offset = 32;//head length
            PingNum = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            PingSecond = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            PingMicSecond = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            Status = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            TravelTime = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            AvgVel = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            SurVel = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            XAngle = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            YAngle = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            Distance = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            XDistance = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            YDistance = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            ZDistance = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            BeamingAngle1 = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            BeamingAngle2 = BitConverter.ToSingle(bytes, offset);
            offset += 4;
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
            Amp = BitConverter.ToSingle(bytes, offset);
            offset += 4;
            Quality = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            Noise = BitConverter.ToSingle(bytes, offset);
            return true;

        }
        private uint ReadHead(byte[] bytes)
        {
            Id = BitConverter.ToUInt32(bytes, 0);
            Index = BitConverter.ToUInt32(bytes, 4);
            Length = BitConverter.ToUInt32(bytes, 8);
            return Length;
        }
        public byte[] SavePackage()//归位前的定位数据打包，无包头
        {
            byte[] bytes = new byte[256];
            Array.Clear(bytes,0,256);
            int offset = 0;
            Buffer.BlockCopy(BitConverter.GetBytes(Id), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Index), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Length), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(HeadReserved, 0, bytes, offset, 20);
            offset += 20;
            Buffer.BlockCopy(BitConverter.GetBytes(PingNum), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(PingSecond), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(PingMicSecond), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Status), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(TravelTime), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(AvgVel), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(SurVel), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(XAngle), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(YAngle), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Distance), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(XDistance), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(YDistance), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(ZDistance), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(BeamingAngle1), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(BeamingAngle2), 0, bytes, offset, 4);
            offset += 4;
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
            Buffer.BlockCopy(BitConverter.GetBytes(Amp), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Quality), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Noise), 0, bytes, offset, 4);
            return bytes;
        }
    }
    public class AjustPositionInfo : StructureInterface
    {
        public uint Status { get; set; }
        public float XAjust { get; set; }
        public float YAjust { get; set; }
        public float ZAjust { get; set; }
        public float AjustLong { get; set; }
        public float AjustLat { get; set; }
        public float Noise { get; set; }
        public SonarConfig scConfig { get; set; }
        public RawPositionInfo raw { get; set; }

        public AjustPositionInfo 
        {
            scConfig = new SonarConfig();
            raw = new RawPositionInfo();
        }
        public bool Parse(byte[] bytes)
        {
            try
            {
                int offset = 0;
                raw.PingNum = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                raw.PingSecond = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                raw.PingMicSecond = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                scConfig.VelCmd = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                scConfig.SurVel = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                scConfig.AvgVel = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                scConfig.FixedTVG = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                scConfig.TVGSampling = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                scConfig.TVGSamples = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                scConfig.TVGA1 = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                scConfig.TVGA2 = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                scConfig.TVGA3 = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                scConfig.PingPeriod = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                scConfig.ADSaved = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                scConfig.PoseSaved = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                scConfig.PosSaved = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                scConfig.SonarGPSx = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                scConfig.SonarGPSy = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                scConfig.SonarGPSz = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                XAjust = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                YAjust = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                ZAjust = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                AjustLong = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                AjustLat = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                raw.BeamingAngle1 = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                raw.BeamingAngle2 = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                raw.EpochSecond = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                raw.EpochMicSecond = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                raw.Pitch = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                raw.Roll = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                raw.Heading = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                raw.Heave = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                raw.Amp = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                raw.Quality = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                raw.Noise = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                raw.TravelTime = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                raw.XAngle = BitConverter.ToSingle(bytes, offset);
                offset += 4;
                raw.YAngle = BitConverter.ToSingle(bytes, offset);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool CalcPosition(SettleSoundFile settleSoundFile, SonarConfig sc, RawPositionInfo raw, float longitude, float latitude)
        {
            if (settleSoundFile != null && sc != null && raw!=null)
            {
                try
                {
                    scConfig = sc;
                    this.raw = raw;
                    USBL_GuiWei Position_Guiwei = new USBL_GuiWei(raw.XDistance, raw.YDistance, raw.ZDistance, raw.Heave, raw.Heading, raw.Pitch, raw.Roll, longitude, latitude, sc.SonarGPSx, sc.SonarGPSy, sc.SonarGPSz, raw.TravelTime, sc.SonarDepth, settleSoundFile.SVPd, settleSoundFile.SVPc);
                    //USBL_GuiWei Position_Guiwei = new USBL_GuiWei((float)0.4785, (float)-0.5465, (float)-6.7128, raw.Heave, (float)48.7943, (float)0.1944, 0, longitude, latitude, sc.SonarGPSx, sc.SonarGPSy, sc.SonarGPSz, (float)0.0045, sc.SonarDepth, settleSoundFile.SVPd, settleSoundFile.SVPc);
                    //USBL_GuiWei Position_Guiwei = new USBL_GuiWei((float)0.6391, (float)-0.0549, (float)-6.9063, raw.Heave, (float)48.73, (float)0.19, 0, longitude, latitude, sc.SonarGPSx, sc.SonarGPSy, sc.SonarGPSz, (float)0.0046, sc.SonarDepth, settleSoundFile.SVPd, settleSoundFile.SVPc);
                    AjustLat = (float)Position_Guiwei.LatTarget;
                    AjustLong = (float)Position_Guiwei.LonTarget;
                    Noise = raw.Noise;
                    Status = raw.Status;
                    XAjust = (float)Position_Guiwei.x_local;
                    YAjust = (float)Position_Guiwei.y_local;
                    ZAjust = (float)Position_Guiwei.z_local;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        public byte[] SavePackage()
        {
            byte[] bytes = new byte[256];
            Array.Clear(bytes, 0, 256);
            int offset = 0;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.PingNum), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.PingSecond), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.PingMicSecond), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.VelCmd), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.SurVel), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.AvgVel), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.FixedTVG), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.TVGSampling), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.TVGSamples), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.TVGA1), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.TVGA2), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.TVGA3), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.PingPeriod), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.ADSaved), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.PoseSaved), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.PosSaved), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.SonarGPSx), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.SonarGPSy), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(scConfig.SonarGPSz), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(XAjust), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(YAjust), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(ZAjust), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(AjustLong), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(AjustLat), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.BeamingAngle1), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.BeamingAngle2), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.EpochSecond), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.EpochMicSecond), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.Pitch), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.Roll), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.Heading), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.Heave), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.Amp), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.Quality), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.Noise), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.TravelTime), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.XAngle), 0, bytes, offset, 4);
            offset += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(raw.YAngle), 0, bytes, offset, 4);
            return bytes;
        }
    }


}
