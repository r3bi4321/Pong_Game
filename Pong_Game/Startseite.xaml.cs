using System.Windows;
using System.Windows.Controls;

namespace Pong_Game
{
    public partial class Startseite : Window
    {
        private readonly float selectedSpeed = 4;

        public Startseite()
        {
            InitializeComponent();
            Loaded += Startseite_Loaded;
            SizeChanged += (s, e) => AdjustLayout();
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
            double titleX = (width - Title.ActualWidth) / 2;
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
            MainWindow mainWindow = new(selectedSpeed);
            this.Visibility = Visibility.Hidden;
            mainWindow.Show();
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
    }
}
