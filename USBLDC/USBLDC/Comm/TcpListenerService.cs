using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using USBLDC.Core;
using USBLDC.Events;
using USBLDC.Structure;

namespace USBLDC.Comm
{
    public abstract class BaseListenerService : ITCPServerService
    {
        public TcpListener _tcpListener;
        protected IPAddress IP;
        public static event EventHandler<DataEventArgs> DoParse;
        protected Thread TdThread;        

        public bool Init(int listenport)
        {
            try
            {
                //IPHostEntry localhost = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress localaddr = localhost.AddressList[0];
                var end = new IPEndPoint(IPAddress.Any, listenport);
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

        public abstract void RecvThread(object obj);

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
    public class PoseListenerService : BaseListenerService
    {
        private CCheckBP check = new CCheckBP();
        private List<byte> _recvQueue = new List<byte>();
        public override void RecvThread(object obj)
        {
            PoseListenerService server = null;
            server = obj as PoseListenerService;
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
                            while (stream.DataAvailable)
                            {
                                stream.Read(myReadBuffer, 0, 1);
                                _recvQueue.Add(myReadBuffer[0]);
                            }
                            CheckQueue(ref _recvQueue);
                        }
                        //server.LinkerClient = null;                         

                    }
                    catch (Exception exception)
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(exception,
                            LogType.OnlyLog));
                    }
                    finally
                    {
                        if (server.LinkerClient != null)
                            server.LinkerClient.Close();
                        server.LinkerClient = null;
                    }
                }

            }

        }

        protected void CheckQueue(ref List<byte> queue)
        {
            var bytes = new byte[queue.Count];
            queue.CopyTo(bytes);
            byte[] ch = new byte[4096];
            //对串口接收的数据进行解析
            if (queue.Count != 0)
            {
                check.WriteData(bytes, (uint)queue.Count);//写入循环缓冲区，取完整帧和校验
                while (check.IsFull())//取出所有完整帧
                {
                    //校验在PalnS1处理
                    uint lenth = 0;
                    check.GetFullData(ch, ref lenth);//得到完整帧以及帧的长度
                    byte[] DataBuffer = new byte[lenth];
                    int id = (int)TypeId.Pose; //与网络包格式一致
                    Buffer.BlockCopy(ch, 0, DataBuffer, 0, (int)lenth);
                    var e = new DataEventArgs(id, null, DataBuffer);
                    OnParsed(e);
                    //break;//只取出一组完整的帧
                }
                queue.Clear();

            }
        }
    }

    public class USBLListenerService : BaseListenerService
    {
        public override void RecvThread(object obj)
        {
            USBLListenerService server = null;
            server = obj as USBLListenerService;
            if (server != null)
            {
                var myReadBuffer = new byte[32768];
                while (true)
                {
                    try
                    {
                        server.LinkerClient = server._tcpListener.AcceptTcpClient();
                        NetworkStream stream = server.LinkerClient.GetStream();
                        while (stream.CanRead)
                        {
                            Array.Clear(myReadBuffer, 0, 32768); //置零
                            int numberOfBytesRead = 0;
                            
                            stream.Read(myReadBuffer, 0, 32); //先读包头
                            var packetLength = BitConverter.ToUInt32(myReadBuffer, 8)-32;
                            // Incoming message may be larger than the buffer size.
                            do
                            {
                                int n = stream.Read(myReadBuffer, 32 + numberOfBytesRead,
                                    (int)(packetLength - numberOfBytesRead));
                                numberOfBytesRead += n;

                            } while (numberOfBytesRead != packetLength);
                            int id = BitConverter.ToInt32(myReadBuffer, 0);
                            var e = new DataEventArgs(id, null, myReadBuffer);
                            OnParsed(e);
                        }
                    }
                    catch (Exception exception)
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(exception,
                            LogType.OnlyLog));
                    }
                    finally
                    {
                        if (server.LinkerClient != null)
                            server.LinkerClient.Close();
                        server.LinkerClient = null;
                    }
                }

            }

        }
    }

    public class USBLCmdListenerService : BaseListenerService
    {
        public override void RecvThread(object obj)
        {
            USBLCmdListenerService server = null;
            server = obj as USBLCmdListenerService;
            if (server != null)
            {
                var myReadBuffer = new byte[32768];
                while (true)
                {
                    try
                    {
                        server.LinkerClient = server._tcpListener.AcceptTcpClient();
                        NetworkStream stream = server.LinkerClient.GetStream();
                        while (stream.CanRead)
                        {
                            Array.Clear(myReadBuffer, 0, 32768); //置零
                            int numberOfBytesRead = 0;

                            stream.Read(myReadBuffer, 0, 32); //先读包头
                            var packetLength = BitConverter.ToUInt32(myReadBuffer, 8) - 32;
                            // Incoming message may be larger than the buffer size.
                            do
                            {
                                int n = stream.Read(myReadBuffer, 32 + numberOfBytesRead,
                                    (int)(packetLength - numberOfBytesRead));
                                numberOfBytesRead += n;

                            } while (numberOfBytesRead != packetLength);
                            int id = BitConverter.ToInt32(myReadBuffer, 0);
                            var e = new DataEventArgs(id, null, myReadBuffer);
                            OnParsed(e);
                        }
                    }
                    catch (Exception exception)
                    {
                        UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(exception,
                            LogType.OnlyLog));
                    }
                    finally
                    {
                        if (server.LinkerClient != null)
                            server.LinkerClient.Close();
                        server.LinkerClient = null;
                    }
                }

            }

        }
    }
}
