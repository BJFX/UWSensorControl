using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using USBLDC.Core;
using USBLDC.Events;

namespace USBLDC.Comm
{
    public class TcpListenerService:ITCPServerService
    {
        public TcpListener _tcpListener;
        protected IPAddress IP;
        public static event EventHandler<DataEventArgs> DoParse;
        protected Thread TdThread;
        public bool Init(int listenport)
        {
            try
            {
                IPHostEntry localhost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress localaddr = localhost.AddressList[0];
                var end = new IPEndPoint(localaddr, listenport);
                _tcpListener = new TcpListener(end);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Register(Observer<Events.DataEventArgs> observer)
        {
            DoParse -= observer.Handle;
            DoParse += observer.Handle;
        }

        public void UnRegister(Observer<Events.DataEventArgs> observer)
        {
            DoParse -= observer.Handle;
        }

        public bool Start()
        {
            _tcpListener.Start();
            TdThread = new Thread(RecvThread);
            TdThread.Start(this);
            return true;
        }

        private void RecvThread(object obj)
        {
            TcpListenerService server = null;
            server = obj as TcpListenerService;
            if (server!=null)
            {
                var myReadBuffer = new byte[4100];
                while (true)
                {
                    try
                    {
                        server.LinkerClient = server._tcpListener.AcceptTcpClient();
                        NetworkStream stream = server.LinkerClient.GetStream();
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
                                int n = stream.Read(myReadBuffer, 32 + numberOfBytesRead,
                                    (int) (packetLength - numberOfBytesRead));
                                numberOfBytesRead += n;

                            } while (numberOfBytesRead != packetLength);
                            var e = new DataEventArgs(id, null, myReadBuffer);
                            OnParsed(e);
                        }
                        server.LinkerClient = null;
                    }
                    catch (Exception exception)
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(exception,
                            LogType.Both));
                    }
                    finally
                    {
                        if (server.LinkerClient != null)
                            server.LinkerClient.Close();
                    }
                }

            }

        }

        public void Stop()
        {
            if (LinkerClient != null)
                LinkerClient.Close();
            if (_tcpListener != null)
                _tcpListener.Stop();
            if (TdThread != null)
            {
                if (TdThread.IsAlive)
                {
                    TdThread.Abort();
                }
            }

        }
        public void OnParsed(DataEventArgs eventArgs)
        {
            if (DoParse != null)
            {
                DoParse(this, eventArgs);
            }
        }
        public bool Connected { get; set; }

        public TcpClient LinkerClient { get; set; }

    }
}
