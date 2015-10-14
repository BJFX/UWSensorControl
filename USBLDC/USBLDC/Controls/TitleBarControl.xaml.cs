﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls.Dialogs;
using TinyMetroWpfLibrary.Controls;
using USBLDC.ViewModel;

namespace USBLDC.Controls
{
    /// <summary>
    /// Interaction logic for TitleBarControl.xaml
    /// </summary>
    public partial class TitleBarControl : UserControl
    {
        public TitleBarControl()
        {
            InitializeComponent();
            
        }

        private MainFrameViewModel _vm;
        public MainFrameViewModel VM
        {
            get
            {
                if (_vm == null)
                {
                    _vm = MainFrameViewModel.pMainFrame;
                     
                }
                return _vm;
            }
        }


        private Window _mainWindow;
        public Window MainWindow
        {
            get
            {
                if (_mainWindow == null)
                {
                    _mainWindow = TreeHelper.TryFindParent<Window>(this);
                }
                return _mainWindow; 
            }
        }
        private void GoToSonarSettings(object sender, RoutedEventArgs e)
        {
            VM.GoToSonarSettings();
        }

        private void ShowAbout(object sender, RoutedEventArgs e)
        {
            VM.ShowAbout();
        }
        private void GoBack(object sender, RoutedEventArgs e)
        {
            VM.GoBack();
        }
        
        private void Minimize(object sender, RoutedEventArgs e)
        {
            if (MainWindow != null)
            {
                MainWindow.WindowState = WindowState.Minimized;
            }
        }

        private async void ExitProgram(object sender, RoutedEventArgs e)
        {
            var md = new MetroDialogSettings();
            md.AffirmativeButtonText = "退出";
            md.NegativeButtonText = "取消";
            md.ColorScheme = MetroDialogColorScheme.Accented;
            var result = await VM.DialogCoordinator.ShowMessageAsync(VM, "退出程序",
                "真的要退出程序吗？", MessageDialogStyle.AffirmativeAndNegative, md);
            if (result == MessageDialogResult.Affirmative)
                VM.ExitProgram();
        }

        private void ShowAboutPane(object sender, RoutedEventArgs e)
        {
            VM.ShowAbout();
        }
        public Visibility BackButtonVisibility
        {
            get { return (Visibility)GetValue(BackButtonVisibilityProperty); }
            set { SetValue(BackButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackButtonVisibilityProperty =
            DependencyProperty.Register("BackButtonVisibility", typeof(Visibility), typeof(TitleBarControl), new PropertyMetadata(Visibility.Visible));

        public ImageSource TitleImageSource
        {
            get { return (ImageSource)GetValue(TitleImageSourceProperty); }
            set { SetValue(TitleImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleImageSourceProperty =
            DependencyProperty.Register("TitleImageSource", typeof(ImageSource), typeof(TitleBarControl), new PropertyMetadata(null));


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TitleBarControl), new PropertyMetadata(string.Empty));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

          
     
        }


    }
}
