using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Security.Cryptography;

using SharpCompress.Archives;
using SharpCompress.Common;

using SuRGeoNix.BitSwarmLib;

using Newtonsoft.Json;

namespace SignalX
{
    public partial class Downloads : Page
    {
        public Downloads()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Signal X", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }

        static List<GameData> GetData()
        {
            WebClient wc = new WebClient();

            string json = wc.DownloadString(new Uri("https://pastebin.com/raw/N3G9KaKt"));

            if (!string.IsNullOrWhiteSpace(json))
            {
                var gamedata = JsonConvert.DeserializeObject<List<GameData>>(json);

                return gamedata;
            }

            return null;
        }

        private static string FormatBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;

            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return String.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        private void downloadClosed(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.MainWindow;
            
            data.bitSwarm.Dispose();
        }

        private void torrentUpdated(object source, BitSwarm.StatsUpdatedArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var window = Application.Current.MainWindow;

                (window as Main).Taskbar.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;

                this.downloadEta1.Content = TimeSpan.FromSeconds((e.Stats.ETA + e.Stats.AvgETA) / 2).ToString(@"hh\:mm\:ss") + " left";
                this.downloadProcentage1.Content = e.Stats.Progress + "%";
                this.downloadSpeed1.Content = FormatBytes(e.Stats.DownRate) + "/s";
                this.progressBar.IsIndeterminate = false;
                this.progressBar.Value = e.Stats.Progress;

                (window as Main).Taskbar.ProgressValue = decimal.ToDouble((decimal)e.Stats.Progress / 100);

                this.gameInfo1.Content = "Downloading";

                this.progressBar.Visibility = Visibility.Visible;
                this.downloadProcentage1.Visibility = Visibility.Visible;
                this.downloadEta1.Visibility = Visibility.Visible;
                this.downloadSpeed1.Visibility = Visibility.Visible;
            });
        }

        private void pauseDownload(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (data.paused == true)
                {
                    data.bitSwarm.Start();
                    this.pause.Source = new Uri(@"pack://application:,,,/Resources/pause.svg");
                    data.paused = false;
                }
                else
                {
                    data.bitSwarm.Pause();
                    this.pause.Source = new Uri(@"pack://application:,,,/Resources/start.svg");
                    data.paused = true;
                }
            });
        }

        private void stopDownload(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            data.bitSwarm.Dispose();
            this.Download1.Visibility = Visibility.Hidden;
        }
    }
}