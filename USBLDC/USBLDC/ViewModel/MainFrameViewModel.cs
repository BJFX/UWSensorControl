using System;
using System.Windows;
using TinyMetroWpfLibrary.Events;
using TinyMetroWpfLibrary.ViewModel;
using USBLDC.Core;
using USBLDC.Events;
using USBLDC.Helpers;
using TinyMetroWpfLibrary.Frames;
using TinyMetroWpfLibrary.EventAggregation;
using MahApps.Metro.Controls;
using USBLDC.Structure;
using MahApps.Metro.Controls.Dialogs;
namespace USBLDC.ViewModel
{
    public class MainFrameViewModel : MainWindowViewModelBase
    {
        public static MainFrameViewModel pMainFrame { get; set; }
        private IDialogCoordinator _dialogCoordinator;
        public override void Initialize()
        {
            base.Initialize();
            pMainFrame = this;
            AboutVisibility = false;
            Version = "版本号：1.0";
            BuildNo = "Build：101";
            NetworkStatus = "网络状态：无连接";
            CommStatus = "GPS端口信息：关闭";
            GPSLastUpdateTime = "GPS数据更新时间：暂无数据";
            PosLastUpdateTime = "定位数据更新时间：暂无数据";
        }

        #region action
        internal void GoToSonarSettings()
        {
            EventAggregator.PublishMessage(new GoSonarConfigEvent());
        }
        internal void GoBack()
        {
            EventAggregator.PublishMessage(new GoBackNavigationRequest());
        }

        internal void ExitProgram()
        {
            Application.Current.Shutdown();
        }

        internal void ShowAbout()
        {
            AboutVisibility = !AboutVisibility;
        }

        #endregion
        #region binding property
        public IDialogCoordinator DialogCoordinator
        {
            get { return _dialogCoordinator; }
            set { _dialogCoordinator = value; }
        }
        public bool AboutVisibility
        {
            get { return GetPropertyValue(() => AboutVisibility); }
            set { SetPropertyValue(() => AboutVisibility, value); }
        }
        public string BuildNo
        {
            get { return GetPropertyValue(() => BuildNo); }
            set { SetPropertyValue(() => BuildNo, value); }
        }

        public string Version
        {
            get { return GetPropertyValue(() => Version); }
            set { SetPropertyValue(() => Version, value); }
        }

        public string NetworkStatus
        {
            get { return GetPropertyValue(() => NetworkStatus); }
            set { SetPropertyValue(() => NetworkStatus, value); }
        }
        public string CommStatus
        {
            get { return GetPropertyValue(() => CommStatus); }
            set { SetPropertyValue(() => CommStatus, value); }
        }
        public string PosLastUpdateTime
        {
            get { return GetPropertyValue(() => PosLastUpdateTime); }
            set { SetPropertyValue(() => PosLastUpdateTime, value); }
        }

        public string GPSLastUpdateTime
        {
            get { return GetPropertyValue(() => GPSLastUpdateTime); }
            set { SetPropertyValue(() => GPSLastUpdateTime, value); }
        }

        
        #endregion


        
    }
}
