using System.Windows.Controls;
using Microsoft.Win32;

namespace USBLDC.Views
{
    /// <summary>
    /// SonarConfig.xaml 的交互逻辑
    /// </summary>
    public partial class SonarConfig : Page
    {
        public SonarConfig()
        {
            InitializeComponent();
        }

        private void SelectProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Title = "选择剖面文件";
            openFileDialog.Filter = "剖面文件 (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                ProfileBox.Text = openFileDialog.FileName;
            }
        }
    }
}
