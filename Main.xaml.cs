using System;
using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Index.Class;

namespace Index
{
    public partial class Main : Window
    {
        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }

        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void DwmSetWindowAttribute(IntPtr hwnd,
                                                         DWMWINDOWATTRIBUTE attribute,
                                                         ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                         uint cbAttribute);
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

            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
            var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL;
            DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));

            Downloads d = new Downloads(null, 0, 3);

            Downloads.Navigate(d);
        }

        private void dragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void exitClick(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void minimizeClick(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void maximizeClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void downloadClick(object sender, MouseButtonEventArgs e)
        {
            Downloads.Visibility = Visibility.Visible;

            Library.Visibility = Visibility.Hidden;
            Home.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Hidden;
            Game.Visibility = Visibility.Hidden;
        }

        private void libraryClick(object sender, MouseButtonEventArgs e)
        {
            Library.Visibility = Visibility.Visible;

            Downloads.Visibility = Visibility.Hidden;
            Home.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Hidden;
            Game.Visibility = Visibility.Hidden;
        }

        private void homeClick(object sender, MouseButtonEventArgs e)
        {
            Home.Visibility = Visibility.Visible;

            Downloads.Visibility = Visibility.Hidden;
            Library.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Hidden;
            Game.Visibility = Visibility.Hidden;
        }

        private void settingsClick(object sender, MouseButtonEventArgs e)
        {
            Settings.Visibility = Visibility.Visible;

            Downloads.Visibility = Visibility.Hidden;
            Library.Visibility = Visibility.Hidden;
            Home.Visibility = Visibility.Hidden;
            Game.Visibility = Visibility.Hidden;
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            DataContext = new WindowBlureffect(this, AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND) { BlurOpacity = 1 };
        }
    }
}