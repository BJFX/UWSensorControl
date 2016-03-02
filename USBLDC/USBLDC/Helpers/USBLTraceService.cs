using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using USBLDC.Core;
using USBLDC.Structure;

namespace USBLDC.Helpers
{
    public class USBLTraceService
    {
        public string Error { get; set; }
        public bool bCanceld = false;
        public bool StartService()
        {
            bCanceld = false;
            var splash = new SplashWindow();
            if (splash.ShowDialog() == false)
            {
                bCanceld = true;
                return false;
            }
            if (TraceFile.Instance.Start(splash.NameBox.Text, splash.PathBox.Text))
            {
                Error = string.Empty;
                return true;
            }
           Error = TraceFile.Instance.Errormsg;
            return false;
        }

        public void SaveAD(ADInfo adInfo)
        {
            TraceFile.Instance.SaveAD(adInfo);
        }
        public void SaveGPS(GPSInfo gpsInfo)
        {
            TraceFile.Instance.SaveGPS(gpsInfo);
        }
        public void SavePosition(AjustPositionInfo ajInfo)
        {
            TraceFile.Instance.SavePosition(ajInfo);
        }
        public void SavePose(PosetureInfo poseinfo)
        {
            TraceFile.Instance.SavePose(poseinfo);
        }
        public void SaveSonarSetting(SonarConfig scConfig)
        {
            TraceFile.Instance.SaveSonarSetting(scConfig);
        }

        public void Stop()
        {
            TraceFile.Instance.Close();
        }
    }
}
