using System;
using System.IO;
using System.Threading;
using TinyMetroWpfLibrary.EventAggregation;
using USBLDC.Comm;
using USBLDC.Events;
using USBLDC.Helpers;
using USBLDC.Structure;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using System.Threading.Tasks;
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
        public SettleSoundFile SoundFile = null;
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
            CurrentModel = null;
            ACMMutex = new Mutex();

        }
        /// <summary>
        /// 更新声纳配置，true从文件中读出，false更新到文件
        /// </summary>
        /// <param name="bRead"></param>
        /// <returns></returns>
        public bool UpdateSonarConfig(bool bRead=true,string configfile=null)
        {

            string conf = configfile;
            if (conf == null)
                conf = BasicConf.MyExecPath + "\\" + "DefConf.dat";

            if (bRead)
            {
                 return SonarConfiguration.Parse(File.ReadAllBytes(conf));
            }
            else
            {
                try
                {
                    var bw = new FileStream(conf, FileMode.OpenOrCreate);
                    bw.Write(SonarConfiguration.SavePackage(), 0, SonarConfiguration.SavePackage().Length);
                    bw.Close();
                    return true;
                }
                catch (Exception)
                {
                    
                    return false;
                }
                
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

        public async Task<bool> Start()
        {
            try
            {
                //USBLTraceService.StartService();
                LoadConfiguration();
                var shippath = _appConf.GetModelPath("Ship");
                if (shippath == null)
                    throw new Exception("未找到模型组件！");
                shippath = BasicConf.MyExecPath+"\\"+shippath;//found
                CurrentModel = await LoadAsync(shippath, false);
                if (CurrentModel==null)
                    throw new Exception("加载模型组件失败！");
                //this part replace to the connection splash
                //NetCore.Initialize();
                //NetCore.Start();
                //CommCore.Initialize();
                //CommCore.Start();
                _serviceStarted = true;//if failed never get here
                
                return _serviceStarted;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                EventAggregator.PublishMessage(new ErrorEvent(ex, LogType.Both));
                return false;
            }
            
            
        }

        public void Stop()
        {
            NetCore.Stop();
            CommCore.Stop();
            USBLTraceService.Stop();
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
        public Model3D CurrentModel { get; set; }
        public static UnitCore Instance
        {
            get { return GetInstance(); }
        }
        private async Task<Model3DGroup> LoadAsync(string model3DPath, bool freeze)
        {
            return await Task.Factory.StartNew(() =>
            {
                var mi = new ModelImporter();

                    // Alt 1. - freeze the model 
                    return mi.Load(model3DPath, null, true);

            });
        }
    }
}
