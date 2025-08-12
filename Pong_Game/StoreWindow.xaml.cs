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
            Store.SizeChanged += Store_SizeChanged;
        }

        private void Store_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           AdjustLayout();
        }

        private void OpenStartWindow(object sender, RoutedEventArgs e)
        {
            Startseite startwindow = new();
            this.Visibility = Visibility.Hidden; 
            startwindow.Show();
        }

        private void Startseite_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustLayout();
        }
        public void AdjustLayout()
        {

            double width = Store.ActualWidth;
            double height = Store.ActualHeight;

            double elementWidth = width / 4;
            double elementHeight = height / 15;
            double fontSize = width / 40;
            double spacing = elementHeight * 1.5;


        }

        private void buyItem(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
