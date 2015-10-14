using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TinyMetroWpfLibrary.Controls;
using USBLDC.Core;

namespace USBLDC
{
    /// <summary>
    /// SplashWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SplashWindow
    {
        
        public SplashWindow()
        {
            InitializeComponent();
            PathBox.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            NameBox.Text = CreateNonBlankTimeString();
            ErrBlock.Text = "";
        }
        private string CreateNonBlankTimeString()
        {
            string timestring = DateTime.Now.Year.ToString("0000_", CultureInfo.InvariantCulture) +
                                DateTime.Now.Month.ToString("00_", CultureInfo.InvariantCulture) +
                                DateTime.Now.Day.ToString("00_", CultureInfo.InvariantCulture);
            int tail = 0;
            DirectoryInfo di = null;
            try
            {
                di = new DirectoryInfo(PathBox.Text);
            }
            catch (Exception)
            {
                PathBox.Text = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            }
            
            while (true)
            {
                var name = timestring + tail.ToString();
                di = new DirectoryInfo(PathBox.Text + "\\" + name);
                if (di.Exists)
                {
                    tail++;
                    continue;
                }
                else
                {
                    timestring = name;
                    break;
                }
                
            }
            
             
            return timestring;
        }

        /// <summary>
        /// 检查文件名是否合法.文字名中不能包含字符\/:*?"<>|
        /// </summary>
        /// <param name="fileName">文件名,不包含路径</param>
        /// <returns></returns>
        private bool IsValidFileName(string fileName)
        {
            bool isValid = true;
            string errChar = "\\/:*?\"<>|";  //
            if (string.IsNullOrEmpty(fileName))
            {
                isValid = false;
            }
            else
            {
                for (int i = 0; i < errChar.Length; i++)
                {
                    if (fileName.Contains(errChar[i].ToString()))
                    {
                        isValid = false;
                        break;
                    }
                }
            }
            return isValid;
        }     

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = System.Environment.CurrentDirectory;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                PathBox.Text = fbd.SelectedPath;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if(!IsValidFileName(NameBox.Text))
                    throw new Exception("工程名无效！");
                DirectoryInfo di = new DirectoryInfo(PathBox.Text);
                if (!di.Exists)
                {
                    var result = System.Windows.Forms.MessageBox.Show("路径不存在，需要创建文件夹吗？", "提示", MessageBoxButtons.OKCancel);
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        di.Create();
                    }
                    else
                    {
                        throw new Exception("请重新选择工程路径！");
                    }
                }
                this.Hide();

            }
            catch (Exception exception)
            {
                ErrBlock.Text = exception.Message;
            }

        }

    }
}
