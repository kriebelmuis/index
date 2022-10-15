using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;

using SuRGeoNix.BitSwarmLib;

using System.IO;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Windows.Media;

namespace Index
{
    public partial class Downloads : Page
    {
        public static int dl = -1;

        public Downloads(string hash, int ID, int type)
        {
            InitializeComponent();

            if(type == 0)
            {
                if (ID != 0 && hash != null)
                {
                    if (string.IsNullOrWhiteSpace(Properties.Settings.Default.Directory) == false)
                    {
                        Dispatcher.Invoke(async () =>
                        {
                            foreach (var b in Data.dls)
                            {
                                if (b == false)
                                {
                                    dl += 1;
                                    Data.dls[dl] = true;
                                    break;
                                }
                            }

                            if (dl == 0)
                            {
                                MessageBox.Show("Maximum of 3 downloads reached", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            Data.bitSwarm.StatsUpdated += torrentUpdated;
                            Data.bitSwarm.StatusChanged += torrentChanged;

                            Canvas obj = (Canvas)FindName("Download" + dl);

                            if (Data.games != null)
                            {
                                for (var i = 0; i < Data.games.Count; i++)
                                {
                                    if (Data.games[i].ID == ID)
                                    {
                                        await Dispatcher.Invoke(async () =>
                                        {
                                            MessageBox.Show(Data.games[i].Images.Banners.B1);
                                            var info = Data.games[i];

                                            MessageBox.Show(dl.ToString());

                                            Label gameName = (Label)obj.FindName("gameName" + dl);
                                            gameName.Content = info.Name;

                                            Directory.CreateDirectory(Path.Combine(Properties.Settings.Default.Directory, @"\", info.Name, @"\Download"));

                                            Data.bitSwarm.Options.FolderComplete = Path.Combine(Properties.Settings.Default.Directory, @"\", info.Name, @"\Download");
                                            Data.bitSwarm.Options.FolderIncomplete = Path.Combine(Properties.Settings.Default.Directory, @"\", info.Name, @"\Download");
                                            Data.bitSwarm.Options.FolderSessions = Path.Combine(Properties.Settings.Default.Directory, @"\", info.Name, @"\Download");
                                            Data.bitSwarm.Options.FolderTorrents = Path.Combine(Properties.Settings.Default.Directory, @"\", info.Name, @"\Download");

                                            MemoryStream memStream = new();

                                            using (HttpClient client = new())
                                            {
                                                var response = await client.GetAsync(Data.games[i].Images.Banners.B1);
                                                if (response != null && response.StatusCode == HttpStatusCode.OK)
                                                {
                                                    using var stream = await response.Content.ReadAsStreamAsync();
                                                    await stream.CopyToAsync(memStream);
                                                    memStream.Position = 0;
                                                }
                                            }

                                            MemoryStream ms = new();
                                            new Bitmap(memStream).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                                            BitmapImage image = new();
                                            image.BeginInit();
                                            ms.Seek(0, SeekOrigin.Begin);
                                            image.StreamSource = ms;
                                            image.EndInit();

                                            ImageBrush img = (ImageBrush)obj.FindName("image" + dl);
                                            img.ImageSource = image;
                                        });
                                    }
                                }
                            }

                            obj.Opacity = 100;

                            dl = 0;
                            Data.bitSwarm.Open(hash);
                            Data.bitSwarm.Start();
                        });
                    }
                }
            }
            if(type == 1)
            {
                MessageBox.Show("verify");
            }
            if (type == 2)
            {
                if (ID != 0 && hash == null)
                {
                    if (string.IsNullOrWhiteSpace(Properties.Settings.Default.Directory) == false)
                    {
                        Dispatcher.Invoke(async () =>
                        {
                            var dl = -1;

                            foreach (var b in Data.dls)
                            {
                                if (b == false)
                                {
                                    dl += 1;
                                    Data.dls[dl] = true;
                                }
                            }

                            if (dl == -1)
                            {
                                MessageBox.Show("Maximum of 3 downloads reached", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            Data.bitSwarm.StatsUpdated += torrentUpdated;
                            Data.bitSwarm.StatusChanged += torrentChanged;

                            Canvas obj = (Canvas)FindName("Download" + dl);

                            if (Data.games != null)
                            {
                                for (var i = 0; i < Data.games.Count; i++)
                                {
                                    if (Data.games[i].ID == ID)
                                    {
                                        await Dispatcher.Invoke(async () =>
                                        {
                                            MessageBox.Show(Data.games[i].Images.Banners.B1);
                                            var info = Data.games[i];

                                            MessageBox.Show(dl.ToString());

                                            Label gameName = (Label)obj.FindName("gameName" + dl);
                                            gameName.Content = info.Name;

                                            MemoryStream memStream = new();

                                            using (HttpClient client = new())
                                            {
                                                var response = await client.GetAsync(Data.games[i].Images.Banners.B1);
                                                if (response != null && response.StatusCode == HttpStatusCode.OK)
                                                {
                                                    using var stream = await response.Content.ReadAsStreamAsync();
                                                    await stream.CopyToAsync(memStream);
                                                    memStream.Position = 0;
                                                }
                                            }

                                            MemoryStream ms = new();
                                            new Bitmap(memStream).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                                            BitmapImage image = new();
                                            image.BeginInit();
                                            ms.Seek(0, SeekOrigin.Begin);
                                            image.StreamSource = ms;
                                            image.EndInit();

                                            ImageBrush img = (ImageBrush)obj.FindName("image" + dl);
                                            img.ImageSource = image;

                                            gameInfo1.Content = "Uninstalling";

                                            obj.Opacity = 100;

                                            dl = 0;

                                            if (Process.GetProcessesByName(Data.games[i].Filename).Length > 0)
                                            {
                                                Process[] processes = Process.GetProcesses();
                                                for (var p = 0; i < processes.Length; i++)
                                                {
                                                    if (processes[p].ProcessName == Data.games[i].Filename)
                                                    {
                                                        processes[p].Kill();
                                                    }
                                                }
                                            }

                                            await Task.Factory.StartNew(path => Directory.Delete((string)path, true), Path.Combine(Properties.Settings.Default.Directory, @"\", info.Name));

                                            obj.Opacity = 0;
                                        });
                                    }
                                }
                            }
                        });
                    }
                }
            }
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
            Data.bitSwarm.Dispose();
        }

        private void torrentChanged(object source, BitSwarm.StatusChangedArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.Status == 0)
                {
                    gameInfo1.Content = "Extracting";
                    downloadProcentage1.Visibility = Visibility.Hidden;
                    downloadEta1.Visibility = Visibility.Hidden;
                    downloadSpeed1.Visibility = Visibility.Hidden;
                    progressBar1.IsIndeterminate = true;
                }
                if (e.Status == 1)
                {
                    gameInfo1.Content = "Stopped";
                    downloadProcentage1.Visibility = Visibility.Hidden;
                    progressBar1.Value = 0;
                }
                if (e.Status == 2)
                {
                    gameInfo1.Content = "Error";
                    downloadProcentage1.Visibility = Visibility.Hidden;
                    progressBar1.Value = 0;
                }
            });
        }

        private void torrentUpdated(object source, BitSwarm.StatsUpdatedArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var window = Application.Current.MainWindow;

                (window as Main).Taskbar.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;

                downloadEta1.Content = TimeSpan.FromSeconds((e.Stats.ETA + e.Stats.AvgETA) / 2).ToString(@"mm\:ss") + " left";
                downloadProcentage1.Content = e.Stats.Progress + "%";
                downloadSpeed1.Content = FormatBytes(e.Stats.DownRate) + "/s";
                progressBar1.IsIndeterminate = false;
                progressBar1.Value = e.Stats.Progress;

                (window as Main).Taskbar.ProgressValue = decimal.ToDouble((decimal)e.Stats.Progress / 100);

                gameInfo1.Content = "Downloading";

                progressBar1.Visibility = Visibility.Visible;
                downloadProcentage1.Visibility = Visibility.Visible;
                downloadEta1.Visibility = Visibility.Visible;
                downloadSpeed1.Visibility = Visibility.Visible;
            });
        }

        private void pauseDownload(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (Data.paused[0] == true)
                {
                    Data.paused[0] = false;
                    pause.Glyph = "&#xe768;";
                    Data.bitSwarm.Start();
                }
                else
                {
                    Data.paused[0] = true;
                    pause.Glyph = "&#xe71a;";
                    Data.bitSwarm.Pause();
                }
            });
        }

        private void stopDownload(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Data.bitSwarm.Dispose();
            Download1.Visibility = Visibility.Hidden;
        }
    }
}