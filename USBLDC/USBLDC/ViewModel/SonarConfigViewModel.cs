using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.ViewModel;
using USBLDC.Core;
using USBLDC.Events;
using USBLDC.Structure;

namespace USBLDC.ViewModel
{
    public class SonarConfigViewModel:ViewModelBase
    {
        public override void Initialize()
        {
            RateInfo = new List<string>();
            RateInfo.Add("4800");
            RateInfo.Add("9600");
            RateInfo.Add("19200");
            RateInfo.Add("38400");
            RateInfo.Add("115200");
            SaveCommand = RegisterCommand(ExecutesaveCommand, CanExecutesaveCommand, true);
            SurVelSrcIndex = 0;
            AvgVelIndex = 0;
        }

        public override void InitializePage(object extraData)
        {
            if(CommInfo!=null)
                CommInfo.Clear();
            CommInfo = SerialPort.GetPortNames().ToList();
            
            try
            {
                //读取端口设置
                IpAddr = BasicConf.GetInstance().GetIP();
                IpPort = int.Parse(BasicConf.GetInstance().GetNetPort());
                IpPoseAddr = BasicConf.GetInstance().GetPoseIP();
                IpPosePort = int.Parse(BasicConf.GetInstance().GetPosePort());
                GPSIndex = BasicConf.GetInstance().GetCommGPS();
                SelectComm = CommInfo.IndexOf(GPSIndex);
                GPSRate = BasicConf.GetInstance().GetGPSDataRate();
                SelectRate = RateInfo.IndexOf(GPSRate);
                var sc = UnitCore.Instance.SonarConfiguration;
                uint velcmd = sc.VelCmd;
                SurVelSrcIndex = velcmd & 0x11;
                AvgVelIndex = velcmd & 0x100;
                SurVel = sc.SurVel;
                AvgVel = sc.AvgVel;
                FixedGain = sc.FixedGain;
                TVGCmd = sc.TVGCmd;
                FixedTVG = sc.FixedTVG;
                TVGSampling = sc.TVGSampling;
                TVGSamples = sc.TVGSamples;
                TVGA1 = sc.TVGA1;
                TVGA2 = sc.TVGA2;
                TVGA3 = sc.TVGA3;
                PingPeriod = sc.PingPeriod;
                ADSaved = sc.ADSaved;
                PoseSaved = sc.PoseSaved;
                PosSaved = sc.PosSaved;
                SonarDepth = sc.SonarDepth;
                SonarGPSx = sc.SonarGPSx;
                SonarGPSy = sc.SonarGPSy;
                SonarGPSz = sc.SonarGPSz;
            }
            catch (Exception ex)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("读取端口配置失败！", ex, LogType.Error));
            }
            
        }
        public string IpAddr
        {
            get { return GetPropertyValue(() => IpAddr); }
            set
            {
                IPAddress tempAddress;
                if (IPAddress.TryParse(value, out tempAddress) == false)
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("主IP地址格式不正确！",null, LogType.Warning));
                else
                {
                    SetPropertyValue(() => IpAddr, value);
                }
            }
        }
        public int IpPort
        {
            get { return GetPropertyValue(() => IpPort); }
            set
            {
                if (value < 1 || value > 65535)
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("主IP端口格式不正确！", null, LogType.Warning));
                else
                {
                    SetPropertyValue(() => IpPort, value);
                }
            }
        }
        public string IpPoseAddr
        {
            get { return GetPropertyValue(() => IpPoseAddr); }
            set
            {
                IPAddress tempAddress;
                if (IPAddress.TryParse(value, out tempAddress) == false)
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("姿态传感器IP地址格式不正确！", null, LogType.Warning));
                else
                {
                    SetPropertyValue(() => IpPoseAddr, value);
                }
            }
        }
        public int IpPosePort
        {
            get { return GetPropertyValue(() => IpPosePort); }
            set
            {
                if (value < 1 || value > 65535)
                    UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("姿态传感器网络端口格式不正确！", null, LogType.Warning));
                else
                {
                    SetPropertyValue(() => IpPosePort, value);
                }
            }
        }
        public List<string> CommInfo
        {
            get { return GetPropertyValue(() => CommInfo); }
            set { SetPropertyValue(() => CommInfo, value); }
        }
        public int SelectComm
        {
            get { return GetPropertyValue(() => SelectComm); }
            set { SetPropertyValue(() => SelectComm, value); }
        }
        public string GPSIndex
        {
            get { return GetPropertyValue(() => GPSIndex); }
            set { SetPropertyValue(() => GPSIndex, value); }
        }
        public List<string> RateInfo
        {
            get { return GetPropertyValue(() => RateInfo); }
            set { SetPropertyValue(() => RateInfo, value); }
        }
        public int SelectRate
        {
            get { return GetPropertyValue(() => SelectRate); }
            set { SetPropertyValue(() => SelectRate, value); }
        }
        public string GPSRate
        {
            get { return GetPropertyValue(() => GPSRate); }
            set { SetPropertyValue(() => GPSRate, value); }
        }
        /////
        public uint SurVelSrcIndex
        {
            get { return GetPropertyValue(() => SurVelSrcIndex); }
            set { SetPropertyValue(() => SurVelSrcIndex, value); }
        }
        public float SurVel
        {
            get { return GetPropertyValue(() => SurVel); }
            set { SetPropertyValue(() => SurVel, value); }
        }
        public uint AvgVelIndex
        {
            get { return GetPropertyValue(() => AvgVelIndex); }
            set { SetPropertyValue(() => AvgVelIndex, value); }
        }
        public float AvgVel
        {
            get { return GetPropertyValue(() => AvgVel); }
            set { SetPropertyValue(() => AvgVel, value); }
        }
        public float FixedGain
        {
            get { return GetPropertyValue(() => FixedGain); }
            set { SetPropertyValue(() => FixedGain, value); }
        }
        public uint TVGCmd
        {
            get { return GetPropertyValue(() => TVGCmd); }
            set { SetPropertyValue(() => TVGCmd, value); }
        }
        public float FixedTVG
        {
            get { return GetPropertyValue(() => FixedTVG); }
            set { SetPropertyValue(() => FixedTVG, value); }
        }
        public float TVGSampling
        {
            get { return GetPropertyValue(() => TVGSampling); }
            set { SetPropertyValue(() => TVGSampling, value); }
        }
        public uint TVGSamples
        {
            get { return GetPropertyValue(() => TVGSamples); }
            set { SetPropertyValue(() => TVGSamples, value); }
        }
        public float TVGA1
        {
            get { return GetPropertyValue(() => TVGA1); }
            set { SetPropertyValue(() => TVGA1, value); }
        }
        public float TVGA2
        {
            get { return GetPropertyValue(() => TVGA2); }
            set { SetPropertyValue(() => TVGA2, value); }
        }
        public float TVGA3
        {
            get { return GetPropertyValue(() => TVGA3); }
            set { SetPropertyValue(() => TVGA3, value); }
        }
        public float PingPeriod
        {
            get { return GetPropertyValue(() => PingPeriod); }
            set { SetPropertyValue(() => PingPeriod, value); }
        }
        public uint ADSaved
        {
            get { return GetPropertyValue(() => ADSaved); }
            set { SetPropertyValue(() => ADSaved, value); }
        }
        public uint PoseSaved
        {
            get { return GetPropertyValue(() => PoseSaved); }
            set { SetPropertyValue(() => PoseSaved, value); }
        }
        public uint PosSaved
        {
            get { return GetPropertyValue(() => PosSaved); }
            set { SetPropertyValue(() => PosSaved, value); }
        }
        public float SonarDepth
        {
            get { return GetPropertyValue(() => SonarDepth); }
            set { SetPropertyValue(() => SonarDepth, value); }
        }
        public float SonarGPSx
        {
            get { return GetPropertyValue(() => SonarGPSx); }
            set { SetPropertyValue(() => SonarGPSx, value); }
        }
        public float SonarGPSy
        {
            get { return GetPropertyValue(() => SonarGPSy); }
            set { SetPropertyValue(() => SonarGPSy, value); }
        }
        public float SonarGPSz
        {
            get { return GetPropertyValue(() => SonarGPSz); }
            set { SetPropertyValue(() => SonarGPSz, value); }
        }

        public ICommand SaveCommand
        {
            get { return GetPropertyValue(() => SaveCommand); }
            set { SetPropertyValue(() => SaveCommand, value); }
        }
        private void CanExecutesaveCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private void ExecutesaveCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            try
            {
                if (!BasicConf.GetInstance().SetIP(IpAddr)) throw new Exception("保存IP地址失败");
                if (!BasicConf.GetInstance().SetNetPort(IpPort.ToString())) throw new Exception("保存IP端口失败");
                if(!BasicConf.GetInstance().SetPoseIP(IpPoseAddr)) throw new Exception("保存姿态传感器IP地址失败");
                if (!BasicConf.GetInstance().SetPosePort(IpPosePort.ToString())) throw new Exception("保存姿态传感器IP端口失败");
                if (SelectComm != -1)
                    if(!BasicConf.GetInstance().SetCommGPS(CommInfo[SelectComm])) throw new Exception("保存串口端口失败");
                if (SelectRate != -1)
                    if (!BasicConf.GetInstance().SetGPSDataRate(RateInfo[SelectRate])) throw new Exception("保存串口速率失败");
                var sc = UnitCore.Instance.SonarConfiguration;
                sc.VelCmd = SurVelSrcIndex;
                if (AvgVelIndex == 1)
                    sc.VelCmd = sc.VelCmd | 0x0100;
                sc.SurVel = SurVel;
                sc.AvgVel = AvgVel;
                sc.FixedGain = FixedGain;
                sc.TVGCmd = TVGCmd;
                sc.FixedTVG = FixedTVG;
                sc.TVGSampling = TVGSampling;
                sc.TVGSamples = TVGSamples;
                sc.TVGA1 = TVGA1;
                sc.TVGA2 = TVGA2;
                sc.TVGA3 = TVGA3;
                sc.PingPeriod = PingPeriod;
                sc.ADSaved = ADSaved;
                sc.PoseSaved = PoseSaved;
                sc.PosSaved = PosSaved;
                sc.SonarDepth = SonarDepth;
                sc.SonarGPSx = SonarGPSx;
                sc.SonarGPSy = SonarGPSy;
                sc.SonarGPSz = SonarGPSz;
                if(!UnitCore.Instance.UpdateSonarConfig(false)) throw new Exception("声纳参数保存失败");
            }
            catch (Exception ex)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent(ex.Message, ex, LogType.Error));
                return;
            }
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "好的";
            MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "保存成功",
                "修改后的IP地址和端口配置将在下次启动程序时应用", MessageDialogStyle.Affirmative, md);
        }
    }
}
