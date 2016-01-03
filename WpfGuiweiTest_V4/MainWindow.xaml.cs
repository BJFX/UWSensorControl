using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using USBL.GuiWei;

namespace WpfGuiweiTest_V4
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

                           
        int maxDepth = 13000;//最大深度
        int[] SVPd = new int[13000 + 31];
        double[] SVPc = new double[13000 + 31];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt文件|*.txt|所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                SettleSoundFile SoundFile = new SettleSoundFile(openFileDialog.FileName);//整理声速剖面文件
                SVPd = SoundFile.SVPd;
                SVPc = SoundFile.SVPc;
                filepathbox.Text = openFileDialog.FileName;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //载体坐标系下潜器坐标
            float x_carrier = (float)-903.0254, y_carrier = (float)651.7115, z_carrier = (float)-868.9429;
//            float x_carrier = (float)-3.1919, y_carrier = (float)2.6654, z_carrier = (float)-12.3494;
//           float x_carrier = (float)-364.9461, y_carrier = (float)323.9491, z_carrier = (float)-3967.8869;
            //传感器姿态
            float Heading = 100, Pitch = 5, Roll = -5,Heave=0;
            //船的经纬度
            float Lon_local = 0, Lat_local = 0;
            //阵在船体坐标系下的坐标
            float us_x = 0, us_y = 0, us_z = -2;
            //传播时间
            float Travel_time = (float)0.94176875;
//            float Travel_time = (float)0.0087;
//            float Travel_time = (float)2.6652;
            //阵的深度
            float ArrayDepth=2;
            //归位处理
            USBL_GuiWei Position_Guiwei = new USBL_GuiWei(x_carrier, y_carrier, z_carrier,Heave, Heading, Pitch, Roll, Lon_local, Lat_local, us_x, us_y, us_z, Travel_time, ArrayDepth, SVPd, SVPc);

        }




    }
}
