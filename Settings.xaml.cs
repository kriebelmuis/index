using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Index
{
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();

            this.Dispatcher.Invoke(() =>
            {
                if (Properties.Settings.Default.Theme == "Dark")
                {
                    this.Dark.IsSelected = true;
                    this.Light.IsSelected = false;
                    Wpf.Ui.Appearance.Accent.Apply(SystemParameters.WindowGlassColor, Wpf.Ui.Appearance.ThemeType.Dark, true);
                }
                if (Properties.Settings.Default.Theme == "Light")
                {
                    this.Light.IsSelected = true;
                    this.Dark.IsSelected = false;
                    Wpf.Ui.Appearance.Accent.Apply(SystemParameters.WindowGlassColor, Wpf.Ui.Appearance.ThemeType.Light, true);
                }

                if (string.IsNullOrWhiteSpace(Properties.Settings.Default.Directory))
                {
                    this.DirectoryText.Content = "Not set";
                }
                else
                {
                    this.DirectoryText.Content = Properties.Settings.Default.Directory;
                }

                if (string.IsNullOrWhiteSpace(Properties.Settings.Default.Throttle.ToString()) == false && Properties.Settings.Default.Throttle != 0)
                {
                    throttletoggle.IsEnabled = true;
                    throttletoggle.IsChecked = true;
                    throttlebox.Text = Properties.Settings.Default.Throttle.ToString();
                }
            });
        }

        private void SetFolder(object sender, MouseButtonEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                using var fbd = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    this.DirectoryText.Content = fbd.SelectedPath;

                    Properties.Settings.Default.Directory = fbd.SelectedPath;
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Reload();
                }
            });
        }

        private void ThemeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Theme.SelectedIndex != -1)
            {
                if (Theme.SelectedIndex == 0)
                {
                    Properties.Settings.Default.Theme = "Dark";
                    Wpf.Ui.Appearance.Accent.Apply(SystemParameters.WindowGlassColor, Wpf.Ui.Appearance.ThemeType.Dark, true);
                }
                if (Theme.SelectedIndex == 1)
                {
                    Properties.Settings.Default.Theme = "Light";
                    Wpf.Ui.Appearance.Accent.Apply(SystemParameters.WindowGlassColor, Wpf.Ui.Appearance.ThemeType.Light, true);
                }

                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            }
        }

        private void ThrottleToggled(object sender, RoutedEventArgs e)
        {
            throttlebox.IsEnabled = true;
        }

        private void ThrottleUntoggled(object sender, RoutedEventArgs e)
        {
            throttlebox.IsEnabled = false;
            throttlebox.Text = "";
            Properties.Settings.Default.Throttle = 0;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        private void ThrottleChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                float flt;

                if (float.TryParse(throttlebox.Text, out flt))
                {
                    Properties.Settings.Default.Throttle = float.Parse(throttlebox.Text);
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Reload();
                }
            }
            catch
            {
                MessageBox.Show("Couldn't throttle download", "Index");
            }
        }
    }
}