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
        private bool trig = false;

        public Library()
        {
            InitializeComponent();

            if (!trig)
            {
                trig = true;
                _ = Boot();
            }
        }

        private async Task Boot()
        {
            if (Properties.Settings.Default.Installed.Any())
            {
                var count = 1;
                foreach (var game in Properties.Settings.Default.Installed)
                {
                    count++;

                    Canvas cnvs = (Canvas)XamlReader.Load(XmlReader.Create(new StringReader(XamlWriter.Save(game1))));
                    cnvs.Name = "game" + count.ToString();
                    Games.Children.Add(cnvs);

                    cnvs.PreviewMouseLeftButtonUp += Game;

                    await Refresh(count, cnvs);
                }
                Games.Visibility = Visibility.Visible;
                scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            }
        }

        private async Task Refresh(int id, FrameworkElement cnvs)
        {
            foreach (Canvas c in Games.Children)
            {
                await Dispatcher.Invoke(async () =>
                {
                    var game = Data.games[id];

                    if (game == null)
                    {
                        MessageBox.Show("Invalid gameid", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    ((Label)cnvs.FindName("gameName")).Content = game.Metadata.Name;
                    ((Label)cnvs.FindName("gameSize")).Content = "0 B";

                    try
                    {
                        var memStream = new MemoryStream();

                        using (var client = new HttpClient())
                        {
                            var response = await client.GetAsync(game.Images.Banners[0]);
                            if (response != null && response.StatusCode == HttpStatusCode.OK)
                            {
                                using var stream = await response.Content.ReadAsStreamAsync();
                                await stream.CopyToAsync(memStream);
                                memStream.Position = 0;
                            }
                        }

                        MemoryStream ms = new();
                        (new Bitmap(memStream)).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        BitmapImage img = new();
                        img.BeginInit();
                        ms.Seek(0, SeekOrigin.Begin);
                        img.StreamSource = ms;
                        img.EndInit();

                        System.Windows.Controls.Image im = (System.Windows.Controls.Image)cnvs.FindName("image");
                        im.Source = img;
                    }
                    catch
                    {
                        MessageBox.Show("Error while loading image (" + id + ")", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
        }

        private void Game(object sender, MouseButtonEventArgs e)
        {
            // Launch
        }
    }
}