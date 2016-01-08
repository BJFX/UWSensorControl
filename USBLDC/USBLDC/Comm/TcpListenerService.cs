using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;
namespace USBLDC.Comm
{
    public class TcpListenerService:ITCPServerService
    {
        public bool Init(TcpListener tcpListener, int listenport)
        {
            try
            {
                IPHostEntry localhost = Dns.GetHostEntry(hostname);
                IPAddress localaddr = localhost.AddressList[0];
                var end = new IPEndPoint(localaddr, listenport);
                tcpListener = new TcpListener(end);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Register(Observer<Events.DataEventArgs> observer)
        {
            throw new NotImplementedException();
        }

        public void UnRegister(Observer<Events.DataEventArgs> observer)
        {
            throw new NotImplementedException();
        }

        public bool Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public bool Connected
        {
            get { throw new NotImplementedException(); }
        }

        public TcpClient LinkerClient
        {
            get { throw new NotImplementedException(); }
        }
    }
}
