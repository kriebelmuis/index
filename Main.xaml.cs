using Index;
using Index.Class;
using SuRGeoNix.BitSwarmLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SignalX
{
    public partial class Main : Window
    {
        public Main()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }

        private void dragWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void exitClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void minimizeClick(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void maximizeClick(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void downloadClick(object sender, MouseButtonEventArgs e)
        {
            this.Downloads.Visibility = Visibility.Visible;

            this.Library.Visibility = Visibility.Hidden;
            this.Home.Visibility = Visibility.Hidden;
            this.Settings.Visibility = Visibility.Hidden;
        }

        private void libraryClick(object sender, MouseButtonEventArgs e)
        {
            this.Library.Visibility = Visibility.Visible;

            this.Downloads.Visibility = Visibility.Hidden;
            this.Home.Visibility = Visibility.Hidden;
            this.Settings.Visibility = Visibility.Hidden;
        }

        private void homeClick(object sender, MouseButtonEventArgs e)
        {
            this.Home.Visibility = Visibility.Visible;

            this.Downloads.Visibility = Visibility.Hidden;
            this.Library.Visibility = Visibility.Hidden;
            this.Settings.Visibility = Visibility.Hidden;
        }

        private void settingsClick(object sender, MouseButtonEventArgs e)
        {
            this.Settings.Visibility = Visibility.Visible;

            this.Downloads.Visibility = Visibility.Hidden;
            this.Library.Visibility = Visibility.Hidden;
            this.Home.Visibility = Visibility.Hidden;
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            DataContext = new WindowBlureffect(this, AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND) { BlurOpacity = 100 };
        }
    }
}