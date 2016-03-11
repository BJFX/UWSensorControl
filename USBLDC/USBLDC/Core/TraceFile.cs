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
        public  string  ProjectName { get; set; }
        public  string Path { get; set; }
        public  string  Errormsg { get; set; }
        public ADFile GpsFile { get; set; }
        public ADFile PoseFile { get; set; }
        public  PingFile ADFile { get; set; }
        public  PingFile PosFile { get; set; }
        public  FileStream SonarSetting { get; set; }
        public  int GPSFileSize { get; set; }
        public  int PoseFileSize { get; set; }
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

        public bool Start(string name,string path)
        {
            try
            {
                SetProject(name);
                SetPath(path);
                return true;
            }
            catch (Exception exception)
            {
                Errormsg = exception.Message;
                return false;
            }
            
        }
        public  void Close()
        {
            Path = null;
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
        public  void SetProject(string name)
        {
            ProjectName = name;
            
        }

        public  void SetPath(string path)
        {
            Path = path + "\\" + ProjectName;
            try
            {
                Directory.CreateDirectory(Path);
            }
            catch (Exception)
            {

                throw new Exception("创建文件夹失败！");
            }
        }
        public  void SaveGPS(GPSInfo gpsInfo)
        {
            if (Path==null)
                return;
            if (GpsFile == null)
            {
                GpsFile = new ADFile("GPS","dat");
                GpsFile.SetFileSizeLimit(GPSFileSize);
                GpsFile.SetPath(new DirectoryInfo(Path));
                GpsFile.Create();

            }
            if (GpsFile.Write(gpsInfo.SavePackage()) == 0)
                throw new Exception("创建GPS文件失败！");
        }
        public void SavePose(PosetureInfo poseinfo)
        {
            if (Path == null)
                return;
            if (PoseFile == null)
            {
                PoseFile = new ADFile("Pose", "dat");
                PoseFile.SetFileSizeLimit(PoseFileSize);
                PoseFile.SetPath(new DirectoryInfo(Path));
                PoseFile.Create();

            }
            if (PoseFile.Write(poseinfo.SavePackage()) == 0)
                throw new Exception("创建Pose文件失败！");
        }
        public  void SaveAD(ADInfo info)
        {
            if (Path == null)
                return;
            if (ADFile == null)
            {
                ADFile = new PingFile(ProjectName,"ad");
                ADFile.SetPath(new DirectoryInfo(Path));
                ADFile.Create();
                ADFile.Write(info.SavePackage());
                return;
            }
            if (ADFile.PingID != info.PingNum) //新的ping号
            {
                ADFile.Close();
                ADFile.Create();
                
            }
            ADFile.Write(info.SavePackage());

        }
        public void SavePosition(AjustPositionInfo info)
        {
            if (Path == null)
                return;
            if (PosFile == null)
            {
                PosFile = new PingFile(ProjectName, "pos");
                PosFile.SetPath(new DirectoryInfo(Path));
                PosFile.Create();
                PosFile.Write(info.SavePackage());
                return;
            }
            if (PosFile.PingID != info.raw.PingNum) //新的ping号
            {
                PosFile.Close();
                PosFile.Create();
            }
            PosFile.Write(info.SavePackage());

        }
        public  void SaveSonarSetting(SonarConfig scConfig)
        {
            if (SonarSetting == null)
            {
                var  MyExecPath = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
                MyExecPath += "SonarConfig";
                SonarSetting = new FileStream(MyExecPath,FileMode.OpenOrCreate);
            }
            SonarSetting.Write(scConfig.SavePackage(), 0, 288);
        }
    }
}
