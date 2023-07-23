using System.Threading;
using System;
using System.Windows;
using System.Windows.Input;

using Index.Class;

using KeyAuth;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Microsoft.Win32;

namespace Index
{
    public partial class Auth : Window
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

        public static api KeyAuthApp = new(
            name: "index",
            ownerid: "K6t7JKnr3q",
            secret: "", // life is... roblox
            version: "1.0"
        );

        public static RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Index");
        public static RegistryKey subkey = key.OpenSubKey("license");

        public Auth()
        {
            InitializeComponent();

            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
            var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL;
            DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));

            KeyAuthApp.init();

            if (!KeyAuthApp.response.success)
            {
                Methods.CheckConnection("KeyAuth");
                Error.Content = "Couldn't connect to KeyAuth";
            }

            if (key.GetValue("license") != null)
            {
                KeyAuthApp.license(key.GetValue("license").ToString());

                if (KeyAuthApp.response.success)
                {
                    KeyAuthApp.log($"Valid license: {licenseBox.Text}");
                    Main main = new();
                    main.Show();
                    Close();
                }
                else
                {
                    if (KeyAuthApp.response.message == "Invalid license key")
                    {
                        key.SetValue("license", null);
                        key.Close();

                        KeyAuthApp.log($"Incorrect license: {licenseBox.Text}");
                        licenseBox.Clear();
                        Error.Content = KeyAuthApp.response.message;
                    }
                    else
                    {
                        KeyAuthApp.log($"Unknown error: {KeyAuthApp.response.message}");
                        licenseBox.Clear();
                        Error.Content = KeyAuthApp.response.message;
                    }
                }
            }
        }

        private void continueClick(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(licenseBox.Text))
            {
                if (tosCheck.IsChecked == true)
                {
                    KeyAuthApp.license(licenseBox.Text);

                    if (KeyAuthApp.response.success)
                    {
                        key.SetValue("license", licenseBox.Text);
                        key.Close();
                        KeyAuthApp.log($"Valid license: {licenseBox.Text}");
                        Main main = new();
                        main.Show();
                        Close();
                    }
                    else
                    {
                        if (KeyAuthApp.response.message == "Invalid license key")
                        {
                            KeyAuthApp.log($"Incorrect license: {licenseBox.Text}");
                            licenseBox.Clear();
                            Error.Content = KeyAuthApp.response.message;
                        }
                        else
                        {
                            KeyAuthApp.log($"Unknown error: {KeyAuthApp.response.message}");
                            licenseBox.Clear();
                            Error.Content = KeyAuthApp.response.message;
                        }
                    }
                }
                else
                {
                    Error.Content = "You are required to agree to the ToS";
                }
            }
            else
            {
                Error.Content = "Invalid license key";
                licenseBox.Clear();
            }
        }

        private void dragWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ExitClick(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void MinimizeClick(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeClick(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            DataContext = new WindowBlureffect(this, AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND) { BlurOpacity = 3 };
        }
    }
}
