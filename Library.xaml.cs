using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Index
{
    public partial class Library : Page
    {
        public Library()
        {
            InitializeComponent();
            _ = boot();
        }

        private async Task boot()
        {
            if (Data.games != null)
            {
                if (Properties.Settings.Default.Installed.Count() > 0)
                {
                    var count = 1;
                    for (var i = 1; i < Properties.Settings.Default.Installed.Count(); i++)
                    {
                        count++;

                        Canvas cnvs = (Canvas)XamlReader.Load(XmlReader.Create(new StringReader(XamlWriter.Save(game1))));
                        cnvs.Name = "game" + count.ToString();
                        Games.Children.Add(cnvs);

                        cnvs.PreviewMouseLeftButtonUp += gameClick;

                        await refresh(count, cnvs);
                    }
                    Games.Visibility = Visibility.Visible;
                    scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                }
                else
                {
                    MessageBox.Show("No games?");
                }
            }
            else
            {
                Methods.CheckConnection();
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

        private async Task refresh(int id, FrameworkElement cnvs)
        {
            if (Data.games != null && Data.games.Count > 0)
            {
                for (var i = 0; i < Data.games.Count; i++)
                {
                    foreach (Canvas c in Games.Children)
                    {
                        if (Data.games[i].ID == id)
                        {
                            await Dispatcher.Invoke(async () =>
                            {
                                Label text = (Label)cnvs.FindName("gameName");
                                text.Content = Data.games[i].Name;

                                Label size = (Label)cnvs.FindName("gameSize");
                                size.Content = "0 B";

                                try
                                {
                                    var memStream = new MemoryStream();

                                    using (var client = new HttpClient())
                                    {
                                        var response = await client.GetAsync(Data.games[i].Images.Banners.B1);
                                        if (response != null && response.StatusCode == HttpStatusCode.OK)
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

                                    System.Windows.Controls.Image img = (System.Windows.Controls.Image)cnvs.FindName("image");
                                    img.Source = image;
                                }
                                catch
                                {
                                    MessageBox.Show("Error while loading image", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            });
                        }
                    }
                }
            }
            else
            {
                Methods.CheckConnection();
            }
        }

        private void gameClick(object sender, MouseButtonEventArgs e)
        {
            var window = Application.Current.MainWindow;

            Game g;

            FrameworkElement element = (FrameworkElement)sender;

            var val = Regex.Match(element.Name, @"\d+").Value;

            if (string.IsNullOrWhiteSpace(Regex.Match(element.Name, @"\d+").Value))
            {
                g = new Game(1);
            }
            else
            {
                g = new Game(int.Parse(val));
            }

            (window as Main).Game.Navigate(g);
            (window as Main).Game.Visibility = Visibility.Visible;
        }
    }
}