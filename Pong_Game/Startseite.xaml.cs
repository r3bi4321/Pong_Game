using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Pong_Game
{
    public partial class Startseite : Window
    {
        private int selectedSpeed;
        private int pointsNeeded;
        private double ballSpeedX;
        private double ballSpeedY;

        private List<(Ellipse ball, double dx, double dy)> balls;
        private DispatcherTimer timer;

        public Startseite()
        {
            InitializeComponent();
            Loaded += Startseite_Loaded;
            SizeChanged += (s, e) => AdjustLayout();
            backgroundMovement(); 
        }

        private void Startseite_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustLayout();
          
        }

        public void AdjustLayout()
        {
            double width = StartScreen.ActualWidth;
            double height = StartScreen.ActualHeight;

            double elementWidth = width / 4;
            double elementHeight = height / 15;
            double fontSize = width / 40;

            double spacing = elementHeight * 1.5;
            Title.FontSize = width / 12;
            double titleX = (width - Title.ActualWidth) / 2.5;
            Canvas.SetLeft(Title, titleX);
            Canvas.SetTop(Title, height * 0.1);

            speedBall.Width = elementWidth;
            speedBall.Height = elementHeight;
            speedBall.FontSize = fontSize;
            Canvas.SetLeft(speedBall, (width - elementWidth) / 2);
            Canvas.SetTop(speedBall, height * 0.3);

            enterPoints.Width = elementWidth;
            enterPoints.Height = elementHeight;
            enterPoints.FontSize = fontSize;
            Canvas.SetLeft(enterPoints, (width - elementWidth) / 2);
            Canvas.SetTop(enterPoints, height * 0.3 + spacing);

            singlePlayerButton.Width = elementWidth;
            singlePlayerButton.Height = elementHeight;
            singlePlayerButton.FontSize = fontSize;
            Canvas.SetLeft(singlePlayerButton, (width - elementWidth) / 2);
            Canvas.SetTop(singlePlayerButton, height * 0.3 + spacing * 2);

            twoPlayerButton.Width = elementWidth;
            twoPlayerButton.Height = elementHeight;
            twoPlayerButton.FontSize = fontSize;
            Canvas.SetLeft(twoPlayerButton, (width - elementWidth) / 2);
            Canvas.SetTop(twoPlayerButton, height * 0.3 + spacing * 3);

            Settings.Width = elementWidth * 0.6;
            Settings.Height = elementHeight;
            Settings.FontSize = fontSize;
            Canvas.SetTop(Settings, 10);
            Canvas.SetLeft(Settings, width - Settings.Width - 20);

            goToStore.Width = elementWidth * 0.6;
            goToStore.Height = elementHeight;
            goToStore.FontSize = fontSize;
            Canvas.SetTop(goToStore, 20 + Settings.Height + 10);
            Canvas.SetLeft(goToStore, width - goToStore.Width - 20);
        }

        private void OpenWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                selectedSpeed = int.Parse(speedBall.Text);
                pointsNeeded = int.Parse(enterPoints.Text);

                MainWindow mainWindow = new(selectedSpeed, pointsNeeded);
                this.Visibility = Visibility.Hidden;
                mainWindow.Show();
            }
            catch (FormatException)
            {
                MessageBox.Show("Bitte gültige Zahlen eingeben!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenSettingWindow(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new();
            this.Visibility = Visibility.Hidden;
            settingsWindow.Show();
        }

        private void OpenWindowStore(object sender, RoutedEventArgs e)
        {
            StoreWindow storeWindow = new();
            this.Visibility = Visibility.Hidden;
            storeWindow.Show();
        }

        private void OpensinglePlayerWindow(object sender, RoutedEventArgs e)
        {
            SinglePlayer singlePlayer = new();
            this.Visibility = Visibility.Hidden;
            singlePlayer.Show();
        }

        public void backgroundMovement()
        {
            balls =
            [
                (Ball1,  2,  2),
                (Ball2, -2,  1.5),
                (Ball3,  1, -2),
                (Ball4,  2, -1.5),
                (Ball5, -2.5, 1),
                (Ball6,  1.5,  2.5),
                (Ball7, -1.5, -2),
                (Ball8,  2.5, -1.2),
                (Ball9, -2,  1.8),
                (Ball10, 2, 3.2),
                (Ball11, 4, 1.9),
                (Ball12, 3.4, 1.5),
                (Ball13, 2.1, 4.1),
                (Ball14, 3.4, 2.6),
                (Ball15, 2.6, 1.3)
            ];


            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double maxX = StartScreen.ActualWidth;
            double maxY = StartScreen.ActualHeight;

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
