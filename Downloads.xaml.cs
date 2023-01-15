using System;
<<<<<<< HEAD
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Data;
using ModernWpf;
using System.Windows.Media;

namespace Index
{
	public partial class Downloads : System.Windows.Controls.Page
	{
		public static int dl = -1;

		public static DataTable dltable = new();

		public Downloads()
		{
			InitializeComponent();

            Data.dltable.TableNewRow += DLNewRow;

        }

        private void DLNewRow(object sender, DataTableNewRowEventArgs e)
		{
			var dlid = e.Row[(int)DownloadTable.DLID];

			if (dlid != null)
			{
				var canvas = Window.FindDescendantByName($"Download{dlid}");

				var bit = new BitmapImage();
				bit.BeginInit();
				bit.UriSource = new Uri(e.Row[(int)DownloadTable.Banner].ToString());
				bit.EndInit();

				var bg = canvas.FindDescendant<ImageBrush>();
				var name = (Label)canvas.FindDescendantByName($"Name{dlid}");
                var proctext = (Label)canvas.FindDescendantByName($"Procentage{dlid}");
                var bar = (ProgressBar)canvas.FindDescendantByName($"Bar{dlid}");

                if (bg.ImageSource != bit)
				{
					bg.ImageSource = bit;
				}

				if ((string)name.Content != e.Row[(int)DownloadTable.Name].ToString())
				{
					name.Content = e.Row[(int)DownloadTable.Name].ToString();
				}

                if ((string)proctext.Content != e.Row[(int)DownloadTable.Procentage].ToString())
                {
                    proctext.Content = e.Row[(int)DownloadTable.Name].ToString();
                    bar.Value = int.Parse(e.Row[(int)DownloadTable.Name].ToString());
                }
			}
			else
			{
				var row = e.Row;

				var canvas = (Canvas)FindName($"Download{dlid}");
				var da = new DoubleAnimation
				{
					From = 0.0,
					To = 1.0,
					Duration = new Duration(TimeSpan.FromSeconds(.25))
				};
				da.Completed += (sender, e) => DLCompleted(sender, e, int.Parse(row[(int)DownloadTable.ID].ToString()));
				Download0.BeginAnimation(OpacityProperty, da);
				Data.bitswarm[int.Parse(row[(int)DownloadTable.ID].ToString())].Dispose();
			}
		}

		private void DLCompleted(object sender, EventArgs e, int id)
		{
			var canvas = (FrameworkElement)FindName($"Download{id}");
			canvas.Visibility = Visibility.Hidden;
		}

		private void PauseDownload(object sender, MouseButtonEventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				var element = (FrameworkElement)sender;
				var id = Int16.Parse(Regex.Match(element.Name, @"\d+").Value);

				var row = Data.dltable.Select($"id = '{id}'")[0];

				var pause = (ModernWpf.Controls.FontIcon)FindName($"Pause{row[(int)DownloadTable.DLID]}");

				if (bool.Parse(row[(int)DownloadTable.Paused].ToString()) == true)
				{
					row[(int)DownloadTable.Paused] = false;
					pause.Glyph = "&#xe768;";
					Data.bitswarm[int.Parse(row[(int)DownloadTable.ID].ToString())].Start();
				}
				else
				{
					row[(int)DownloadTable.Paused] = true;
					pause.Glyph = "&#xe71a;";
					Data.bitswarm[int.Parse(row[(int)DownloadTable.ID].ToString())].Pause();
				}
			});
		}

		private void StopDownload(object sender, MouseButtonEventArgs e)
		{
			var element = (FrameworkElement)sender;
			var id = Int16.Parse(Regex.Match(element.Name, @"\d+").Value);

			var row = Data.dltable.Select($"id = `{id}`")[0];

			var da = new DoubleAnimation
			{
				From = 0.0,
				To = 1.0,
				Duration = new Duration(TimeSpan.FromSeconds(.25))
			};

			da.Completed += (sender, e) => StopCompleted(sender, e, id);
			Download0.BeginAnimation(OpacityProperty, da);
			Data.bitswarm[int.Parse(row[(int)DownloadTable.ID].ToString())].Dispose();
		}

		private void StopCompleted(object sender, EventArgs e, int id)
		{
			var canvas = (FrameworkElement)FindName($"Download{id}");
			canvas.Visibility = Visibility.Hidden;
		}
	}
}