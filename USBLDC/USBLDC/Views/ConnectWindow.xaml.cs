using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using USBLDC.Core;
using USBLDC.Events;
using USBLDC.ViewModel;

namespace USBLDC.Views
{
    /// <summary>
    /// SplashWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectWindow 
    {
        private DispatcherTimer t = null;
        List<string> RateInfo = new List<string>();
        List<string> CommInfo = new List<string>();
        private int exit = -1;//-1：非正常退出，0:正常工作，1：退出程序，2：修改配置
        public ConnectWindow()
        {
            InitializeComponent();
            
            RateInfo.Add("4800");
            RateInfo.Add("9600");
            RateInfo.Add("19200");
            RateInfo.Add("38400");
            RateInfo.Add("115200");
            foreach (var rate in RateInfo)
            {
                GpsRateBox.Items.Add(rate);
            }
            
        }

        private void ModifyPara_Click(object sender, RoutedEventArgs e)
        {
            exit = 2;
            this.Close();
            UnitCore.Instance.EventAggregator.PublishMessage(new GoSonarConfigEvent(false));
        }

        private void ModifyNet_Click(object sender, RoutedEventArgs e)
        {
            SetPopUpControl.IsOpen = true;
        }

        private void StartWork_Click(object sender, RoutedEventArgs e)
        {
            exit = 0;
            this.Close();
            UnitCore.Instance.EventAggregator.PublishMessage(new GoHomePageNavigationEvent());
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            exit = 1;
            this.Close();
            MainFrameViewModel.pMainFrame.ExitProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CommInfo != null)
                CommInfo.Clear();
            CommInfo = SerialPort.GetPortNames().ToList();
            IpAddrBox.Text = BasicConf.GetInstance().GetIP();
            IpPortBox.Text = BasicConf.GetInstance().GetNetPort();
            IpCmdPortBox.Text =BasicConf.GetInstance().GetNetCmdPort();
            IpPoseAddrBox.Text = BasicConf.GetInstance().GetPoseIP();
            IpPosePortBox.Text = BasicConf.GetInstance().GetPosePort();
            GpsBox.SelectedIndex = CommInfo.IndexOf(BasicConf.GetInstance().GetCommGPS());
            GpsRateBox.SelectedIndex = RateInfo.IndexOf(BasicConf.GetInstance().GetGPSDataRate());
            t = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Background, StartConnect,Dispatcher.CurrentDispatcher);
            t.Start();
        }

        private void StartConnect(object sender, EventArgs e)
        {
            if(UnitCore.Instance==null||UnitCore.Instance.NetCore==null||UnitCore.Instance.CommCore==null)
                return;
            
            if (UnitCore.Instance.NetCore.SonarIsOK)
            {
                SonarConnectStatus.Visibility = Visibility.Collapsed;
                SonarBar.Visibility = Visibility.Hidden;
                TCPOK.Visibility = Visibility.Visible;
                StartWork.Visibility = Visibility.Visible;
            }
            else
            {
                SonarConnectStatus.Visibility = Visibility.Visible;
                SonarConnectStatus.Text = "连接失败";
                SonarBar.Visibility = Visibility.Collapsed;
                TCPOK.Visibility = Visibility.Collapsed;
            }
            if (UnitCore.Instance.NetCore.PoseIsOK)
            {
                POSEConnectStatus.Visibility = Visibility.Collapsed;
                PoserBar.Visibility = Visibility.Hidden;
                POSEOK.Visibility = Visibility.Visible;
            }
            else
            {
                POSEConnectStatus.Visibility = Visibility.Visible;
                POSEConnectStatus.Text = "连接失败";
                POSEOK.Visibility = Visibility.Collapsed;
                PoserBar.Visibility = Visibility.Collapsed;
            }
            
            if(UnitCore.Instance.CommCore.IsWorking)
            {
                GPSConnectStatus.Visibility = Visibility.Collapsed;
                GPSBar.Visibility = Visibility.Hidden;
                GPSConnectStatus.Visibility = Visibility.Visible;
            }
            else
            {
                GPSConnectStatus.Visibility = Visibility.Visible;
                GPSConnectStatus.Text = "端口打开失败";
                CommOK.Visibility = Visibility.Collapsed;
                GPSBar.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveConnBtn_Click(object sender, RoutedEventArgs e)
        {
            string ipaddr;
            int dataport,cmdport;
            string poseaddr;
            int poseport;
            IPAddress tempAddress;
            if (IPAddress.TryParse(IpAddrBox.Text, out tempAddress) == false)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("声纳地址格式不正确！", LogType.OnlyInfo));
                return;
            }
            else
            {
                ipaddr = IpAddrBox.Text;
            }
            if (int.TryParse(IpPortBox.Text, out dataport) && (dataport > 0 || dataport < 65535))
            {

            }
            else
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("数据端口格式不正确！", LogType.OnlyInfo));
                return;
            }
            if (int.TryParse(IpCmdPortBox.Text, out cmdport) && (cmdport > 0 || cmdport < 65535))
            {

            }
            else
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("命令端口格式不正确！", LogType.OnlyInfo));
                return;
            }
            if (IPAddress.TryParse(IpPoseAddrBox.Text, out tempAddress) == false)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("运动传感器地址格式不正确！", LogType.OnlyInfo));
                return;
            }
            else
            {
                poseaddr = IpPoseAddrBox.Text;
            }
            if (int.TryParse(IpPosePortBox.Text, out poseport) && (poseport > 0 || poseport < 65535))
            {

            }
            else
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("传感器端口格式不正确！", LogType.OnlyInfo));
                return;
            }
            try
            {
                if (!BasicConf.GetInstance().SetIP(ipaddr)) throw new Exception("保存IP地址失败");
                if (!BasicConf.GetInstance().SetNetPort(dataport.ToString())) throw new Exception("保存数据端口失败");
                if (!BasicConf.GetInstance().SetCmdPort(cmdport.ToString())) throw new Exception("保存命令端口失败");
                if (!BasicConf.GetInstance().SetPoseIP(poseaddr)) throw new Exception("保存姿态传感器IP地址失败");
                if (!BasicConf.GetInstance().SetPosePort(poseport.ToString())) throw new Exception("保存姿态传感器IP端口失败");
                if (GpsBox.SelectedIndex!=-1)
                    if (!BasicConf.GetInstance().SetCommGPS(CommInfo[GpsBox.SelectedIndex])) throw new Exception("保存串口端口失败");
                if (!BasicConf.GetInstance().SetGPSDataRate(RateInfo[GpsRateBox.SelectedIndex])) throw new Exception("保存串口速率失败");
            }
            catch (Exception exception)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent(exception.Message, LogType.Both));
                return;
            }
            UnitCore.Instance.Stop();
            SetPopUpControl.IsOpen = false;
            SonarBar.Visibility = Visibility.Visible;
            PoserBar.Visibility = Visibility.Visible;
            GPSBar.Visibility = Visibility.Visible;
            SonarConnectStatus.Visibility = Visibility.Collapsed;
            POSEConnectStatus.Visibility = Visibility.Collapsed;
            GPSConnectStatus.Visibility = Visibility.Collapsed;
            StartWork.Visibility = Visibility.Hidden;
            UnitCore.Instance.Start();
            Task.Factory.StartNew(() =>
            {
                UnitCore.Instance.NetCore.Initialize();
                UnitCore.Instance.NetCore.Start();
                UnitCore.Instance.CommCore.Initialize();
                UnitCore.Instance.CommCore.Start();
            });
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SetPopUpControl.IsOpen = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (exit == -1)
            {
                UnitCore.Instance.Stop();
                MainFrameViewModel.pMainFrame.ExitProgram();
            }
                
        }



    }

}
