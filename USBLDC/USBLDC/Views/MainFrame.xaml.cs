using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.Controller;
using USBLDC.Core;
using USBLDC.Events;
using USBLDC.ViewModel;
using System.Windows;
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
            //DataContext = MainFrameViewModel.pMainFrame;

            Kernel.Instance.Controller.SetRootFrame(ContentFrame);
        }

        private void ContentFrame_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

            ProgressDialogController remote = null;
            UnitCore.Instance.Start();
            UnitCore.Instance.EventAggregator.PublishMessage(new GoHomePageNavigationEvent());
        }

        private void MetroWindow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
    }
}
