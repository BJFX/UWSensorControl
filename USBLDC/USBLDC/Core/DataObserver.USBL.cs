using System;
using System.Net.Sockets;
using TinyMetroWpfLibrary.EventAggregation;
using USBL.GuiWei;
using USBLDC.Events;
using USBLDC.Helpers;
using USBLDC.Structure;
using USBLDC.Core;
namespace USBLDC.Core
{
    public class USBLDataObserver : Comm.Observer<DataEventArgs>
    {
        private float longitude, latitude=0;
        const int maxDepth = 13000;//最大深度
        int[] SVPd = new int[maxDepth + 31];
        double[] SVPc = new double[maxDepth + 31];
        StructureInterface gpsInfo = new GPSInfo();
        StructureInterface adInfo = new ADInfo();
        StructureInterface poseInfo = new PosetureInfo();
        StructureInterface RawposInfo = new RawPositionInfo();
        public void Handle(object sender, DataEventArgs e)
        {
            try
            {
                StructureInterface info = null;
                switch (e.Id)
                {
                    case (int)TypeId.GPS:
                        info = gpsInfo;
                        break;
                    case (int)TypeId.ADINFO:
                        info = adInfo;
                        break;
                    case (int)TypeId.Pose:
                        info = poseInfo;
                        break;
                    case (int)TypeId.RawPos:
                        info = RawposInfo;
                        break;
                    default:
                        break;
                }
                if (info.Parse(e.Bytes))
                {
                    if (e.Id == (int) TypeId.GPS)
                    {
                        longitude = ((GPSInfo) info).Long;
                        latitude = ((GPSInfo)info).Lat;    
                    }
                    if (e.Id == (int) TypeId.RawPos)
                    {
                        AjustPositionInfo ajInfo = new AjustPositionInfo();
                        ajInfo.CalcPosition(UnitCore.Instance.SoundFile, UnitCore.Instance.SonarConfiguration,
                            (RawPositionInfo) info,longitude, latitude);
                        info = (StructureInterface)ajInfo;
                    }
                    UnitCore.Instance.EventAggregator.PublishMessage(new ShowStructureInfo(info, e.Id));
                    SaveRecvData(info, e.Id);
                }
            }
            catch (Exception ex)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(ex, LogType.Both));
            }
            
        }

        private void SaveRecvData(StructureInterface info, int p)
        {
            var ut = UnitCore.Instance.USBLTraceService;
            switch (p)
            {
                case (int)TypeId.GPS:
                    var gps = info as GPSInfo;
                    ut.SaveGPS(gps);
                    break;
                case (int)TypeId.ADINFO:
                    var ad = info as ADInfo;
                    ut.SaveAD(ad);
                    break;
                case (int)TypeId.Pose:
                    var pose = info as PosetureInfo;
                    ut.SavePose(pose);
                    break;
                case (int)TypeId.AjustPos:
                    var adj = info as AjustPositionInfo;
                    ut.SavePosition(adj);
                    break;
                default:
                    break;
            }
        }


    }
}
