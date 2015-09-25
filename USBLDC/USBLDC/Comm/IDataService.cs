using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using USBLDC.Events;

namespace USBLDC.Comm
{
    public interface IUSBLSerialService
    {
        bool Init(SerialPort serialPort);
        void Register(Observer<DataEventArgs> observer);
        void UnRegister(Observer<DataEventArgs> observer);
        bool Start();
        bool Stop();
        void OnParsed(DataEventArgs eventArgs);
        SerialPort ReturnSerialPort();
    }

    public interface Observer<in T>
    {
        void Handle(Object sender, T e);
    }

    public interface IUDPService
    {
        bool Init(UdpClient udpClient);
        void Register(Observer<DataEventArgs> observer);
        void UnRegister(Observer<DataEventArgs> observer);
        bool Start();
        void Stop();

        UdpClient ReturnUdpClient();

        void OnParsed(DataEventArgs eventArgs);
    }

    public interface ITCPClientService
    {
        bool Init(TcpClient tcpClient,IPAddress ip,int destport);
        void Register(Observer<DataEventArgs> observer);
        void UnRegister(Observer<DataEventArgs> observer);
        void ConnectSync();
        bool Start();
        void Stop();
        //void RecvThread(object obj);
        //TcpClient ReturnTcpClient();
        //void OnParsed(CustomEventArgs eventArgs);

        bool Connected
        {
            get;

        }
    }
}
