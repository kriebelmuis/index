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
            _ = Boot();
        }

        private async Task Boot()
        {
            if(!trig)
            {
                trig = true;
                if (Data.games != null)
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
                else
                {
                    Methods.CheckConnection();
                }
            }
        }

        private async Task Refresh(int id, FrameworkElement cnvs)
        {
            if (Data.games != null && Data.games.Count > 0)
            {
                foreach (var game in Data.games)
                {
                    foreach (Canvas c in Games.Children)
                    {
                        if (game.ID == id)
                        {
                            await Dispatcher.Invoke(async () =>
                            {
                                ((Label)cnvs.FindName("gameName")).Content = game.Name;
                                ((Label)cnvs.FindName("gameSize")).Content = "0 B";

                                try
                                {
                                    var memStream = new MemoryStream();

                                    using (var client = new HttpClient())
                                    {
                                        var response = await client.GetAsync(game.Images.Banners.B1);
                                        if (response != null && response.StatusCode == HttpStatusCode.OK)
                                        {
                                            using var stream = await response.Content.ReadAsStreamAsync();
                                            await stream.CopyToAsync(memStream);
                                            memStream.Position = 0;
                                        }
                                    }

                                    MemoryStream ms = new MemoryStream();
                                    (new Bitmap(memStream)).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                                    BitmapImage img = new BitmapImage();
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
                }
            }
            else
            {
                Methods.CheckConnection();
            }
        }

        private void Game(object sender, MouseButtonEventArgs e)
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