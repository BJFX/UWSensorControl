using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;
using NMEA0183;
using TinyMetroWpfLibrary.EventAggregation;
using TinyMetroWpfLibrary.ViewModel;
using USBLDC.Core;
using USBLDC.Events;
using USBLDC.Structure;
using USBLDC.Helpers;
using System.Windows.Controls;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using HelixToolkit.Wpf;

namespace USBLDC.ViewModel
{
    public class HomePageViewModel : ViewModelBase, IHandleMessage<ShowStructureInfo>
    {
        private DispatcherTimer dtTimer = null;
        private int GpsTickCount = 0;
        private int PoseTickCount = 0;
        private int PosTickCount = 0;
        private List<Point3D> Path = new List<Point3D>();//track

        /// 中间变量，负责缓存大量更新数据
        private PosetureInfo poseture = null;
        private GPSInfo gpsInfo = null;

        /// 
        private DateTime posetime = new DateTime(1970,1,1);

        private DateTime ajusttime = new DateTime(1970, 1, 1);

        private DateTime gpstime = new DateTime(1970, 1, 1);
        public override void Initialize()
        {
            Heading = 45;
            Pitch = -30;
            Roll = 30;
            Heave = 100;
            StartCMD = RegisterCommand(ExecuteStartCMD, CanExecuteStartCMD, true);
            StopCMD = RegisterCommand(ExecuteStopCMD, CanExecuteStopCMD, true);
            HeadingChartTitle = "X=" + coordinateX.ToString("F02") + "m" + "\n" + "Y=" + coordinateY.ToString("F02") +
                                "m" + "\n" +
                                "Z=" + coordinateZ.ToString("F02") + "m";
            ShowCmd = true;
            CurrentModel = null;
            gpsTime = "GPS";
            ObjectVisibility = false;
        }

        public override void InitializePage(object extraData)
        {
            if (CurrentModel == null && UnitCore.Instance.CurrentModel != null)
                CurrentModel = UnitCore.Instance.CurrentModel;
            var pos = new AjustPositionInfo();
            pos.Status = 1;
            pos.XAjust = 1000;
            pos.YAjust = 3040;
            pos.ZAjust = 2300;
            pos.Noise = 71;
            UpdatePositionView(pos);
            if(dtTimer==null)
                dtTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Input,
            UpdateAllData, Dispatcher.CurrentDispatcher);
        }

        private void UpdateAllData(object sender, EventArgs e)
        {
            PoseTickCount++;
            if(gpsInfo!=null)
                UpdateGpsView(gpsInfo);
            if(poseture!=null)
                UpdatePoseView(poseture);
            
            if (PoseTickCount %2==0)
            {
                if (UnitCore.Instance.ajustPosition != null)
                    UpdatePositionView(UnitCore.Instance.ajustPosition.Last().Value);
            }
            if (UnitCore.Instance.NetCore != null && UnitCore.Instance.NetCore.IsWorking&&UnitCore.Instance.NetCore.SonarIsOK)
            {
                if (UnitCore.Instance.NetCore.SonarIsLink)
                {
                    if (ajusttime.CompareTo(new DateTime(1970,1,1))<=0)
                        SonarUpdate = "已连接，无数据 ";
                    else
                        SonarUpdate = "已连接，更新时间 " + ajusttime.ToString();
                }
                else
                {
                    SonarUpdate = "等待连接";
                }
                    
            }
            else
            {
                SonarUpdate = "无连接";
            }
            if (UnitCore.Instance.NetCore != null && UnitCore.Instance.NetCore.IsWorking&&UnitCore.Instance.NetCore.PoseIsOK)
            {
                if (UnitCore.Instance.NetCore.PoseIsLink)
                {
                    if (posetime.CompareTo(new DateTime(1970, 1, 1)) <= 0)
                        PoseUpdate = "已连接，无数据 ";
                    else
                        PoseUpdate = "已连接，更新时间 " + posetime.ToString();
                }
                else
                {
                    PoseUpdate = "等待连接";
                }
                    
            }
            else
            {
                SonarUpdate = "无连接";
            }
            if (UnitCore.Instance.CommCore != null && UnitCore.Instance.CommCore.IsWorking)
            {
                if (gpsTime.CompareTo(new DateTime(1970, 1, 1)) <= 0)
                    GPSLastUpdate = "已连接，无数据 ";
                else
                    GPSLastUpdate = "端口打开，更新时间 " + gpsTime.ToString();
            }
            else
            {
                GPSLastUpdate = "端口关闭";
            }
        }
        public string SonarUpdate
        {
            get { return GetPropertyValue(() => SonarUpdate); }
            set { SetPropertyValue(() => SonarUpdate, value); }
        }
        public string PoseUpdate
        {
            get { return GetPropertyValue(() => PoseUpdate); }
            set { SetPropertyValue(() => PoseUpdate, value); }
        }

        public string GPSLastUpdate
        {
            get { return GetPropertyValue(() => GPSLastUpdate); }
            set { SetPropertyValue(() => GPSLastUpdate, value); }
        }
        public uint SonarStatus
        {
            get { return GetPropertyValue(() => SonarStatus); }
            set { SetPropertyValue(() => SonarStatus, value); }
        }

        public float coordinateX
        {
            get { return GetPropertyValue(() => coordinateX); }
            set { SetPropertyValue(() => coordinateX, value); }
        }

        public float coordinateY
        {
            get { return GetPropertyValue(() => coordinateY); }
            set { SetPropertyValue(() => coordinateY, value); }
        }
        
        public float coordinateZ
        {
            get { return GetPropertyValue(() => coordinateZ); }
            set { SetPropertyValue(() => coordinateZ, value); }
        }

        public float targetLong
        {
            get { return GetPropertyValue(() => targetLong); }
            set { SetPropertyValue(() => targetLong, value); }
        }

        public float targetLat
        {
            get { return GetPropertyValue(() => targetLat); }
            set { SetPropertyValue(() => targetLat, value); }
        }

        public float Noise
        {
            get { return GetPropertyValue(() => Noise); }
            set { SetPropertyValue(() => Noise, value); }
        }

        public string gpsTime
        {
            get { return GetPropertyValue(() => gpsTime); }
            set { SetPropertyValue(() => gpsTime, value); }
        }

        public float shipLong
        {
            get { return GetPropertyValue(() => shipLong); }
            set { SetPropertyValue(() => shipLong, value); }
        }

        public float shipLat
        {
            get { return GetPropertyValue(() => shipLat); }
            set { SetPropertyValue(() => shipLat, value); }
        }

        public uint Satelites
        {
            get { return GetPropertyValue(() => Satelites); }
            set { SetPropertyValue(() => Satelites, value); }
        }

        public float gpsSpeed
        {
            get { return GetPropertyValue(() => gpsSpeed); }
            set { SetPropertyValue(() => gpsSpeed, value); }
        }

        public uint gpsStatus
        {
            get { return GetPropertyValue(() => gpsStatus); }
            set { SetPropertyValue(() => gpsStatus, value); }
        }

        public float Heading
        {
            get { return GetPropertyValue(() => Heading); }
            set { SetPropertyValue(() => Heading, value); }
        }

        public float Pitch
        {
            get { return GetPropertyValue(() => Pitch); }
            set { SetPropertyValue(() => Pitch, value); }
        }

        public float Roll
        {
            get { return GetPropertyValue(() => Roll); }
            set { SetPropertyValue(() => Roll, value); }
        }

        public float Heave
        {
            get { return GetPropertyValue(() => Heave); }
            set { SetPropertyValue(() => Heave, value); }
        }
        private uint poseStatus = 0;
        
        public Model3D CurrentModel
        {
            get { return GetPropertyValue(() => CurrentModel); }
            set { SetPropertyValue(() => CurrentModel, value); }
        }
        public Model3D TrackModel
        {
            get { return GetPropertyValue(() => TrackModel); }
            set { SetPropertyValue(() => TrackModel, value); }
        }
        public string HeadingChartTitle
        {
            get { return GetPropertyValue(() => HeadingChartTitle); }
            set { SetPropertyValue(() => HeadingChartTitle, value); }
        }

        public string ObjectCenter
        {
            get { return GetPropertyValue(() => ObjectCenter); }
            set { SetPropertyValue(() => ObjectCenter, value); }
        }
        public bool ObjectVisibility
        {
            get { return GetPropertyValue(() => ObjectVisibility); }
            set { SetPropertyValue(() => ObjectVisibility, value); }
        }
        public uint PoseStatus
        {
            get { return GetPropertyValue(() => PoseStatus); }
            set { SetPropertyValue(() => PoseStatus, value); }
        }
        public string PoseTime
        {
            get { return GetPropertyValue(() => PoseTime); }
            set { SetPropertyValue(() => PoseTime, value); }
        }

        public bool ShowCmd
        {
            get { return GetPropertyValue(() => ShowCmd); }
            set { SetPropertyValue(() => ShowCmd, value); }
        }
        public ICommand StartCMD
        {
            get { return GetPropertyValue(() => StartCMD); }
            set { SetPropertyValue(() => StartCMD, value); }
        }
        private void CanExecuteStartCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private async void ExecuteStartCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {

            if (!UnitCore.Instance.NetCore.SonarIsOK)
            {
                UnitCore.Instance.NetCore.Stop();
                UnitCore.Instance.NetCore.Initialize();
                UnitCore.Instance.NetCore.Start();
            }
            if (!UnitCore.Instance.CommCore.IsWorking)
            {
                UnitCore.Instance.CommCore.Initialize();
                UnitCore.Instance.CommCore.Start();
            }
            await TaskEx.Delay(2000);
            if (UnitCore.Instance.NetCore.SonarIsOK != true)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("无法与声纳通信，请检查网络连接！", LogType.OnlyInfo));
                return;
            }
            if (UnitCore.Instance.SoundFile == null)
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("没有可用的声速剖面文件！", LogType.OnlyInfo));
                return;
            }
            if (!UnitCore.Instance.USBLTraceService.StartService())
            {
                UnitCore.Instance.EventAggregator.PublishMessage(new LogEvent("工程设置失败！", LogType.OnlyInfo));
                return;
            }
            var sc = UnitCore.Instance.SonarConfiguration;
            uint velcmd = sc.VelCmd;
            
            //处理声速剖面
            var velarray = UnitCore.Instance.SoundFile.SVPc;
            var deparray = UnitCore.Instance.SoundFile.SVPd;
            if ((velcmd & 0x11) == 0)
            {
                int id = 0;
                foreach (var d in deparray)
                {
                    if (d == sc.SonarDepth)
                    {
                        break;
                    }
                    id++;
                }
                sc.SurVel = (float)velarray[id];
            }
            if ((velcmd & 0x11) == 1)
            {
                sc.AvgVel = (float) velarray.Average();
            }
            bool ret = false;
            UnitCore.Instance.SonarConfiguration.Cmd = 1;
            ret =  UnitCore.Instance.NetCore.SendCMD(UnitCore.Instance.SonarConfiguration.SavePackage());
            if (ret == false)
            {
                var md = new MetroDialogSettings();
                md.AffirmativeButtonText = "关闭";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "开始工作失败",
                    UnitCore.Instance.NetCore.Error, MessageDialogStyle.Affirmative, md);
            }
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = "开始工作";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                    dialog);

                var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                textBlock.Text = "命令发送成功！";

                await TaskEx.Delay(1000);
                ShowCmd = false;
                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
            }
        }
        public ICommand StopCMD
        {
            get { return GetPropertyValue(() => StopCMD); }
            set { SetPropertyValue(() => StopCMD, value); }
        }

        

        private void CanExecuteStopCMD(object sender, CanExecuteRoutedEventArgs eventArgs)
        {
            eventArgs.CanExecute = true;
        }


        private async void ExecuteStopCMD(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if(!UnitCore.Instance.NetCore.IsWorking || ShowCmd)
                return;
            bool ret = false;
            UnitCore.Instance.SonarConfiguration.Cmd = 0;
            ret = UnitCore.Instance.NetCore.SendCMD(UnitCore.Instance.SonarConfiguration.SavePackage());
            if (ret == false)
            {
                var md = new MetroDialogSettings();
                md.AffirmativeButtonText = "关闭";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMessageAsync(MainFrameViewModel.pMainFrame, "停止工作",
                    UnitCore.Instance.NetCore.Error, MessageDialogStyle.Affirmative, md);
            }
            else
            {
                var dialog = (BaseMetroDialog)App.Current.MainWindow.Resources["CustomInfoDialog"];
                dialog.Title = "停止工作";
                await MainFrameViewModel.pMainFrame.DialogCoordinator.ShowMetroDialogAsync(MainFrameViewModel.pMainFrame,
                    dialog);

                var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
                textBlock.Text = "命令发送成功！";

                await TaskEx.Delay(2000);
                ShowCmd = true;
                await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, dialog);
            }
        }
        public void Handle(ShowStructureInfo message)
        {
            switch (message.Id)
            {
                case (int)TypeId.GPS:
                    //UpdateGpsView(message);
                    gpsInfo = message.Info as GPSInfo;
                    gpstime = DateTime.Now;
                    break;
                case (int)TypeId.Pose:
                    //UpdatePoseView(message);
                    poseture = message.Info as PosetureInfo;
                    posetime = DateTime.Now;
                    break;
                case (int)TypeId.RawPos:
                    break;
                case (int)TypeId.AjustPos:
                    //UpdatePositionView(message);
                    if (UnitCore.Instance.ajustPosition == null)
                        UnitCore.Instance.ajustPosition = new Dictionary<DateTime, AjustPositionInfo>();
                    ajusttime = DateTime.Now;
                    UnitCore.Instance.ajustPosition.Add(ajusttime, message.Info as AjustPositionInfo);
                    break;

                default:
                    break;
            }
        }

        private void UpdatePoseView(PosetureInfo info)
        {
            //PoseTime = info.EpochSecond;
            Heading = info.Heading;
            Pitch = info.Pitch;
            Roll = info.Roll;
            Heave = info.Heave;
            PoseStatus = info.Status;
            
        }

        private void UpdatePositionView(AjustPositionInfo info)
        {
                coordinateX = info.XAjust;
                coordinateY = info.YAjust;
                coordinateZ = info.ZAjust;
                SonarStatus = info.Status;
                targetLat = info.AjustLat;
                targetLong = info.AjustLong;
                Noise = info.Noise;
                var x = -coordinateX;//坐标轴x相反，取反,5是画图系数
                var y = -coordinateY;//坐标轴y相反，取反
                var z = -coordinateY;//坐标轴z相反，取反
                ObjectCenter = x.ToString("F02") + "," + y.ToString("F02") + "," + z.ToString("F02");
                if ((x * x + y * y + z * z) > 1)
                    ObjectVisibility = true;
                UpdataTrack(x, y, z);
                UpdataTrack(x+200, y-100, z+500);
                UpdataTrack(x + 600, y + 1000, z + 1500);
        }
        public bool TrackVisible
        {
            get { return GetPropertyValue(() => TrackVisible); }
            set
            {

                if (value == false)
                    TrackModel = null;
                else
                {
                    // create the WPF3D model
                    var m = new Model3DGroup();
                    var gm = new MeshBuilder();
                    gm.AddTube(Path, 18, 10, false);
                    m.Children.Add(new GeometryModel3D(gm.ToMesh(), Materials.Gold));
                    TrackModel = m;
                }
                SetPropertyValue(() => TrackVisible, value);
                
            }
        }
        private void RmoveTrack()
        {
            TrackVisible=false;
            Path.RemoveAll((s)=> { return s != null; });
            
        }

        private void UpdataTrack(float x, float y, float z)
        {
            Path.Add(new Point3D(x, y, z));
            if (TrackVisible)
            {
                // create the WPF3D model
                var m = new Model3DGroup();
                var gm = new MeshBuilder();
                gm.AddTube(Path, 18, 10, false);
                m.Children.Add(new GeometryModel3D(gm.ToMesh(), Materials.Gold));
                TrackModel = m;
            }
        }

        private void UpdateGpsView(GPSInfo info)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            gpsTime = "GPS-"+epoch.AddSeconds(info.GPSSecond).AddMilliseconds(info.GPSMicSecond).ToString();
            gpsStatus = info.GPSStatus;
            Satelites = info.SatNum;
            gpsSpeed = info.Velocity;
            shipLong = info.Long;
            shipLat = info.Lat;
        }
    }
}
