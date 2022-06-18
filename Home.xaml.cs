using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SignalX
{
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void game1Click(object sender, MouseButtonEventArgs e)
        {
            var window = Application.Current.MainWindow;

            Game g = new Game("Phasmophobia");

            (window as Main).Game.Navigate(g);
            (window as Main).Game.Visibility = Visibility.Visible;
        }

        private void game2Click(object sender, MouseButtonEventArgs e)
        {
            var window = Application.Current.MainWindow;

            Game g = new Game("Grand Theft Auto V");

            (window as Main).Game.Navigate(g);
            (window as Main).Game.Visibility = Visibility.Visible;
        }

        private void game3Click(object sender, MouseButtonEventArgs e)
        {
            var window = Application.Current.MainWindow;

            Game g = new Game("Among Us");

            (window as Main).Game.Navigate(g);
            (window as Main).Game.Visibility = Visibility.Visible;
        }

        private void game4Click(object sender, MouseButtonEventArgs e)
        {
            var window = Application.Current.MainWindow;

            Game g = new Game("Astroneer");

            (window as Main).Game.Navigate(g);
            (window as Main).Game.Visibility = Visibility.Visible;
        }
    }
}