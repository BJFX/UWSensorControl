using System;
using System.Collections.Generic;
using System.IO;
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
        public static string MyExecPath;
        public static BasicConf GetInstance()
        {
            lock (SyncObject)
            {
                return _basicConf ?? (_basicConf = new BasicConf());
            }
        }

        protected BasicConf()
        {
            MyExecPath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            xmldoc = MyExecPath + "\\" + xmldoc;

        }

        protected string GetValue(string[] str)
        {
            return XmlHelper.GetConfigValue(xmldoc, str);
        }
        protected bool SetValue(string[] str,string value)
        {
            return XmlHelper.SetConfigValue(xmldoc, str,value);
        }
        public string GetIP()
        {
            string[] str = {"Net", "IP"};
            return GetValue(str);
        }
        public bool SetIP(string newip)
        {
            string[] str = {"Net", "IP" };
            return SetValue(str, newip);
        }
        public string GetNetPort()
        {
            string[] str = {"Net", "DataPort"};
            return GetValue(str);
        }
        public bool SetNetPort(string newport)
        {
            string[] str = {"Net", "DataPort" };
            return SetValue(str, newport);
        }
        public string GetPoseIP()
        {
            string[] str = {"Net", "PoseIP" };
            return GetValue(str);
        }
        public bool SetPoseIP(string newPoseIp)
        {
            string[] str = {"Net", "PoseIP" };
            return SetValue(str, newPoseIp);
        }
        public string GetPosePort()
        {
            string[] str = {"Net", "PosePort" };
            return GetValue(str);
        }
        public bool SetPosePort(string newport)
        {
            string[] str = {"Net", "PosePort" };
            return SetValue(str, newport);
        }
        public string GetCommGPS()
        {
            string[] str = {"GPS", "ComPort"};
            return GetValue(str);
        }
        public bool SetCommGPS(string newgpscomm)
        {
            string[] str = {"GPS", "ComPort" };
            return SetValue(str, newgpscomm);
        }
        public string GetGPSDataRate()
        {
            string[] str = {"GPS", "DataRate" };
            return GetValue(str);
        }
        public bool SetGPSDataRate(string datarate)
        {
            string[] str = {"GPS", "DataRate" };
            return SetValue(str, datarate);
        }
        /// <summary>
        /// model的相对路径
        /// </summary>
        /// <returns></returns>
        public string GetModelPath(string name)
        {
            string[] str = { "Model", name };
            return GetValue(str);
        }
    }
}
