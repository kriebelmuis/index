using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using DoubleAnimation = System.Windows.Media.Animation.DoubleAnimation;
using Storyboard = System.Windows.Media.Animation.Storyboard;
using System.Windows.Media;

namespace Index
{
    public partial class Game : System.Windows.Controls.Page
    {
        private int id;

        public Game(int ID) 
        {
            InitializeComponent();
            
            id = ID;

            if (Data.games != null)
            {
                foreach (var game in Data.games)
                {
                    if (game.ID == id)
                    {
                        this.Dispatcher.Invoke(async () =>
                        {
                            GameName.Content = game.Name;
                            GameDesc.Text = game.Description;
                            GameVersion.Content = "Build " + game.LVersion;

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
                                    var response = await client.GetAsync(game.Images.Banners.B1);
                                    if (response is { StatusCode: HttpStatusCode.OK })
                                    {
                                        using var stream = await response.Content.ReadAsStreamAsync();
                                        await stream.CopyToAsync(memStream);
                                        memStream.Position = 0;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Error fetching game image! (" + game.Images.Banners.B1 + ")");
                                    }
                                }

                                MemoryStream ms = new MemoryStream();
                                (new Bitmap(memStream)).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                                BitmapImage image = new BitmapImage();
                                image.BeginInit();
                                ms.Seek(0, SeekOrigin.Begin);
                                image.StreamSource = ms;
                                image.EndInit();

                                this.gameImage1.Source = image;

<<<<<<< HEAD
                                DoubleAnimation da = new DoubleAnimation
                                {
                                    From = 0.0,
                                    To = 1.0,
                                    Duration = new Duration(TimeSpan.FromSeconds(.25))
                                };
                                gameImage1.BeginAnimation(OpacityProperty, da);
=======
                                DoubleAnimation myDoubleAnimation = new DoubleAnimation
                                {
                                    From = 0.0,
                                    To = 0.75,
                                    Duration = new Duration(TimeSpan.FromSeconds(.5))
                                };

                                Storyboard myStoryboard = new Storyboard();
                                myStoryboard.Children.Add(myDoubleAnimation);
                                Storyboard.SetTargetName(myDoubleAnimation, "anim");
                                Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(GradientStop.OffsetProperty));

                                DoubleAnimation myDoubleAnimation1 = new DoubleAnimation
                                {
                                    From = 0.25,
                                    To = 1.0,
                                    Duration = new Duration(TimeSpan.FromSeconds(.5))
                                };

                                Storyboard myStoryboard1 = new Storyboard();
                                myStoryboard1.Children.Add(myDoubleAnimation1);
                                Storyboard.SetTargetName(myDoubleAnimation1, "anim1");
                                Storyboard.SetTargetProperty(myDoubleAnimation1, new PropertyPath(GradientStop.OffsetProperty));

                                myStoryboard.Begin(this);
                                myStoryboard1.Begin(this);
>>>>>>> 8b1a1162fc34f25eaaa864a332174d5c350361d6
                            }
                            catch
                            {
                                var result = new Ping().Send("https://github.com/");

                                if (result.Status != IPStatus.Success)
                                {
                                    MessageBox.Show("Unreachable database error", "Index", MessageBoxButton.OK);
                                    Application.Current.Shutdown();
                                }
                                else
                                {
                                    if (NetworkInterface.GetIsNetworkAvailable())
                                    {
                                        MessageBox.Show("Unknown network error, please report at Discord", "Index", MessageBoxButton.OK);
                                        Application.Current.Shutdown();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Failed to connect to network", "Index", MessageBoxButton.OK);
                                        Application.Current.Shutdown();
                                    }
                                }
                            }
                        });
                        break;
                    }
                }
            }
        }

        private void Download(object sender, MouseButtonEventArgs e)
        {
<<<<<<< HEAD
            var exists = false;

            foreach (var game in Data.games)
            {
                if (game.ID == id)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                return;
            }

            var window = Application.Current.MainWindow;

            DoubleAnimation da = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(.25))
            };
            DoubleAnimation da1 = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(.25))
            };
            da1.Completed += DownloadCompleted;
            (window as Main).Game.BeginAnimation(OpacityProperty, da);
            (window as Main).Home.BeginAnimation(OpacityProperty, da);
            (window as Main).Downloads.Navigate(id, 0);
            (window as Main).Downloads.Visibility = Visibility.Visible;
            (window as Main).Downloads.BeginAnimation(OpacityProperty, da1);
        }

        private void DownloadCompleted(object sender, EventArgs e)
        {
            var window = Application.Current.MainWindow;
            (window as Main).Game.Visibility = Visibility.Hidden;
=======
            if (Data.games != null)
            {
                foreach (var game in Data.games)
                {
                    if (game.ID == id)
                    {
                        var window = Application.Current.MainWindow;

                        Downloads d = new Downloads(game.Infohash, id, 0);

                        (window as Main).Downloads.Navigate(d);
                        (window as Main).Game.Visibility = Visibility.Hidden;
                        (window as Main).Downloads.Visibility = Visibility.Visible;
                        break;
                    }
                }
            }
>>>>>>> 8b1a1162fc34f25eaaa864a332174d5c350361d6
        }

        private void Verify(object sender, MouseButtonEventArgs e)
        {
            var window = Application.Current.MainWindow;

<<<<<<< HEAD
            Downloads d = new Downloads(id, 1);
=======
            Downloads d = new Downloads(null, id, 1);
>>>>>>> 8b1a1162fc34f25eaaa864a332174d5c350361d6

            (window as Main).Downloads.Navigate(d);
            (window as Main).Game.Visibility = Visibility.Hidden;
            (window as Main).Downloads.Visibility = Visibility.Visible;
        }

        private void Uninstall(object sender, MouseButtonEventArgs e)
        {
            var window = Application.Current.MainWindow;

<<<<<<< HEAD
            Downloads d = new Downloads(id, 2);
=======
            Downloads d = new Downloads(null, id, 2);
>>>>>>> 8b1a1162fc34f25eaaa864a332174d5c350361d6

            (window as Main).Downloads.Navigate(d);
            (window as Main).Game.Visibility = Visibility.Hidden;
            (window as Main).Downloads.Visibility = Visibility.Visible;
        }

        private void goBackClick(object sender, MouseButtonEventArgs e)
        {
            var window = Application.Current.MainWindow;
<<<<<<< HEAD
            DoubleAnimation da = new DoubleAnimation
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
=======
>>>>>>> 8b1a1162fc34f25eaaa864a332174d5c350361d6
            (window as Main).Game.Visibility = Visibility.Hidden;
        }
    }
}