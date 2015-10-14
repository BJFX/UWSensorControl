using System;
using System.Net.Sockets;
using TinyMetroWpfLibrary.EventAggregation;
using USBLDC.Events;
using USBLDC.Structure;
using USBLDC.Core;
namespace USBLDC.Core
{
    public class USBLDataObserver : Comm.Observer<DataEventArgs>
    {
        StructureInterface gpsInfo = new GPSInfo();
        StructureInterface adInfo = new ADInfo();
        StructureInterface poseInfo = new PosetureInfo();
        StructureInterface RawposInfo = new RawPositionInfo();
        StructureInterface AjustposInfo = new AjustPositionInfo();
        StructureInterface scInfo = new SonarConfig();

        public void Handle(object sender, DataEventArgs e)
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
                case (int)TypeId.SONARCONFIG:
                    info = scInfo;
                    break;
                case (int)TypeId.AjustPos:
                    info = AjustposInfo;
                    break;
                default:
                    break;
            }
            if (info.Parse(e.Bytes))
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new ShowStructureInfo(info, e.Id));
            }
        }
    }
}
