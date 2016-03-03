using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;
using USBLDC.Core;
using USBLDC.Events;

namespace USBLDC.Comm
{
    public class NerWorkServer : INetCore
    {
        private readonly static object SyncObject = new object();
        private static INetCore _netInstance;
        private ITCPServerService _tcpPoseService;
        private ITCPServerService _tcpDataService;
        private ITCPServerService _tcpCmdService;
        private TcpListener _poseListener;
        private TcpListener _dataListener;
        private TcpListener _cmdListenert;
        private BasicConf _conf;
        private Observer<DataEventArgs> _dataObserver;
        private string _error;
        public static INetCore GetInstance(BasicConf conf, Observer<DataEventArgs> observer)
        {
            lock (SyncObject)
            {
                if (conf != null)
                    return _netInstance ?? (_netInstance = new NerWorkServer(conf, observer));
                else
                {
                    return null;
                }
            }
        }
        protected NerWorkServer(BasicConf conf, Observer<DataEventArgs> observe)
        {
            _conf = conf;
            _dataObserver = observe;

        }
        public ITCPServerService TCPDataService
        {
            get { return _tcpDataService ?? (_tcpDataService = new USBLListenerService()); }
        }
        public ITCPServerService TCPCmdService
        {
            get { return _tcpCmdService ?? (_tcpCmdService = new USBLListenerService()); }
        }
        public ITCPServerService TCPPoseService
        {
            get { return _tcpPoseService ?? (_tcpPoseService = new PoseListenerService()); }
        }
        public bool SonarIsOK { get; set; }

        public bool PoseIsOK { get; set; }


        public bool SendCMD(byte[] buf)
        {
            if (SonarIsOK && TCPCmdService.LinkerClient!=null)
            {
                var cmd = new TcpCommand(TCPCmdService.LinkerClient, buf);
                return cmd.Send(out _error);
            }
            return false;
        }

        public Observer<Events.DataEventArgs> NetDataObserver
        {
            get { return _dataObserver; }
        }

        public void Initialize()
        {
            if (IsInitialize)
            {
                throw new Exception("已初始化");
            }
            IsInitialize = true;
            try
            {
                if (!TCPDataService.Init(int.Parse(_conf.GetNetPort())))
                    throw new Exception("数据端口初始化失败");
                if (!TCPCmdService.Init(int.Parse(_conf.GetNetCmdPort())))
                    throw new Exception("命令端口初始化失败");
            }
            catch (Exception)
            {
                IsInitialize = false;
            }
            try
            {

                if (!TCPPoseService.Init(int.Parse(_conf.GetPosePort())))
                    throw new Exception("姿态传感器端口初始化失败");
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
            IsWorking = CreateTCPListening();
        }

        private bool CreateTCPListening()
        {
            try
            {
                if (!SonarIsOK)
                {
                    TCPDataService.Start();
                    TCPDataService.Register(NetDataObserver);
                    TCPCmdService.Start();
                    TCPCmdService.Register(NetDataObserver);
                    SonarIsOK = true;
                }
                if (!PoseIsOK)
                {
                    TCPPoseService.Start();
                    TCPPoseService.Register(NetDataObserver);
                    PoseIsOK = true;
                }
                return PoseIsOK && SonarIsOK;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return false;
            }
        }

        public bool IsWorking { get; set; }
        

        public bool IsInitialize { get;set ;}

        public string Error
        {
            get { return _error; }
            set { _error = value; }
        }


        public bool SonarIsLink
        {
            get { return (TCPDataService.LinkerClient != null); }
        }

        public bool PoseIsLink
        {
            get { return (TCPPoseService.LinkerClient != null); }
        }
    }
}
