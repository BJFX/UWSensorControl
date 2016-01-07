using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyMetroWpfLibrary.Controller;
using USBLDC.Comm;
using USBLDC.Events;

namespace USBLDC.Core
{
    public interface IMessageController
    {
        void Init();
        //发送信息到系统记录
        void SendMessage(string message);
        //将info写入文件
        void WriteLog(string message);
        //将错误信息写入文件
        void ErrorLog(string message, Exception ex);
        //发送消息到界面
        void Alert(string message, Exception ex = null);
        //通过UDP调试端口广播信息，用于调试
        void BroadCast(string message);
    }
    public interface ICore
    {
        void Initialize();
        void Stop();
        void Start();
        bool IsWorking { get; set; }
        bool IsInitialize { get; set; }
        string Error { get; set; }
    }

    public interface INetCore : ICore
    {

        //TCP客户端接收数据服务
        ITCPClientService TCPDataService { get; }
        bool SonarIsOK { get; }
        bool PoseIsOK { get; }
        bool SendCMD(byte[] buf);

        /// <summary>
        /// 数据观察类，主要负责数据的解析和保存
        /// </summary>
        Observer<DataEventArgs> NetDataObserver { get; }

    }
    public interface ICommCore : ICore
    {

        //串口数据接收服务
        IUSBLSerialService SerialService { get; }
        
        /// <summary>
        /// 数据观察类，主要负责数据的解析和保存
        /// </summary>
        Observer<DataEventArgs> CommDataObserver { get; }

    }
    public interface IFileCore : ICore
    {

        //文件服务
        //TBD
        /// <summary>
        /// 数据观察类，主要负责数据的解析和保存
        /// </summary>
        Observer<DataEventArgs> FileDataObserver { get; }

    }
    public interface IUnitKernel : IKernel
    {
        IMessageController MessageController { get; }

    }
}
