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
using System.Windows.Media.Animation;
using Windows.Gaming.Input;

namespace Index
{
	public partial class Downloads : Page
	{
		public static int dl = -1;

		public Downloads(int gameID, int type)
		{
			InitializeComponent();

            if (gameID == 0)
            {
                MessageBox.Show("Invalid game ID or game hash", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.Directory))
            {
                MessageBox.Show("Default download directory is not set", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            dl = -1;

            foreach (var b in Data.dls)
            {
                if (b == false)
                {
                    dl += 1;
                    Data.dls[dl] = true;
                    break;
                }
            }

            if (dl == -1)
            {
                MessageBox.Show("Maximum of 3 downloads reached", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Canvas obj = (Canvas)FindName("Download" + dl);

            if (type == 0)
			{
                Dispatcher.Invoke(async () =>
				{
					Data.bitSwarm.StatsUpdated += torrentUpdated;
					Data.bitSwarm.StatusChanged += torrentChanged;

					foreach (var game in Data.games)
					{
						if (game.ID == gameID)
						{
							await Dispatcher.Invoke(async () =>
							{
								if (Directory.Exists(Properties.Settings.Default.Directory + @"/" + game.Name))
								{
									var result = MessageBox.Show("A folder already exists with the game name " + game.Name + " are you sure you want to remove the contents?", "Index", MessageBoxButton.YesNo, MessageBoxImage.Question);
								}

								Label gameName = (Label)obj.FindName("gameName" + dl);
								gameName.Content = game.Name;

								Directory.CreateDirectory(Path.Combine(Properties.Settings.Default.Directory, @"\", game.Name, @"\Download"));

								Data.bitSwarm.Options.FolderComplete = Path.Combine(Properties.Settings.Default.Directory, @"\", game.Name, @"\Download");
								Data.bitSwarm.Options.FolderIncomplete = Path.Combine(Properties.Settings.Default.Directory, @"\", game.Name, @"\Download");
								Data.bitSwarm.Options.FolderSessions = Path.Combine(Properties.Settings.Default.Directory, @"\", game.Name, @"\Download");
								Data.bitSwarm.Options.FolderTorrents = Path.Combine(Properties.Settings.Default.Directory, @"\", game.Name, @"\Download");

								MemoryStream memStream = new();

								using (HttpClient client = new())
								{
									var response = await client.GetAsync(game.Images.Banners.B1);
									if (response is { StatusCode: HttpStatusCode.OK })
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

                                DoubleAnimation da = new DoubleAnimation
                                {
                                    From = 0.0,
                                    To = 1.0,
                                    Duration = new Duration(TimeSpan.FromSeconds(.25))
                                };
                                obj.BeginAnimation(OpacityProperty, da);

                                Data.bitSwarm.Open(game.Infohash);
                                Data.bitSwarm.Start();
                            });
						}
					}
				});
			}
			if(type == 1)
			{
				
			}
			if (type == 2)
			{
				Dispatcher.Invoke(async () =>
				{
					foreach (var game in Data.games)
					{
						if (game.ID == gameID)
						{
							await Dispatcher.Invoke(async () =>
							{
								Label gameName = (Label)obj.FindName("gameName" + dl);
								gameName.Content = game.Name;

								MemoryStream memStream = new();

								using (HttpClient client = new())
								{
									var response = await client.GetAsync(game.Images.Banners.B1);
									if (response is { StatusCode: HttpStatusCode.OK })
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

								gameInfo0.Content = "Uninstalling";

								DoubleAnimation da = new DoubleAnimation
								{
									From = 0.0,
									To = 1.0,
									Duration = new Duration(TimeSpan.FromSeconds(.25))
								};
								obj.BeginAnimation(OpacityProperty, da);

								if (Process.GetProcessesByName(game.Filename).Length > 0)
								{
									Process[] processes = Process.GetProcesses();
									int i = 0;
									for (var p = 0; i < processes.Length; i++)
									{
										if (processes[p].ProcessName == Data.games[i].Filename)
										{
											processes[p].Kill();
										}
									}
								}

								await Task.Factory.StartNew(path => Directory.Delete((string)path, true), Path.Combine(Properties.Settings.Default.Directory, @"\", game.Name));

								DoubleAnimation da1 = new DoubleAnimation
								{
									From = 1.0,
									To = 0.0,
									Duration = new Duration(TimeSpan.FromSeconds(.25))
								};
								obj.BeginAnimation(OpacityProperty, da1);
							});
						}
					}
				});
			}
		}

		private static string FormatBytes(long bytes)
		{
			string[] suffix = { "B", "KB", "MB", "GB", "TB" };
			int i;
			double dblSByte = bytes;

			for (i = 0; i < suffix.Length && bytes >= 1024; i++, bytes /= 1024)
			{
				dblSByte = bytes / 1024.0;
			}

			return String.Format("{0:0.##} {1}", dblSByte, suffix[i]);
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
					gameInfo0.Content = "Extracting";
					downloadProcentage0.Visibility = Visibility.Hidden;
					downloadEta0.Visibility = Visibility.Hidden;
					downloadSpeed0.Visibility = Visibility.Hidden;
					progressBar0.IsIndeterminate = true;
				}
				if (e.Status == 1)
				{
					gameInfo0.Content = "Stopped";
					downloadProcentage0.Visibility = Visibility.Hidden;
					progressBar0.Value = 0;
				}
				if (e.Status == 2)
				{
					gameInfo0.Content = "Error";
					downloadProcentage0.Visibility = Visibility.Hidden;
					progressBar0.Value = 0;
				}
			});
		}

		private void torrentUpdated(object source, BitSwarm.StatsUpdatedArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				var window = Application.Current.MainWindow;

				(window as Main).Taskbar.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;

				downloadEta0.Content = TimeSpan.FromSeconds((e.Stats.ETA + e.Stats.AvgETA) / 2).ToString(@"mm\:ss") + " left";
				downloadProcentage0.Content = e.Stats.Progress + "%";
				downloadSpeed0.Content = FormatBytes(e.Stats.DownRate) + "/s";
				progressBar0.IsIndeterminate = false;
				progressBar0.Value = e.Stats.Progress;

				(window as Main).Taskbar.ProgressValue = decimal.ToDouble((decimal)e.Stats.Progress / 100);

				gameInfo0.Content = "Downloading";

				progressBar0.Visibility = Visibility.Visible;
				downloadProcentage0.Visibility = Visibility.Visible;
				downloadEta0.Visibility = Visibility.Visible;
				downloadSpeed0.Visibility = Visibility.Visible;
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
			DoubleAnimation da = new DoubleAnimation
			{
				From = 0.0,
				To = 1.0,
				Duration = new Duration(TimeSpan.FromSeconds(.25))
			};
			da.Completed += StopCompleted;
			Download0.BeginAnimation(OpacityProperty, da);
			Data.bitSwarm.Dispose();
		}

		private void StopCompleted(object sender, EventArgs e)
		{
			Download0.Visibility = Visibility.Hidden;
		}
	}
}