using System;
using System.IO;
using System.Threading;
using TinyMetroWpfLibrary.EventAggregation;
using USBLDC.Comm;
using USBLDC.Events;
using USBLDC.Helpers;
using USBLDC.Structure;

namespace USBLDC.Core
{
    /// <summary>
    /// 核心业务类，包括通信服务，数据解析，服务状态及一些其他的系统变量
    /// </summary>
    public class UnitCore
    {
        private readonly static object SyncObject = new object();
        //静态接口，用于在程序域中任意位置操作UnitCore中的成员
        private static UnitCore _instance;
        //事件绑定接口，用于事件广播
        private IEventAggregator _eventAggregator;
        //网络服务接口
        private INetCore _iNetCore;
        //串口服务接口
        private ICommCore _iCommCore;
        //文件服务接口
        private IFileCore _iFileCore;
        private USBLTraceService _usblTraceService;
        //基础配置信息
        private BasicConf _appConf;
        private ProjectConf _projectConf;
        private SonarConfig _sonarConfig;
        private Comm.Observer<DataEventArgs> _observer; 
        private bool _serviceStarted = false;
        public string Error { get; private set; }

        public Mutex ACMMutex { get; set; }//全局解析锁


        public USBLTraceService USBLTraceService
        {
            get { return _usblTraceService ?? (_usblTraceService = new USBLTraceService()); }
        }

        public SonarConfig SonarConfiguration
        {
            get { return _sonarConfig ?? (_sonarConfig = new SonarConfig()); }
        }

        public static UnitCore GetInstance()
        {
            lock (SyncObject)
            {

                return _instance ?? (_instance = new UnitCore());
            }
        }

        protected UnitCore()
        {
            
            ACMMutex = new Mutex();

        }

        public bool UpdateSonarConfig()
        {
            string conf = BasicConf.MyExecPath + "\\" + "DefConf.dat";
            if (File.Exists(conf))
            {
                return SonarConfiguration.Parse(File.ReadAllBytes(conf));
            }
            else
            {
                var bw = new FileStream(conf,FileMode.OpenOrCreate);
                bw.Write(SonarConfiguration.SavePakage(), 0, SonarConfiguration.SavePakage().Length);
                bw.Close();
                return true;
            }
        }
        public bool LoadConfiguration()
        {
            bool ret = true;
            try
            {
                _appConf = BasicConf.GetInstance();
                ret = UpdateSonarConfig();
            }
            catch (Exception ex)
            {
                ret = false;
                
            }
            return ret;
        }


        public IEventAggregator EventAggregator
        {
            get { return _eventAggregator ?? (_eventAggregator = UnitKernal.Instance.EventAggregator); }
        }

        

        public INetCore NetCore
        {
            get { return _iNetCore ?? (_iNetCore = NetWorkService.GetInstance(_appConf, Observer)); }
        }

        public ICommCore CommCore
        {
            get { return _iCommCore ?? (_iCommCore = CommService.GetInstance(_appConf, Observer)); }
        }

        public bool Start()
        {
            try
            {
                LoadConfiguration();
                NetCore.Initialize();
                NetCore.Start();
                CommCore.Initialize();
                CommCore.Start();
                _serviceStarted = true;//if failed never get here
                
                return _serviceStarted;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                EventAggregator.PublishMessage(new LogEvent(ex.Message, ex, LogType.Error));
                return false;
            }
            
            
        }

        public void Stop()
        {
            if(NetCore.IsWorking)
                NetCore.Stop();
            if(CommCore.IsWorking)
                CommCore.Stop();
            _serviceStarted = false;
        }
        public bool IsWorking
        {
            get { return _serviceStarted; }
        }

        public BasicConf SystemConf
        {
            get { return _appConf; }
        }

        public ProjectConf CurrentProjectConf
        {
            get { return _projectConf; }
        }
        public Observer<DataEventArgs> Observer
        {
            get { return _observer ?? (_observer = new USBLDataObserver()); }

        }

        public IFileCore IFileCore
        {
            get { return _iFileCore; }
            set { _iFileCore = value; }
        }

        public static UnitCore Instance
        {
            get { return GetInstance(); }
        }
    }
}
