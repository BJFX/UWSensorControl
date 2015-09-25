using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyMetroWpfLibrary.Utility;
namespace USBLDC.Core
{
    public class BasicConf
    {
        private static readonly object SyncObject = new object();
        private static BasicConf _basicConf;

        //配置文件
        private string xmldoc = "BasicConf.xml"; //const

        public static BasicConf GetInstance()
        {
            lock (SyncObject)
            {
                return _basicConf ?? (_basicConf = new BasicConf());
            }
        }

        protected BasicConf()
        {
            string MyExecPath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            xmldoc = MyExecPath + "\\" + xmldoc;

        }

        protected string GetValue(string[] str)
        {
            return XmlHelper.GetConfigValue(xmldoc, str);
        }

        public string GetIP()
        {
            string[] str = {"USBL", "Net", "IP"};
            return GetValue(str);
        }

        public string GetNetPort()
        {
            string[] str = {"USBL", "Net", "DataPort"};
            return GetValue(str);
        }

        public string GetCommPort()
        {
            string[] str = {"USBL", "Comm", "ComPort"};
            return GetValue(str);
        }

        public string GetCommDataRate()
        {
            string[] str = {"USBL", "Comm", "DataRate"};
            return GetValue(str);
        }

    }
}
