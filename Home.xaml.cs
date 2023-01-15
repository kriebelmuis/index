using System;
<<<<<<< HEAD
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
=======
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
>>>>>>> b6dd1929d093d3c5b1e5ed7acaec71f9eb586773
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml;
<<<<<<< HEAD
using ModernWpf;
=======
>>>>>>> b6dd1929d093d3c5b1e5ed7acaec71f9eb586773

namespace Index
{
    public partial class Home : Page
    {
        private readonly QuinticEase ease = new()
        {
            EasingMode = EasingMode.EaseInOut
        };

        public Home()
        {
            try
            {
                InitializeComponent();
                Trace.WriteLine("Home initialized");
                Boot();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error while preparing Home.xaml.cs\n\n{ex}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Boot()
        {
            try
            {
                var row = 0;
                var colomn = 0;

                Int16 indexid = 1;

                foreach (var game in Data.games)
                {
                    if (Data.gametable.Select($"id = {game.Metadata.ID}").Length == 0)
                    {
                        Trace.WriteLine("Found new game " + game.Metadata.Name + game.Images.Banners[0].ToString());

                        object[] data = {
                            game.Metadata.ID,
                            indexid,
                            game.Metadata.Name,
                            game.Metadata.Description,
                            game.Images.Banners[0],
                            game.Download.InfoHash
                        };

                        var newrow = Data.gametable.NewRow();

                        var dex = 0;
                        foreach (var dat in data)
                        {
                            if (dat is null)
                            {
                                newrow[dex] = "Data unavailable";
                                dex++;
                                break;
                            }
                            newrow[dex] = dat;
                            dex++;
                        }

                        Data.gametable.Rows.Add(newrow);

                        var cnvs = (Grid)XamlReader.Load(XmlReader.Create(new StringReader(XamlWriter.Save(game1))));
                        cnvs.Name = $"game{indexid}";

                        if (colomn == 3)
                        {
                            RowDefinition ne = new()
                            {
                                Height = new GridLength(140)
                            };

                            Games.RowDefinitions.Add(ne);
                            colomn = 0;
                            row++;
                        }

                        Grid.SetColumn(cnvs, colomn);
                        Grid.SetRow(cnvs, row);

                        Games.Children.Add(cnvs);

                        Refresh(game.Metadata.ID, cnvs);
                        indexid++;
                        colomn++;
                    }
                    else
                    {
                        Trace.WriteLine("Found indexed game " + game.Metadata.Name);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error while filling gametable data\n\n{ex}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Refresh(Int16 id, FrameworkElement cnvs)
        {
            DataRow row = null;

            try
            {
                row = Data.gametable.Select($"id = '{id}'")[0];
            }
            catch
            {
                MessageBox.Show($"Error while finding gametable row\nIndexID: {id}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                var text = (Label)cnvs.FindDescendantByName($"gameName");
                text.Content = row[(int)GameTable.Name].ToString();

                var desc = (TextBlock)cnvs.FindDescendantByName($"gameDesc");
                desc.Text = row[(int)GameTable.Description].ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error while setting text\nIndexID: {id} GameID: {row[2]}\n\n{ex}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(row[(int)GameTable.Banner].ToString());
                bitmap.EndInit();

                var img = (Image)cnvs.FindDescendantByName($"image");
                img.Source = bitmap;

                cnvs.Visibility = Visibility.Visible;
                DoubleAnimation da = new()
                {
                    From = cnvs.Opacity,
                    To = 1.0,
                    Duration = new Duration(TimeSpan.FromSeconds(.25)),
                    EasingFunction = ease
                };
                cnvs.BeginAnimation(OpacityProperty, da);

                cnvs.PreviewMouseLeftButtonUp += GameClick;
                cnvs.MouseEnter += GameEnter;
                cnvs.MouseLeave += GameLeave;
                cnvs.Cursor = Cursors.Hand;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error while loading image\nIndexID: {id}\nImage: {row[(int)GameTable.Banner]}\n\n{ex}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GameClick(object sender, MouseEventArgs e)
        {
            try
            {
                var window = Application.Current.MainWindow;
                var element = (FrameworkElement)sender;

                var indexid = Int16.Parse(Regex.Match(element.Name, @"\d+").Value);
                var row = Data.gametable.Select($"indexid = {indexid}")[0];

                (window as Main).Game.Navigate(new Game(Int16.Parse(row[(int)GameTable.ID].ToString())));
                (window as Main).Game.Visibility = Visibility.Visible;
                DoubleAnimation da = new()
                {
                    From = (window as Main).Game.Opacity,
                    To = 1.0,
                    Duration = new Duration(TimeSpan.FromSeconds(.25)),
                    EasingFunction = ease
                };
                (window as Main).Game.BeginAnimation(OpacityProperty, da);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error while preparing game page:\n\n{ex}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private readonly Duration dur = new(TimeSpan.FromMilliseconds(500));
        private readonly Duration dur1 = new(TimeSpan.FromMilliseconds(750));

        private void GameEnter(object sender, MouseEventArgs e)
        {
            try
            {
                var window = Application.Current.MainWindow;
                var element = (FrameworkElement)sender;

                var indexid = Int16.Parse(Regex.Match(element.Name, @"\d+").Value);
                var row = Data.gametable.Select($"indexid = {indexid}")[0];

                var button = (Canvas)element.FindDescendantByName("gameButton");

                var text = (Label)element.FindDescendantByName("gameName");
                var desc = (TextBlock)element.FindDescendantByName("gameDesc");

                ThicknessAnimation da = new()
                {
                    From = text.Margin,
                    To = new Thickness(0, 10, 0, 0),
                    Duration = dur,
                    EasingFunction = ease
                };
                ThicknessAnimation da1 = new()
                {
                    From = desc.Margin,
                    To = new Thickness(0, 40, 0, 0),
                    Duration = dur,
                    EasingFunction = ease

                };
                DoubleAnimation da2 = new()
                {
                    From = button.OpacityMask.Opacity,
                    To = .25,
                    Duration = dur1,
                    EasingFunction = ease
                };
                text.BeginAnimation(MarginProperty, da);
                desc.BeginAnimation(MarginProperty, da1);
                button.BeginAnimation(OpacityProperty, da2);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while animating game enter:\n\n{ex}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GameLeave(object sender, MouseEventArgs e)
        {
            try
            {
                var window = Application.Current.MainWindow;
                var element = (FrameworkElement)sender;

                var indexid = Int16.Parse(Regex.Match(element.Name, @"\d+").Value);
                var row = Data.gametable.Select($"indexid = {indexid}")[0];

                var button = (Canvas)element.FindDescendantByName("gameButton");

                var text = (Label)element.FindDescendantByName("gameName");
                var desc = (TextBlock)element.FindDescendantByName("gameDesc");

                ThicknessAnimation da = new()
                {
                    From = text.Margin,
                    To = new Thickness(0, 120, 0, 0),
                    Duration = dur,
                    EasingFunction = ease
                };
                ThicknessAnimation da1 = new()
                {
                    From = desc.Margin,
                    To = new Thickness(0, 150, 0, 0),
                    Duration = dur,
                    EasingFunction = ease
                };
                DoubleAnimation da2 = new()
                {
                    From = button.OpacityMask.Opacity,
                    To = 1,
                    Duration = dur1,
                    EasingFunction = ease
                };
                text.BeginAnimation(MarginProperty, da);
                desc.BeginAnimation(MarginProperty, da1);
                button.BeginAnimation(OpacityProperty, da2);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while animating game leave:\n\n{ex}", "Index", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}