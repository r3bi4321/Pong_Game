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
            backgroundMovement();
        }

        private void BackToStart_Click(object sender, RoutedEventArgs e)
        {
            Startseite startPage = new();
            this.Visibility = Visibility.Hidden;
            startPage.Show();
        }

        private void paddleSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void ballSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void changeBackground(object sender, RoutedEventArgs e)
        {

        }

        public void backgroundMovement()
        {
            balls =
            [
                (Ball1,  2,  2),
                (Ball2, -2.5,  2),
                (Ball3,  1, -2),
                (Ball4,  2, -1.5),
                (Ball5, -2.5, 1),
                (Ball6,  1.5,  2.5),
                (Ball7, -1.5, -2),
                (Ball8,  2.5, -1.2),
                (Ball9, -2,  1.8),
                (Ball10, 3, -4),
                (Ball11, 2.8, 1.7 ),
                (Ball12, 2.6, 3.6)
            ];


            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double maxX = windowSetting.ActualWidth;
            double maxY = windowSetting.ActualHeight;

            for (int i = 0; i < balls.Count; i++)
            {
                var (ball, dx, dy) = balls[i];

                double x = Canvas.GetLeft(ball) + dx;
                double y = Canvas.GetTop(ball) + dy;


                if (x <= 0 || x + ball.Width >= maxX)
                    dx = -dx;
                if (y <= 0 || y + ball.Height >= maxY)
                    dy = -dy;

                Canvas.SetLeft(ball, x);
                Canvas.SetTop(ball, y);

                balls[i] = (ball, dx, dy);
            }
        }

    }
}
