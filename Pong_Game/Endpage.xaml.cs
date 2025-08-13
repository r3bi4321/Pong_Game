using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Pong_Game
{
    /// <summary>
    /// Interaction logic for Endpage.xaml
    /// </summary>
    public partial class Endpage : Window
    {
        private int winningPlayer;
        private List<(Ellipse ball, double dx, double dy)> balls;
        private DispatcherTimer timer;

        public Endpage(string winnerText)
        {
            InitializeComponent();
            WinnerLabel.Text = winnerText;
        }

        public Endpage(int winningPlayer)
        {
            this.winningPlayer = winningPlayer;
            
        }

        private void OpenStartWindow(object sender, RoutedEventArgs e)
        {
            Startseite startWindow = new();
            this.Hide();
            startWindow.Show();
        }

        private void endGame_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void goToStore_Click(object sender, RoutedEventArgs e)
        {
            StoreWindow storeWindow = new();
            storeWindow.Show();
            this.Hide();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new();   
            settingsWindow.Show();
            this.Hide();
        }
    }
}
