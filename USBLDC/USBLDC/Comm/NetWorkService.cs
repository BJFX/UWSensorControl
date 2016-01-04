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
        private ITCPClientService _tcpCmdService;
        private TcpClient _posetcpClient;
        private TcpClient _datatcpClient;
        private TcpClient _cmdtcpClient;
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
        public ITCPClientService TCPCmdService
        {
            get { return _tcpCmdService ?? (_tcpCmdService = new TcpService()); }
        }
        public ITCPClientService TCPPoseService
        {
            get { return _tcpPoseService ?? (_tcpPoseService = new TcpService()); }
        }
        private bool CreateTCPService(BasicConf conf)
        {
            // 同步方法，会阻塞进程，调用init用task
            if (!SonarIsOK)
            {
                TCPDataService.ConnectSync();
                TCPDataService.Register(NetDataObserver);
                TCPCmdService.ConnectSync();
                TCPCmdService.Register(NetDataObserver);
                
            }
            if (!PoseIsOK)
            {
                TCPPoseService.ConnectSync();
                TCPPoseService.Register(NetDataObserver);
                
            }
            if (SonarIsOK && PoseIsOK)
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
            _cmdtcpClient = new TcpClient { SendTimeout = 1000 };
            _posetcpClient = new TcpClient {SendTimeout = 1000};
            IsInitialize = true;
            try
            {
                if (!TCPDataService.Init(_datatcpClient, IPAddress.Parse(_conf.GetIP()), int.Parse(_conf.GetNetPort())))
                    throw new Exception("数据端口初始化失败");
                if (!TCPCmdService.Init(_cmdtcpClient, IPAddress.Parse(_conf.GetIP()), int.Parse(_conf.GetNetCmdPort())))
                    throw new Exception("命令端口初始化失败");
            }
            catch (Exception)
            {
                IsInitialize = false;
            }
            try
            {
                
                if (!TCPPoseService.Init(_posetcpClient, IPAddress.Parse(_conf.GetPoseIP()), int.Parse(_conf.GetPosePort())))
                    throw new Exception("姿态传感器网络初始化失败");
            }
            catch (Exception)
            {
                IsInitialize = false;
            }
            
        }

        public void Stop()
        {
            if (IsInitialize)
            {
                TCPDataService.UnRegister(NetDataObserver);
                TCPDataService.Stop();
                TCPCmdService.Stop();
                TCPPoseService.UnRegister(NetDataObserver);
                TCPPoseService.Stop();
            }
            IsWorking = false;
            IsInitialize = false;
        }

        public void Start()
        {
            IsWorking = false;
            IsWorking = CreateTCPService(_conf);

        }

        public bool IsWorking { get; set; }

        public bool IsInitialize{ get; set; }
        
        public string Error{ get; set; }






        public bool SonarIsOK
        {
            get
            {
                return (TCPDataService.Connected && TCPCmdService.Connected);
            }
        }


        public bool PoseIsOK
        {
            get
            {
                return TCPPoseService.Connected;
            }
        }
        
    }
}
