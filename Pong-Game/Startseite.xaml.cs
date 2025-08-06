using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Pong_Game
{
    public partial class Startseite : Window
    {
        private float selectedSpeed = 4; 

        public Startseite()
        {
            InitializeComponent();
            Loaded += Startseite_Loaded;
            SizeChanged += (s, e) => changeWindowSize();
        }

        private void Startseite_Loaded(object sender, RoutedEventArgs e)
        {
            changeWindowSize();
        }

        public void changeWindowSize()
        {
            double centerXText = (StartScreen.ActualWidth - Title.ActualWidth) / 2;
            double centerYText = (StartScreen.ActualHeight - Title.ActualHeight) / 4;
            Canvas.SetLeft(Title, centerXText);
            Canvas.SetBottom(Title, centerYText);

            double centerXPlay = (StartScreen.ActualWidth - PlayButton.ActualWidth) / 2;
            double centerYPlay = (StartScreen.ActualHeight - PlayButton.Height) / 4;
            Canvas.SetLeft(PlayButton, centerXPlay);
            Canvas.SetBottom(PlayButton, centerYPlay);

        }

        private void chooseBallSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (chooseBallSpeed.SelectedItem is ComboBoxItem selectedItem)
            {
                string value = selectedItem.Content.ToString();

                if (float.TryParse(value, out float speed))
                {
                    selectedSpeed = speed;
                    
                }
            }
        }

        private void OpenWindow(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(selectedSpeed);
            this.Visibility = Visibility.Hidden;
            mainWindow.Show();
        }

        private void OpenSettingWindow(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            this.Visibility = Visibility.Hidden;
            settingsWindow.Show();
        }
    }
}
