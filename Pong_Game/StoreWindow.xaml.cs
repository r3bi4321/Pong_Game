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

namespace Pong_Game
{
    /// <summary>
    /// Interaction logic for StoreWindow.xaml
    /// </summary>
    public partial class StoreWindow : Window
    {
        public StoreWindow()
        {
            InitializeComponent();
        }

        private void OpenStartWindow(object sender, RoutedEventArgs e)
        {
            Startseite startwindow = new();
            this.Visibility = Visibility.Hidden; 
            startwindow.Show();
        }
    }
}
