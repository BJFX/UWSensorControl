using System;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using TinyMetroWpfLibrary.Utility;
using USBLDC.Core;
using USBLDC.Events;

namespace USBLDC.Comm
{
    public class CommService:ICommCore
    {
        private readonly static object SyncObject = new object();
        private static ICommCore _CommInstance;
        private IUSBLSerialService _serialService;
        private BasicConf _commConf;
        private SerialPort _serialPort;
        private Observer<DataEventArgs> _DataObserver;
        public string Error { get; set; }
        public bool IsInitialize { get; set; }
        public bool IsWorking { get; set; }
        public static ICommCore GetInstance(BasicConf conf, Observer<DataEventArgs> observer)
        {
            lock (SyncObject)
            {

                return _CommInstance ?? (_CommInstance = new CommService(conf, observer));
            }
        }
        protected CommService(BasicConf conf, Observer<DataEventArgs> observer)
        {
            _commConf = conf;
            _DataObserver = observer;
        }
        public IUSBLSerialService SerialService
        {
            get { return _serialService ?? (_serialService = (new USBLSerialService())); }
        }

        public Observer<DataEventArgs> CommDataObserver
        {
            get { return _DataObserver; }
        }

        public void Initialize()
        {
            if (IsInitialize)
            {
                throw new Exception("服务已初始化");
            }
            //可以加入其他的初始化工作 
            IsInitialize = true;
        }

        /// <summary>
        ///  初始化串口数据服务
        /// </summary>
        /// <param name="configure">通信参数</param>
        /// <returns>成功or失败</returns>
        private bool CreateSerialService(BasicConf configure)
        {
            _serialPort = new SerialPort(configure.GetCommGPS(),int.Parse(configure.GetGPSDataRate()));
            if (!SerialService.Init(_serialPort) || !SerialService.Start()) return false;
            SerialService.Register(CommDataObserver);
            return true;
        }
        public void Stop()
        {
            if (IsInitialize)
            {
                SerialService.UnRegister(CommDataObserver);
                SerialService.Stop();
            }
            IsInitialize = false;
        }

        public void Start()
        {
            IsWorking = false;
            if (_commConf == null || _DataObserver == null)
                throw new Exception("无法设置内部端口");
            if (!CreateSerialService(_commConf)) throw new Exception("命令服务无法初始化");
            IsWorking = true;
        }

    }
}
