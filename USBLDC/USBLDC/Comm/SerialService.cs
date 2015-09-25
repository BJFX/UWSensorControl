using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using USBLDC.Events;
using USBLDC.Structure;
namespace USBLDC.Comm
{
    public abstract class SerialSerialServiceBase :IUSBLSerialService
    {
        #region 属性

        protected static SerialPort _serialPort;

        public static event EventHandler<DataEventArgs> DoParse;
        private List<byte> _recvQueue = new List<byte>();
        
        #endregion

        #region 方法
        public bool Init(SerialPort serialPort)
        {
            try
            {
                _recvQueue.Clear();
                _serialPort = serialPort;
                if (SerialPort.GetPortNames().All(t => t != _serialPort.PortName.ToUpper()))
                {
                   return false;
                }
                if (!_serialPort.IsOpen) _serialPort.Open();
                return _serialPort.IsOpen;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            
        }

        public virtual void Register(Observer<DataEventArgs> observer)
        {
            DoParse -= observer.Handle;
            DoParse+=observer.Handle;
        }



        public virtual void UnRegister(Observer<DataEventArgs> observer)
        {
            DoParse -= observer.Handle;
        }

        public bool Stop()
        {
            _serialPort.DataReceived -= _SerialPort_DataReceived;
            try
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
            }
            catch (Exception)
            {
                
                return false;
            }
            return true;
        }

        public virtual bool Start()
        {
            _serialPort.DataReceived -= _SerialPort_DataReceived;
            _serialPort.DataReceived += _SerialPort_DataReceived;
            return _serialPort.IsOpen;
        }

        public virtual SerialPort ReturnSerialPort()
        {
            return _serialPort;
        }
        private void _SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var nCount = _serialPort.BytesToRead;
            if ( nCount < 16)
            {
                Thread.Sleep(50);
                return;
            }
            for (int i = nCount - 1; i >= 0; i--)
            {
                _recvQueue.Add((byte)_serialPort.ReadByte());
                CheckQueue(ref _recvQueue);
            }
            
        }

        protected abstract void CheckQueue(ref List<byte> lstBytes);

        public void OnParsed(DataEventArgs eventArgs)
        {
            if (DoParse != null)
            {
                DoParse(this, eventArgs);
            }
        }
        #endregion
    }
    public class USBLSerialService: SerialSerialServiceBase
    {
        
        
        protected override void CheckQueue(ref List<byte> queue)
        {
            var bytes = new byte[queue.Count];
            queue.CopyTo(bytes);
            var strcmd = Encoding.ASCII.GetString(bytes);
            var arg = new DataEventArgs((int) TypeId.GPS, strcmd, bytes);
            OnParsed(arg);
        }

    }
}
