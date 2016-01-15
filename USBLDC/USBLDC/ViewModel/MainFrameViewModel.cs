using System;
using System.Windows;
using System.Windows.Threading;
using NMEA0183;
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
        

        private  void UpdateStatus(object sender, EventArgs e)
        {
            if (UnitCore.Instance.NetCore != null && UnitCore.Instance.NetCore.IsWorking)
            {
                NetworkStatus = "网络状态：正在监听";
            }
            else
            {
                NetworkStatus = "网络状态：无连接";
            }
            if (UnitCore.Instance.CommCore != null && UnitCore.Instance.CommCore.IsWorking)
            {
                CommStatus = "GPS端口信息：已打开";
                var time = GPS.UTCTime.ToString();
                GPSLastUpdateTime = "GPS数据更新时间(UTC)："+time;
            }
            else
            {
                CommStatus = "GPS端口信息：关闭";
            }
        }
        public override void Initialize()
        {
            base.Initialize();
            DispatcherTimer dt = new DispatcherTimer(TimeSpan.FromSeconds(2), DispatcherPriority.Background, UpdateStatus, Dispatcher.CurrentDispatcher);
            dt.Start();
            pMainFrame = this;
            AboutVisibility = false;
            Version = "版本号：1.0";
            BuildNo = "Build：101";
            NetworkStatus = "网络状态：无连接";
            CommStatus = "GPS端口信息：关闭";
            GPSLastUpdateTime = "GPS数据更新时间：暂无数据";
            
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
            AboutVisibility = true;
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

        public string GPSLastUpdateTime
        {
            get { return GetPropertyValue(() => GPSLastUpdateTime); }
            set { SetPropertyValue(() => GPSLastUpdateTime, value); }
        }

        
        #endregion


        
    }
}
