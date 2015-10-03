using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using USBLDC.Structure;

namespace USBLDC.Core
{
    public class TraceFile
    {
        private readonly static object SyncObject = new object();
        public static string  ProjectName { get; set; }
        public static string Path { get; set; }
        public static string  Errormsg { get; set; }
        public static LogFile GpsFile { get; set; }
        public static LogFile PoseFile { get; set; }
        public static PingFile ADFile { get; set; }
        public static PingFile PosFile { get; set; }
        public static FileStream SonarSetting { get; set; }
        public static int GPSFileSize { get; set; }
        public static int PoseFileSize { get; set; }
        //静态接口
        private static TraceFile _instance;

        protected TraceFile()
        {

                GpsFile = null;
                PoseFile = null;
                ADFile = null;
                PosFile = null;
                ProjectName = null;
                Path = null;
                GPSFileSize = 1024*256;
                PoseFileSize = 1024*256;
        }
        public static TraceFile Instance
        {
            get
            {
                lock (SyncObject)
                {
                    return _instance ?? (_instance = new TraceFile());
                    
                }
            }
        }

        public static void CloseALL()
        {
            if (GpsFile != null)
                GpsFile.Close();
            if (PoseFile != null)
                PoseFile.Close();
            if(ADFile!=null)
                ADFile.Close();
            if (PosFile != null)
                PosFile.Close();
            if (SonarSetting != null)
                SonarSetting.Close();
        }
        public static void SetProject(string name)
        {
            ProjectName = name;
            
        }

        public static void SetPath(string path)
        {
            Path = path + "\\" + ProjectName;
            try
            {
                File.Create(Path);
            }
            catch (Exception)
            {

                throw new Exception("创建文件夹失败！");
            }
        }
        public static void SaveGPS(string str)
        {
            if (GpsFile == null)
            {
                GpsFile = new LogFile("GPS","dat");
                GpsFile.SetFileSizeLimit(GPSFileSize);
                GpsFile.SetPath(new DirectoryInfo(Path));
                GpsFile.Create();

            }
            if(GpsFile.Write(str)==0)
                throw new Exception("创建GPS文件失败！");
        }
        public static void SavePose(string str)
        {
            if (PoseFile == null)
            {
                PoseFile = new LogFile("Pose", "dat");
                PoseFile.SetFileSizeLimit(PoseFileSize);
                PoseFile.SetPath(new DirectoryInfo(Path));
                PoseFile.Create();

            }
            if (PoseFile.Write(str) == 0)
                throw new Exception("创建Pose文件失败！");
        }
        public static void SaveAD(ADInfo info)
        {
            if (ADFile == null)
            {
                ADFile = new PingFile(ProjectName,"dat");
                ADFile.SetPath(new DirectoryInfo(Path));
                ADFile.Create();
            }
            if (ADFile.PingID == 0) //还未写入数据
            {
                ADFile.Write(info.SavePakage());
            }
            else if (ADFile.PingID != info.PingNum) //新的ping号
            {
                ADFile.Close();
                ADFile.Create();
                ADFile.Write(info.SavePakage());
            }
            else
            {
                ADFile.Write(info.SavePakage());
            }
        }

        public static void SaveSonarSetting(SonarConfig scConfig)
        {
            if (SonarSetting == null)
            {
                var  MyExecPath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
                MyExecPath += "SonarConfig";
                SonarSetting = new FileStream(MyExecPath,FileMode.OpenOrCreate);
            }
            SonarSetting.Write(scConfig.SavePakage(), 0, 288);
        }
    }
}
