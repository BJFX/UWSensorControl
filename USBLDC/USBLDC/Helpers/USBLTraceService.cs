using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using USBLDC.Core;

namespace USBLDC.Helpers
{
    public class USBLTraceService
    {
        public string Error { get; set; }
        public bool StartService()
        {
            var splash = new SplashWindow();
            splash.ShowDialog();
            if (TraceFile.Instance.Start(splash.NameBox.Text, splash.PathBox.Text))
            {
                Error = string.Empty;
                return true;
            }
           Error = TraceFile.Instance.Errormsg;
            return false;
        }

        public void Stop()
        {
            TraceFile.Instance.Close();
        }
    }
}
