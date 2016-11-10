using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using TinyMetroWpfLibrary.Controller;
using USBLDC.Core;
using USBLDC.Events;
using USBLDC.ViewModel;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Win32;
using USBLDC.Structure;
using System.Collections.Generic;
using System.IO;
namespace USBLDC.Views
{
    /// <summary>
    /// MainFrame.xaml 的交互逻辑
    /// </summary>
    public partial class MainFrame
    {
        public MainFrame()
        {
            InitializeComponent();
            MainFrameViewModel.pMainFrame.DialogCoordinator = DialogCoordinator.Instance;
            Kernel.Instance.Controller.SetRootFrame(ContentFrame);
        }

        private async void ContentFrame_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.Current.MainWindow = this;
            await UnitCore.Instance.Start();
            await Task.Factory.StartNew(() =>
            {
                UnitCore.Instance.NetCore.Initialize();
                UnitCore.Instance.NetCore.Start();
                UnitCore.Instance.CommCore.Initialize();
                UnitCore.Instance.CommCore.Start();
            });
            Splasher.Splash = new ConnectWindow();
            Splasher.ShowSplash();
            //UnitCore.Instance.EventAggregator.PublishMessage(new GoHomePageNavigationEvent());
        }

        private void MetroWindow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        private void SelectReplayFile(object sender, RoutedEventArgs e)
        {
            var newdialog = (BaseMetroDialog)App.Current.MainWindow.Resources["ReplayDialog"];
            var status = newdialog.FindChild<TextBlock>("StatusBlock");
            var btn = newdialog.FindChild<Button>("PlayBtn");
            var box = newdialog.FindChild<ComboBox>("SelectModeBox");
            if(box.SelectedIndex==0)//file
            {
                OpenFileDialog OpenFileDlg = new OpenFileDialog();
                if (OpenFileDlg.ShowDialog() == true)
                {
                    var ReplayFileName = OpenFileDlg.FileName;
                    if (UnitCore.Instance.Replaylist == null)
                        UnitCore.Instance.Replaylist = new List<string>();
                    UnitCore.Instance.Replaylist.Add(ReplayFileName);
                    btn.IsEnabled = true;
                }
            }
            else if(box.SelectedIndex==1)//folder
            {
                var FolderDlg = new System.Windows.Forms.FolderBrowserDialog();
                FolderDlg.Description = "请选择结果文件路径";
                if (FolderDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string foldPath = FolderDlg.SelectedPath;
                    var di = new DirectoryInfo(foldPath);
                    if (di.Exists)
                    {
                        var filist = di.GetFiles("*.pos", SearchOption.TopDirectoryOnly);
                        foreach (var fi in filist)
                        {
                            var ReplayFileName = fi.FullName;
                            if (UnitCore.Instance.Replaylist == null)
                                UnitCore.Instance.Replaylist = new List<string>();
                            UnitCore.Instance.Replaylist.Add(ReplayFileName);
                            btn.IsEnabled = true;
                        }
                    }
                }
            }
            
        }

        private async void StartReplayBtn(object sender, RoutedEventArgs e)
        {
            AjustPositionInfo ajInfo = new AjustPositionInfo();

            var info = (StructureInterface)ajInfo;
            var Id = (int)TypeId.AjustPos;
            UnitCore.Instance.EventAggregator.PublishMessage(new ShowStructureInfo(info, Id));
            await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, (BaseMetroDialog)App.Current.MainWindow.Resources["ReplayDialog"]);
        }

        private async void CloseDialog(object sender, RoutedEventArgs e)
        {
            await MainFrameViewModel.pMainFrame.DialogCoordinator.HideMetroDialogAsync(MainFrameViewModel.pMainFrame, (BaseMetroDialog)App.Current.MainWindow.Resources["ReplayDialog"]);
        }
    }
}
