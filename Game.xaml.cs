using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Windows.Media.Animation;

namespace Index
{
    public partial class Game : System.Windows.Controls.Page
    {
        public int id;

        public Game(Int16 gameID)
        {
            InitializeComponent();

            GameData game = Data.games[gameID];

            if (game == null)
            {
                MessageBox.Show($"Game not found {gameID}");
                return;
            }

            id = gameID;

            Dispatcher.Invoke(async () =>
            {
                GameName.Content = game.Metadata.Name;
                GameDesc.Text = game.Metadata.Description;
                GameVersion.Content = $"Build {game.Download.LatestVersion}";

                if (!Properties.Settings.Default.Installed.Contains(id))
                {
                    uninstallButton.Visibility = Visibility.Hidden;
                    verifyButton.Visibility = Visibility.Hidden;
                }

                try
                {
                    var memStream = new MemoryStream();

                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(game.Images.Banners[0]);
                        if (response is { StatusCode: HttpStatusCode.OK })
                        {
                            using var stream = await response.Content.ReadAsStreamAsync();
                            await stream.CopyToAsync(memStream);
                            memStream.Position = 0;
                        }
                    }

                    MemoryStream ms = new MemoryStream();
                    (new Bitmap(memStream)).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    ms.Seek(0, SeekOrigin.Begin);
                    image.StreamSource = ms;
                    image.EndInit();

                    gameImage1.Source = image;

                    DoubleAnimation da = new DoubleAnimation
                    {
                        From = 0.0,
                        To = 1.0,
                        Duration = new Duration(TimeSpan.FromSeconds(.25))
                    };
                    gameImage1.BeginAnimation(OpacityProperty, da);
                }
                catch
                {
                    Methods.CheckConnection("Game background image");
                }
            });
        }

        private void Download(object sender, MouseButtonEventArgs e)
        {
            var window = Application.Current.MainWindow;

            Methods.Download(id);

            Data.visible = "Downloads";

            var da = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(.25))
            };
            var da1 = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(.25))
            };
            da1.Completed += DownloadCompleted;
            (window as Main).Game.BeginAnimation(OpacityProperty, da);
            (window as Main).Home.BeginAnimation(OpacityProperty, da);
            (window as Main).Downloads.Navigate(new Downloads());
            (window as Main).Downloads.Visibility = Visibility.Visible;
            (window as Main).Downloads.BeginAnimation(OpacityProperty, da1);
        }

        private void Verify(object sender, MouseButtonEventArgs e)
        {
            var window = Application.Current.MainWindow;

            Methods.Verify(id);

            Data.visible = "Downloads";

            var da = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(.25))
            };
            var da1 = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(.25))
            };
            da1.Completed += DownloadCompleted;
            (window as Main).Game.BeginAnimation(OpacityProperty, da);
            (window as Main).Home.BeginAnimation(OpacityProperty, da);
            (window as Main).Downloads.Navigate(new Downloads());
            (window as Main).Downloads.Visibility = Visibility.Visible;
            (window as Main).Downloads.BeginAnimation(OpacityProperty, da1);
        }

        private void Uninstall(object sender, MouseButtonEventArgs e)
        {
            var window = Application.Current.MainWindow;

            Methods.Uninstall(id);

            Data.visible = "Downloads";

            Downloads d = new Downloads();

            var da = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(.25))
            };
            var da1 = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(.25))
            };
            da1.Completed += DownloadCompleted;
            (window as Main).Game.BeginAnimation(OpacityProperty, da);
            (window as Main).Home.BeginAnimation(OpacityProperty, da);
            (window as Main).Downloads.Navigate(new Downloads());
            (window as Main).Downloads.Visibility = Visibility.Visible;
            (window as Main).Downloads.BeginAnimation(OpacityProperty, da1);
        }

        private void DownloadCompleted(object sender, EventArgs e)
        {
            var window = Application.Current.MainWindow;
            (window as Main).Game.Visibility = Visibility.Hidden;
        }

        private void goBackClick(object sender, MouseButtonEventArgs e)
        {
            var window = Application.Current.MainWindow;
            var da = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(.25))
            };
            da.Completed += Completed;
            (window as Main).Game.BeginAnimation(OpacityProperty, da);
        }

        private void Completed(object sender, EventArgs e)
        {
            var window = Application.Current.MainWindow;
            (window as Main).Game.Visibility = Visibility.Hidden;
        }
    }
}