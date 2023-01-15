
using Index.Class;

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using System.Data;
using System.Diagnostics;

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

                Application.Current.MainWindow = this;

                var hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
                var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
                var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL;
                DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
            }
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Index", MessageBoxButton.OK, MessageBoxImage.Error);
				Environment.Exit(0);
			}
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
			WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
		}

		private void downloadClick(object sender, MouseButtonEventArgs e)
		{
			if (Data.visible != "Downloads")
			{
				var foo = (Frame)FindName(Data.visible);
				DoubleAnimation da = new DoubleAnimation
				{
					From = 1.0,
					To = 0.0,
					Duration = new Duration(TimeSpan.FromSeconds(.25))
				};
				da.Completed += DownloadCompleted;
				var foo1 = (Frame)FindName("Downloads");
				DoubleAnimation da1 = new DoubleAnimation
				{
					From = 0.0,
					To = 1.0,
					Duration = new Duration(TimeSpan.FromSeconds(.25))
				};
				foo.BeginAnimation(OpacityProperty, da);
				foo1.BeginAnimation(OpacityProperty, da1);
			}
		}

		private void DownloadCompleted(object sender, EventArgs e)
		{
			var foo = (Frame)FindName(Data.visible);
			Data.visible = "Downloads";
			foo.Visibility = Visibility.Hidden;
			var foo1 = (Frame)FindName("Downloads");
			foo1.Visibility = Visibility.Visible;
		}

		private void libraryClick(object sender, MouseButtonEventArgs e)
		{
			if (Data.visible != "Library")
			{
				var foo = (Frame)FindName(Data.visible);
				DoubleAnimation da = new DoubleAnimation
				{
					From = 1.0,
					To = 0.0,
					Duration = new Duration(TimeSpan.FromSeconds(.25))
				};
				da.Completed += LibraryCompleted;
				var foo1 = (Frame)FindName("Library");
				DoubleAnimation da1 = new DoubleAnimation
				{
					From = 0.0,
					To = 1.0,
					Duration = new Duration(TimeSpan.FromSeconds(.25))
				};
				foo.BeginAnimation(OpacityProperty, da);
				foo1.BeginAnimation(OpacityProperty, da1);
			}
		}

		private void LibraryCompleted(object sender, EventArgs e)
		{
			var foo = (Frame)FindName(Data.visible);
			Data.visible = "Library";
			foo.Visibility = Visibility.Hidden;
			var foo1 = (Frame)FindName("Library");
			foo1.Visibility = Visibility.Visible;
		}

		private void homeClick(object sender, MouseButtonEventArgs e)
		{
			if (Data.visible != "Home")
			{
				var foo = (Frame)FindName(Data.visible);
				DoubleAnimation da = new DoubleAnimation
				{
					From = 1.0,
					To = 0.0,
					Duration = new Duration(TimeSpan.FromSeconds(.25))
				};
				da.Completed += HomeCompleted;
				var foo1 = (Frame)FindName("Home");
				DoubleAnimation da1 = new DoubleAnimation
				{
					From = 0.0,
					To = 1.0,
					Duration = new Duration(TimeSpan.FromSeconds(.25))
				};
				foo.BeginAnimation(OpacityProperty, da);
				foo1.BeginAnimation(OpacityProperty, da1);
			}
		}

		private void HomeCompleted(object sender, EventArgs e)
		{
			var foo = (Frame)FindName(Data.visible);
			Data.visible = "Home";
			foo.Visibility = Visibility.Hidden;
			var foo1 = (Frame)FindName("Home");
			foo1.Visibility = Visibility.Visible;
		}

		private void settingsClick(object sender, MouseButtonEventArgs e)
		{
			if (Data.visible != "Settings")
			{
				var foo = (Frame)FindName(Data.visible);
				DoubleAnimation da = new DoubleAnimation
				{
					From = 1.0,
					To = 0.0,
					Duration = new Duration(TimeSpan.FromSeconds(.25))
				};
				da.Completed += SettingsCompleted;
				var foo1 = (Frame)FindName("Settings");
				DoubleAnimation da1 = new DoubleAnimation
				{
					From = 0.0,
					To = 1.0,
					Duration = new Duration(TimeSpan.FromSeconds(.25))
				};
				foo.BeginAnimation(OpacityProperty, da);
				foo1.BeginAnimation(OpacityProperty, da1);
			}
		}

		private void SettingsCompleted(object sender, EventArgs e)
		{
			var foo = (Frame)FindName(Data.visible);
			Data.visible = "Settings";
			foo.Visibility = Visibility.Hidden;
			var foo1 = (Frame)FindName("Settings");
			foo1.Visibility = Visibility.Visible;
		}

		private void Load(object sender, RoutedEventArgs e)
		{
			DataContext = new WindowBlureffect(this, AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND) { BlurOpacity = 3 };
		}
	}
}