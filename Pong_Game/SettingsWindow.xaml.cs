using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Pong_Game
{
    public partial class SettingsWindow : Window
    {

        private List<(Ellipse ball, double dx, double dy)> balls;
        private DispatcherTimer timer;
        public SettingsWindow()
        {
            InitializeComponent();

        }

        private void BackToStart_Click(object sender, RoutedEventArgs e)
        {
            Startseite startPage = new();
            this.Visibility = Visibility.Hidden;
            startPage.Show();
        }

        private void paddleSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double valuePaddleSize = paddleSize.Value;
            int PaddleSize = (int)valuePaddleSize;
        }

        private void ballSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double valueBallSize = ballSize.Value;
            int BallSize = (int)valueBallSize;
 
        }

        private void changeBackground(object sender, RoutedEventArgs e)
        {
            DarkAndLightMode darkAndLightMode = new();
        }

    }
}
