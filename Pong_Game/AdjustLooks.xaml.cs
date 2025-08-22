using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Pong_Game
{
    /// <summary>
    /// Interaction logic for AdjustLooks.xaml
    /// </summary>
    public partial class AdjustLooks : Window
    {
        private readonly IMongoCollection<StoreItem> BallDesignCollection;
        private readonly IMongoCollection<StoreItem> PaddleDesignCollection;


        public static Brush SelectedBallBrush { get; private set; } = Brushes.White;
        public static Brush SelectedPaddleBrush { get; private set; } = Brushes.White;

        public AdjustLooks()
        {
            InitializeComponent();

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("PongDB");
            BallDesignCollection = database.GetCollection<StoreItem>("BallDesign");
            PaddleDesignCollection = database.GetCollection<StoreItem>("PaddleDesign");

            LoadPlayerItems();
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

        public void changeGameBall()
        {
            var myItems = BallDesignCollection
                .Find(x => x.Username == Session.Username && x.Bought)
                .ToList();

            if (myItems.Any())
            {
                var selectedBall = myItems.First();

                BrushConverter bc = new BrushConverter();
                try
                {
                    SelectedBallBrush = (Brush)bc.ConvertFromString(selectedBall.Tag);
                }
                catch
                {
                    SelectedBallBrush = Brushes.White; 
                }

                MessageBox.Show($"Ball gesetzt auf: {selectedBall.Tag}");
            }
        }

        public void changePaddle()
        {
            var myItems = PaddleDesignCollection
                .Find(x => x.Username == Session.Username && x.Bought)
                .ToList();

            if (myItems.Any())
            {
                var selectedPaddle = myItems.First();

                BrushConverter bc = new BrushConverter();
                try
                {
                    SelectedPaddleBrush = (Brush)bc.ConvertFromString(selectedPaddle.Tag);
                }
                catch
                {
                    SelectedPaddleBrush = Brushes.White;
                }

                MessageBox.Show($"Paddle gesetzt auf: {selectedPaddle.Tag}");
            }
        }


        private void LoadPlayerItems()
        {
            var ballItems = BallDesignCollection
                .Find(x => x.Username == Session.Username && x.Bought)
                .ToList();

            var paddleItems = PaddleDesignCollection
                .Find(x => x.Username == Session.Username && x.Bought)
                .ToList();

            int col = 0;
            int row = 0;

      
            var validColumns = GridDisplay.ColumnDefinitions
                .Select((cd, index) => new { cd, index })
                .Where(x => x.cd.Width.Value == 100)
                .Select(x => x.index)
                .ToList();

            foreach (var index in validColumns)
            {
                if (ballItems.Count > 0)
                {
                    var item = ballItems[0];
                    ballItems.RemoveAt(0);

                    
                    var ellipse = new Ellipse
                    {
                        Width = 25,
                        Height = 25,
                        Margin = new Thickness(5),
                        Fill = (Brush)new BrushConverter().ConvertFromString(item.Tag)
                    };
                    Grid.SetColumn(ellipse, index);
                    Grid.SetRow(ellipse, row);
                    GridDisplay.Children.Add(ellipse);

                    
                    var btn = new Button
                    {
                        Content = "Equip",
                        Width = 60,
                        Height = 25,
                        Margin = new Thickness(5)
                    };
                    Grid.SetColumn(btn, index);
                    Grid.SetRow(btn, row + 1); 
                    GridDisplay.Children.Add(btn);

                   
                    btn.Click += (s, e) =>
                    {
                        SelectedBallBrush = ellipse.Fill;
                        MessageBox.Show($"Ball ausgewählt: {item.Tag}");
                    };
                }

                if (paddleItems.Count > 0)
                {
                    var item = paddleItems[0];
                    paddleItems.RemoveAt(0);

                    var rect = new Rectangle
                    {
                        Width = 20,
                        Height = 50,
                        Margin = new Thickness(5),
                        Fill = (Brush)new BrushConverter().ConvertFromString(item.Tag)
                    };
                    Grid.SetColumn(rect, index);
                    Grid.SetRow(rect, row + 2); 
                    GridDisplay.Children.Add(rect);

                    var btn = new Button
                    {
                        Content = "Equip",
                        Width = 60,
                        Height = 25,
                        Margin = new Thickness(5)
                    };
                    Grid.SetColumn(btn, index);
                    Grid.SetRow(btn, row + 3);
                    GridDisplay.Children.Add(btn);

                    btn.Click += (s, e) =>
                    {
                        SelectedPaddleBrush = rect.Fill;
                        MessageBox.Show($"Paddle ausgewählt: {item.Tag}");
                    };
                }
            }
        }


    }
}