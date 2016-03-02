using System;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

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
            HelixViewport3D.ZoomExtents(new Rect3D(ObjectD.Center.X, ObjectD.Center.Y, ObjectD.Center.Z, 
                2*Math.Abs(ObjectD.Center.X),2*Math.Abs(ObjectD.Center.Y),2*Math.Abs(ObjectD.Center.Z)));
        }

        private void RadioButton_Checked_1(object sender, System.Windows.RoutedEventArgs e)
        {
            if (HelixViewport3D == null || HelixViewport3D.CameraController == null)
                return;
            HelixViewport3D.CameraController.ChangeDirection(new Vector3D(0, 6000, 0), new Vector3D(0, 0, 1), 1000);
            HelixViewport3D.ZoomExtents(new Rect3D(ObjectD.Center.X, ObjectD.Center.Y, ObjectD.Center.Z,
                2*Math.Abs(ObjectD.Center.X), 2*Math.Abs(ObjectD.Center.Y), 2*Math.Abs(ObjectD.Center.Z)));
        }

        private void RadioButton_Checked_2(object sender, System.Windows.RoutedEventArgs e)
        {
            if (HelixViewport3D == null || HelixViewport3D.CameraController == null)
                return;
            HelixViewport3D.CameraController.ChangeDirection(new Vector3D(-6000, 0, 0), new Vector3D(0, 0, 1), 1000);
            HelixViewport3D.ZoomExtents(new Rect3D(ObjectD.Center.X, ObjectD.Center.Y, ObjectD.Center.Z, 
                2*Math.Abs(ObjectD.Center.X),2*Math.Abs(ObjectD.Center.Y),2*Math.Abs(ObjectD.Center.Z)));

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
            PosViewport3D.ZoomExtents(new Rect3D(ObjectD.Center.X, ObjectD.Center.Y, ObjectD.Center.Z,
                2 * Math.Abs(ObjectD.Center.X), 2 * Math.Abs(ObjectD.Center.Y), 2 * Math.Abs(ObjectD.Center.Z)));
        }
    }
}
