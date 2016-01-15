using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.Controller;
using USBLDC.Core;
using USBLDC.Events;
using USBLDC.ViewModel;
using System.Windows;
using System.Threading.Tasks;
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

        private  void ContentFrame_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.Current.MainWindow = this;
            UnitCore.Instance.Start();
            Task.Factory.StartNew(() =>
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
    }
}
