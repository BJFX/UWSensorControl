using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using USBLDC.Core;

namespace USBLDC.Comm
{
    public class NerWorkServer : INetCore
    {

        public bool SonarIsOK
        {
            get { throw new NotImplementedException(); }
        }

        public bool PoseIsOK
        {
            get { throw new NotImplementedException(); }
        }

        public bool SendCMD(byte[] buf)
        {
            throw new NotImplementedException();
        }

        public Observer<Events.DataEventArgs> NetDataObserver
        {
            get { throw new NotImplementedException(); }
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public bool IsWorking
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsInitialize
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
