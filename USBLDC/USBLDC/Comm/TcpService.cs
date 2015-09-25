using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using USBLDC.Core;
using USBLDC.Events;
using System.Threading;
using System.Diagnostics;

namespace USBLDC.Comm
{
    public abstract class TCPBaseService : ITCPClientService
    {
        protected TcpClient _tcpClient;
        protected IPAddress IP;
        protected int port = 8080;
        public static event EventHandler<DataEventArgs> DoParse;
        protected Thread TdThread;
        public TCPBaseService()
        {

        }


        public bool Init(TcpClient tcpClient, IPAddress ip, int destport)
        {
            if (tcpClient != null)
            {
                _tcpClient = tcpClient;
                IP = ip;
                port = destport;
                return true;
            }
            return false;

        }

        public void Register(Observer<DataEventArgs> observer)
        {
            DoParse -= observer.Handle;
            DoParse += observer.Handle;
        }
        public void UnRegister(Observer<DataEventArgs> observer)
        {
            DoParse -= observer.Handle;
        }
        public void ConnectSync()
        {
            if (_tcpClient == null) return;
            try
            {
                // _tcpClient.Connect(IP, port);
                var result = _tcpClient.BeginConnect(IP, port, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                if (!_tcpClient.Connected)
                {
                    throw new Exception("连接失败");
                }
                // we have connected
                _tcpClient.EndConnect(result);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }

        }

        public bool Start()
        {
            if (!_tcpClient.Connected) return false;
            TdThread = new Thread(RecvThread);
            TdThread.Start(_tcpClient);
            return true;
        }

        public abstract void RecvThread(object obj);

        public void Stop()
        {
            if (_tcpClient != null)
                _tcpClient.Close();
            if (TdThread != null)
            {
                if (TdThread.IsAlive)
                {
                    TdThread.Abort();

                }
            }

        }

        public TcpClient ReturnTcpClient()
        {
            return _tcpClient;
        }

        public void OnParsed(DataEventArgs eventArgs)
        {
            if (DoParse != null)
            {
                DoParse(this, eventArgs);
            }
        }

        public bool Connected { get { return ReturnTcpClient().Connected; } }
    }
    public class TcpService : TCPBaseService
    {
        public override void RecvThread(object o)
        {
            TcpClient client = null;
            try
            {
                using (client = o as TcpClient)
                {
                    var myReadBuffer = new byte[4100];
                    var stream = client.GetStream();
                    while (stream.CanRead)
                    {
                        Array.Clear(myReadBuffer, 0, 4100); //置零
                        int numberOfBytesRead = 0;
                        int id = BitConverter.ToInt32(myReadBuffer, 0);
                        stream.Read(myReadBuffer, 0, 32); //先读包头
                        var packetLength = BitConverter.ToUInt32(myReadBuffer, 8);
                        // Incoming message may be larger than the buffer size.
                        do
                        {
                            int n = stream.Read(myReadBuffer, 32 + numberOfBytesRead, (int)(packetLength - numberOfBytesRead));
                            numberOfBytesRead += n;

                        } while (numberOfBytesRead != packetLength);
                        var e = new DataEventArgs(id,null,myReadBuffer);
                        OnParsed(e);
                    }
                }
            }
            catch (Exception exception)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent(exception.Message, exception,
                    LogType.Error));
            }
            finally
            {
                if (client != null)
                    client.Close();
            }
        }
    }
}
