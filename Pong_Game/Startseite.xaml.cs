using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Pong_Game
{
    public partial class Startseite : Window
    {
        private int selectedSpeed;
        private int pointsNeeded;
        private readonly double ballSpeedX;
        private readonly double ballSpeedY;

        private List<(Ellipse ball, double dx, double dy)> balls;
        private DispatcherTimer timer;

        public Startseite()
        {
            InitializeComponent();
            Loaded += Startseite_Loaded;

            if (Session.IsLoggedIn)
            {
                Login.Content = "Logout";
            }
            else
            {
                Login.Content = "Login";
            }
        }

        private void Startseite_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeBackground();
        }

        private void ChangeBackground()
        {
            return;
        }

        private void Open2PlayerWindow(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Invalid Number", "Mistake", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenWindowStore(object sender, RoutedEventArgs e)
        {
            StoreWindow storeWindow = new();
            this.Visibility = Visibility.Hidden;
            storeWindow.Show();
        }

        private void OpensinglePlayerWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                selectedSpeed = int.Parse(speedBall.Text);
                pointsNeeded = int.Parse(enterPoints.Text);

                SinglePlayer singlePlayerGame = new(selectedSpeed, pointsNeeded);
                this.Visibility = Visibility.Hidden;
                singlePlayerGame.Show();
            }
            catch (FormatException)
            {
                MessageBox.Show("Invalid Number", "Mistake", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void goToLogin(object sender, RoutedEventArgs e)
        {
            if (!Session.IsLoggedIn)
            {
                Login loginWindow = new();
                this.Visibility = Visibility.Hidden;
                loginWindow.ShowDialog();

                if (Session.IsLoggedIn)
                {
                    Login.Content = "Logout";
                }
            }
            else
            {
                Session.Logout();
                Login.Content = "Login";
            }
        }

        private void InputBallSpeed(object sender, RoutedEventArgs e)
        {
            TextBox? ballSpeed = sender as TextBox;
            ballSpeed.Clear();
        }

        private void enterNeededPoints(object sender, RoutedEventArgs e)
        {
            TextBox? neededPoints = sender as TextBox;
            neededPoints.Clear();
        }
    }
}
