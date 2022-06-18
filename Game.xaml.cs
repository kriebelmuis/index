using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace SignalX
{
    public partial class Game : System.Windows.Controls.Page
    {
        static List<GameData> GetData()
        {
            WebClient wc = new WebClient();

            try
            {
                string json = wc.DownloadString(new Uri("https://raw.githubusercontent.com/OmarHopman/signal/main/database/database.json"));

                if (!string.IsNullOrWhiteSpace(json))
                {
                    var gamedata = JsonConvert.DeserializeObject<List<GameData>>(json);

                    return gamedata;
                }

                return null;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(1);

                return null;
            }
        }

        public Game(string gameName) 
        {
            InitializeComponent();

            WebClient wc = new WebClient();

            var games = GetData();
            if (games != null)
            {
                for (int i = 0; i < games.Count; i++)
                {
                    if (games[i].Name == gameName)
                    {
                        GameName.Content = games[i].Name;
                        GameDesc.Text = games[i].Description;

                        {
                            Dispatcher.Invoke(() =>
                            {
                                //back.Content = gamedata;
                                try
                                {
                                    //gameImage1.Source = new BitmapImage(new Uri(games[i].Images.Banners.B1));
                                    gameImage1.Source = new BitmapImage(new Uri(games[i].Images.Banners.B1));
                                }
                                catch
                                {
                                    MessageBox.Show("Image error, please report this to our Discord server!\n", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            });
                        }
                    }
                }
            }

            Dispatcher.Invoke(() =>    
            {
                //back.Content = gamedata;
                try
                {
                    gameImage1.Source = new BitmapImage(new Uri ("Cache/" + GameName + "1.jpg"));
                }
                catch
                {
                    MessageBox.Show("Image error, please report this to our Discord server!\n", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private void Download()
        {
            
        }

        private void Update()
        {

        }

        private void goBackClick(object sender, MouseButtonEventArgs e)
        {
            var window = Application.Current.MainWindow;
            (window as Main).Game.Visibility = Visibility.Hidden;
        }
    }
}