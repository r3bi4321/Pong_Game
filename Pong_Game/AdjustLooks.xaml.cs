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
    /// Interaction logic for AdjustLooks.xaml
    /// </summary>
    public partial class AdjustLooks : Window
    {
        public AdjustLooks()
        {
            InitializeComponent();
        }

        private void goBackToStart(object sender, RoutedEventArgs e)
        {
            Startseite startPage = new();
            this.Visibility = Visibility.Hidden;
            startPage.Show();
        }

        private void goToStore(object sender, RoutedEventArgs e)
        {
            StoreWindow storeWindow = new();
            this.Visibility = Visibility.Hidden;
            storeWindow.Show();
        }
    }
}
