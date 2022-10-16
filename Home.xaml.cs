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
            if (Data.games != null)
            {
                await Refresh(1, game1);
                var count = 1;
                foreach (var game in Data.games)
                {
                    count++;

                    Canvas cnvs = (Canvas)XamlReader.Load(XmlReader.Create(new StringReader(XamlWriter.Save(game1))));
                    cnvs.Name = "game" + count.ToString();
                    Games.Children.Add(cnvs);

                    cnvs.PreviewMouseLeftButtonUp += gameClick;

                    await Refresh(count, cnvs);
                }
                Games.Visibility = Visibility.Visible;
                scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
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
            if (Data.games != null)
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
                                    }

                                    MemoryStream ms = new();
                                    (new Bitmap(memStream)).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                                    BitmapImage image = new();
                                    image.BeginInit();
                                    ms.Seek(0, SeekOrigin.Begin);
                                    image.StreamSource = ms;
                                    image.EndInit();

                                    System.Windows.Controls.Image img = (System.Windows.Controls.Image)cnvs.FindName("image");
                                    img.Source = image;
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
        }
    }
}