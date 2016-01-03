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
using TinyMetroWpfLibrary.Events;
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
            SaveAsCommand = RegisterCommand(ExecuteSaveAsCommand, CanExecuteSaveAsCommand, true);
            LoadCommand = RegisterCommand(ExecuteLoadCommand, CanExecuteLoadCommand, true);
            SaveAndRun = RegisterCommand(ExecuteSaveAndRun, CanExecuteSaveAndRun, true);
            SurVelSrcIndex = 0;
            AvgVelIndex = 0;
            ShowInPage = true;
        }

        public override void InitializePage(object extraData)
        {
            var para = extraData as GoSonarConfigEvent;
            if (para != null)
                ShowInPage = para.Show;
            
            //CommInfo = SerialPort.GetPortNames().ToList();
            
            try
            {
                
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
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(new Exception("读取参数失败！"),  LogType.Both));
            }
            
        }
        public bool ShowInPage
        {
            get { return GetPropertyValue(() => ShowInPage); }
            set { SetPropertyValue(() => ShowInPage, value); }
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

        public ICommand SaveAsCommand
        {
            get { return GetPropertyValue(() => SaveAsCommand); }
            set { SetPropertyValue(() => SaveAsCommand, value); }
        }
        private void CanExecuteSaveAsCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private void ExecuteSaveAsCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            try
            {
                
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
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(ex, LogType.Both));
                return;
            }
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "好的";
            MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "保存成功",
                "修改后的IP地址和端口配置将在下次启动程序时应用", MessageDialogStyle.Affirmative, md);
        }


        public ICommand LoadCommand
        {
            get { return GetPropertyValue(() => LoadCommand); }
            set { SetPropertyValue(() => LoadCommand, value); }
        }
        private void CanExecuteLoadCommand(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }
        private void ExecuteLoadCommand(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            try
            {
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
                if (!UnitCore.Instance.UpdateSonarConfig(false)) throw new Exception("声纳参数保存失败");
            }
            catch (Exception ex)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(ex, LogType.Both));
                return;
            }
            
        }

        public ICommand SaveAndRun
        {
            get { return GetPropertyValue(() => SaveAndRun); }
            set { SetPropertyValue(() => SaveAndRun, value); }
        }
        private void CanExecuteSaveAndRun(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }
        private void ExecuteSaveAndRun(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            try
            {
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
                if (!UnitCore.Instance.UpdateSonarConfig(false)) throw new Exception("声纳参数保存失败");
                UnitCore.Instance.EventAggregator.PublishMessage(new GoBackNavigationRequest());
            }
            catch (Exception ex)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new ErrorEvent(ex, LogType.Both));
                return;
            }

        }
    }
}
