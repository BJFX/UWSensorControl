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
        public void Handle(object sender, DataEventArgs e)
        {
            StructureInterface Info;
            switch (e.Id)
            {
                case (int)TypeId.GPS:
                    Info = new GPSInfo();
                    if (Info.Parse(e.Bytes))
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new ShowGPSInfo(Info));
                    }
                    break;
                case (int)TypeId.ADINFO:
                    Info = new ADInfo();
                    if (Info.Parse(e.Bytes))
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new ShowGPSInfo(Info));
                    }
                    break;
                case (int)TypeId.Pose:
                    Info = new PosetureInfo();
                    if (Info.Parse(e.Bytes))
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new ShowGPSInfo(Info));
                    }
                    break;
                case (int)TypeId.RawPos:
                    Info = new RawPositionInfo();
                    if (Info.Parse(e.Bytes))
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new ShowGPSInfo(Info));
                    }
                    break;
                case (int)TypeId.SONARCONFIG:
                    Info = new SonarConfig();
                    if (Info.Parse(e.Bytes))
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new ShowGPSInfo(Info));
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
