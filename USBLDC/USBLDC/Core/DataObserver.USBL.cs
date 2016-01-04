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
        const int maxDepth = 13000;//最大深度
        int[] SVPd = new int[maxDepth + 31];
        double[] SVPc = new double[maxDepth + 31];
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
                default:
                    break;
            }
            if (info.Parse(e.Bytes))
            {
                var sc = UnitCore.Instance.SonarConfiguration;
                uint velcmd = sc.VelCmd;
                if ((velcmd & 0x11) == 0)
                {

                }
                else if ((velcmd & 0x11) == 1)
                {
                    
                }
                UnitCore.Instance.EventAggregator.PublishMessage(new ShowStructureInfo(info, e.Id));
            }
        }
    }
}
