using System;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using MahApps.Metro.Controls.Dialogs;
using USBLDC.ViewModel;

namespace USBLDC.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HomePageView : Page
    {
        public HomePageView()
        {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (HelixViewport3D == null || HelixViewport3D.CameraController==null)
                return;
            HelixViewport3D.CameraController.ChangeDirection(new Vector3D(0, 0, -6000), new Vector3D(-1, 0, 0), 1000);
            //HelixViewport3D.ZoomExtents(new Rect3D(ObjectD.Center.X, ObjectD.Center.Y, ObjectD.Center.Z, 
            //    2*Math.Abs(ObjectD.Center.X),2*Math.Abs(ObjectD.Center.Y),2*Math.Abs(ObjectD.Center.Z)));
        }

        private void RadioButton_Checked_1(object sender, System.Windows.RoutedEventArgs e)
        {
            if (HelixViewport3D == null || HelixViewport3D.CameraController == null)
                return;
            HelixViewport3D.CameraController.ChangeDirection(new Vector3D(0, 6000, 0), new Vector3D(0, 0, 1), 1000);
            //HelixViewport3D.ZoomExtents(new Rect3D(ObjectD.Center.X, ObjectD.Center.Y, ObjectD.Center.Z,
            //    2*Math.Abs(ObjectD.Center.X), 2*Math.Abs(ObjectD.Center.Y), 2*Math.Abs(ObjectD.Center.Z)));
        }

        private void RadioButton_Checked_2(object sender, System.Windows.RoutedEventArgs e)
        {
            if (HelixViewport3D == null || HelixViewport3D.CameraController == null)
                return;
            HelixViewport3D.CameraController.ChangeDirection(new Vector3D(-6000, 0, 0), new Vector3D(0, 0, 1), 1000);
            //HelixViewport3D.ZoomExtents(new Rect3D(ObjectD.Center.X, ObjectD.Center.Y, ObjectD.Center.Z, 
            //    2*Math.Abs(ObjectD.Center.X),2*Math.Abs(ObjectD.Center.Y),2*Math.Abs(ObjectD.Center.Z)));

        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //LookDownButton.IsChecked = true;
            //NorthButton_Click(null,e);
        }


        private void NorthButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PosViewport3D == null || PosViewport3D.CameraController == null)
                return;
            PosViewport3D.CameraController.ChangeDirection(new Vector3D(0, 0, -6000), new Vector3D(-1, 0, 0), 1000);
           // PosViewport3D.ZoomExtents(new Rect3D(ObjectD.Center.X, ObjectD.Center.Y, ObjectD.Center.Z,
           //     2 * Math.Abs(ObjectD.Center.X), 2 * Math.Abs(ObjectD.Center.Y), 2 * Math.Abs(ObjectD.Center.Z)));
        }

        private void PosViewport3D_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var distance = PosViewport3D.CameraController.CameraPosition.DistanceTo(new Point3D(0, 0, 0));
            if (distance > 18000 && e.Delta < 0)
                e.Handled = true;
            if (distance < 6000 && e.Delta > 0)
                e.Handled = true;
        }

    }
}
