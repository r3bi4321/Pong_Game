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
    /// Interaction logic for StoreWindow.xaml
    /// </summary>
    public partial class StoreWindow : Window
    {

        private List<(Ellipse ball, double dx, double dy)> balls;
        private DispatcherTimer timer;

        public StoreWindow()
        {
            InitializeComponent();
            Store.SizeChanged += Store_SizeChanged;
            backgroundMovement();
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

        public void AdjustLayout()
        {

            return; 

        }

        private void buyItem(object sender, RoutedEventArgs e)
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
                (Ball11, 4, -1.3),
                (Ball12, 5, -2),

            ];

            
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16); 
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double maxX = Store.ActualWidth;
            double maxY = Store.ActualHeight;

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
