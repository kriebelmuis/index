using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Index
{
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
            _ = boot();
        }

        private async Task boot()
        {
            await Refresh(1, game1);

            var count = 2;
            foreach (var game in Data.games)
            {
                Canvas cnvs = (Canvas)XamlReader.Load(XmlReader.Create(new StringReader(XamlWriter.Save(game1))));
                cnvs.Name = $"game{count}";
                Games.Children.Add(cnvs);

                cnvs.PreviewMouseLeftButtonUp += gameClick;

                await Refresh(count, cnvs);

                count++;
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

        private async Task Refresh(int id, FrameworkElement cnvs)
        {
            foreach (var game in Data.games)
            {
                foreach (Canvas c in Games.Children)
                {
                    if (game.ID == id)
                    {
                        await Dispatcher.Invoke(async () =>
                        {
                            Label text = (Label)cnvs.FindName("gameName");
                            text.Content = game.Name;

                            Label size = (Label)cnvs.FindName("gameSize");
                            size.Content = "0 B";

                            try
                            {
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.UriSource = new Uri(game.Images.Banners.B1);
                                bitmap.EndInit();

                                System.Windows.Controls.Image img = (System.Windows.Controls.Image)cnvs.FindName("image");
                                img.Source = bitmap;

                                DoubleAnimation da = new DoubleAnimation
                                {
                                    From = 0.0,
                                    To = 1.0,
                                    Duration = new Duration(TimeSpan.FromSeconds(.25))
                                };
                                cnvs.BeginAnimation(OpacityProperty, da);
                            }
                            catch
                            {
                                MessageBox.Show("Error while loading image (" + id + ")", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        });
                    }
                }
            }
        }

        private void gameClick(object sender, MouseButtonEventArgs e)
        {
            var window = Application.Current.MainWindow;

            Game g;

            FrameworkElement element = (FrameworkElement)sender;

            var val = Regex.Match(element.Name, @"\d+").Value;

            g = string.IsNullOrWhiteSpace(Regex.Match(element.Name, @"\d+").Value) ? new Game(1) : new Game(int.Parse(val));

            (window as Main).Game.Navigate(g);

            (window as Main).Game.Visibility = Visibility.Visible;
            DoubleAnimation da = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromSeconds(.25))
            };
            (window as Main).Game.BeginAnimation(OpacityProperty, da);
        }
    }
}