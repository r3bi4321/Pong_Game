using System;
using System.Collections.Generic;
using System.Drawing;
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
using Brushes = System.Windows.Media.Brushes;

namespace Pong_Game
{
    public partial class SettingsWindow : Window
    {
        private bool isDarkMode = true; 

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void BackToStart_Click(object sender, RoutedEventArgs e)
        {
            Startseite Startpage = new();
            this.Visibility = Visibility.Hidden;
            Startpage.Show();
        }

        private void changeBackground(object sender, RoutedEventArgs e)
        {
            if (isDarkMode == false)
            {
                this.Background = Brushes.Black;
                isDarkMode = true;
            }
            else if (isDarkMode == true) 
            {
                this.Background = Brushes.White;
                isDarkMode = false;
            }
        }
    }
}
