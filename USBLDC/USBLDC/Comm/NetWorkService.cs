using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using USBLDC.Core;
using USBLDC.Events;

namespace USBLDC.Comm
{
    public class NetWorkService : INetCore
    {
        private readonly static object SyncObject = new object();
        private static INetCore _netInstance;
        private ITCPClientService _tcpPoseService;
        private ITCPClientService _tcpDataService;

        private TcpClient _posetcpClient;
        private TcpClient _datatcpClient;
        private BasicConf _conf;
        private Observer<DataEventArgs> _dataObserver;

        public static INetCore GetInstance(BasicConf conf, Observer<DataEventArgs> observer)
        {
            lock (SyncObject)
            {
                if (conf != null)
                    return _netInstance ?? (_netInstance = new NetWorkService(conf, observer));
                else
                {
                    return null;
                }
            }
        }

        protected NetWorkService(BasicConf conf, Observer<DataEventArgs> observe)
        {
            _conf = conf;
            _dataObserver = observe;
        }
        public ITCPClientService TCPDataService
        {
            get { return _tcpDataService ?? (_tcpDataService = new TcpService()); }
        }
        public ITCPClientService TCPPoseService
        {
            get { return _tcpPoseService ?? (_tcpPoseService = new TcpService()); }
        }
        private bool CreateTCPService(BasicConf conf)
        {
            // 同步方法，会阻塞进程，调用init用task
            TCPDataService.ConnectSync();
            TCPPoseService.ConnectSync();
            TCPDataService.Register(NetDataObserver);
            TCPPoseService.Register(NetDataObserver);
            if (TCPDataService.Connected && TCPDataService.Start() && TCPPoseService.Connected && TCPPoseService.Start())
                return true;
            return false;
        }
  

        public Task<bool> SendCMD(byte[] buf)
        {
            throw new NotImplementedException();
        }




        public Observer<DataEventArgs> NetDataObserver
        {
            get { return _dataObserver; }
        }

        public void Initialize()
        {
            if (IsInitialize)
            {
                throw new Exception("已初始化");
            }
            _datatcpClient = new TcpClient { SendTimeout = 1000 };
            _posetcpClient = new TcpClient {SendTimeout = 1000};
            try
            {
                if (!TCPDataService.Init(_datatcpClient, IPAddress.Parse(_conf.GetIP()), int.Parse(_conf.GetNetPort())))
                    throw new Exception("通信网络初始化失败");
                if (!TCPPoseService.Init(_posetcpClient, IPAddress.Parse(_conf.GetPoseIP()), int.Parse(_conf.GetPosePort())))
                    throw new Exception("姿态传感器网络初始化失败");
            }
            catch (Exception)
            {
                throw new Exception("网络配置初始化失败");
            }

            IsInitialize = true;
        }

        public void Stop()
        {
            if (IsInitialize)
            {
                TCPDataService.UnRegister(NetDataObserver);
                TCPDataService.Stop();
                TCPPoseService.UnRegister(NetDataObserver);
                TCPPoseService.Stop();
            }
            IsWorking = false;
            IsInitialize = false;
        }

        public void Start()
        {
            IsWorking = false;
            if (_conf == null || _dataObserver == null)
                throw new Exception("无法设置网络通信");
            if (!CreateTCPService(_conf)) throw new Exception("通信服务无法启动");
            IsWorking = true;
        }

        public bool IsWorking { get; set; }

        public bool IsInitialize{ get; set; }
        
        public string Error{ get; set; }



      
    }
}
