using MongoDB.Driver;
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
                    SelectedBallBrush = Brushes.GreenYellow;
                }

                MessageBox.Show($"Ball set to: {selectedBall.Tag}");
            }
        }

        public void ChangePaddle()
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
                    SelectedPaddleBrush = Brushes.HotPink;
                }

                MessageBox.Show($"Paddle set to: {selectedPaddle.Tag}");
            }
        }

        private void LoadPlayerItems()
        {
            LoadPaddleItems();
            LoadBallItems();
        }

        private void LoadPaddleItems()
        {
            var paddleItems = PaddleDesignCollection
                .Find(x => x.Username == Session.Username && x.Bought)
                .ToList();

            int row = 0;
            var validColumns = GridDisplay.ColumnDefinitions
                .Select((cd, index) => new { cd, index })
                .Where(x => x.cd.Width.Value == 100)
                .Select(x => x.index)
                .ToList();

            int half = validColumns.Count / 2;
            var paddleColumns = validColumns.Take(half).ToList();
            int paddleColsCount = paddleColumns.Count;

            for (int i = 0; i < paddleItems.Count; i++)
            {
                var item = paddleItems[i];
                int colIndex = paddleColumns[i % paddleColsCount];
                int currentRow = (i / paddleColsCount);

                var stack = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var rect = new Rectangle
                {
                    Width = 15,
                    Height = 40,
                    Fill = (Brush)new BrushConverter().ConvertFromString(item.Tag)
                };

                var btn = new Button
                {
                    Content = item.Equipped ? "Equipped" : "Equip",
                    Width = 80,
                    Height = 20,
                    Margin = new Thickness(0, 5, 0, 0),
                    Background = item.Equipped ? Brushes.DarkRed : Brushes.LightGray,
                    Foreground = item.Equipped ? Brushes.White : Brushes.Black,
                    Tag = "PaddleButton"
                };

                stack.Children.Add(rect);
                stack.Children.Add(btn);

                Grid.SetColumn(stack, colIndex);
                Grid.SetRow(stack, currentRow);
                Grid.SetRowSpan(stack, 2); 
                GridDisplay.Children.Add(stack);


                if (item.Equipped)
                    SelectedPaddleBrush = rect.Fill;

                btn.Click += (s, e) =>
                {
                    var filter = Builders<StoreItem>.Filter.Eq(x => x.Username, Session.Username);
                    var update = Builders<StoreItem>.Update.Set(x => x.Equipped, false);
                    PaddleDesignCollection.UpdateMany(filter, update);

                    var filterEquip = Builders<StoreItem>.Filter.And(
                        Builders<StoreItem>.Filter.Eq(x => x.Username, Session.Username),
                        Builders<StoreItem>.Filter.Eq(x => x.Tag, item.Tag)
                    );
                    var updateEquip = Builders<StoreItem>.Update.Set(x => x.Equipped, true);
                    PaddleDesignCollection.UpdateOne(filterEquip, updateEquip);

                    foreach (var child in GridDisplay.Children.OfType<StackPanel>())
                    {
                        foreach (var b in child.Children.OfType<Button>().Where(b => (string)b.Tag == "PaddleButton"))
                        {
                            b.Content = "Equip";
                            b.Background = Brushes.LightGray;
                            b.Foreground = Brushes.Black;
                        }
                    }

                    btn.Content = "Equipped";
                    btn.Background = Brushes.DarkRed;
                    btn.Foreground = Brushes.White;

                    SelectedPaddleBrush = rect.Fill;
                    MessageBox.Show($"Paddle selected: {item.Tag}");
                };
            }
        }


        private void LoadBallItems()
        {
            var ballItems = BallDesignCollection
                .Find(x => x.Username == Session.Username && x.Bought)
                .ToList();

            var validColumns = GridDisplay.ColumnDefinitions
                .Select((cd, index) => new { cd, index })
                .Where(x => x.cd.Width.Value == 100)
                .Select(x => x.index)
                .ToList();

            int half = validColumns.Count / 2;
            var ballColumns = validColumns.Skip(half).ToList();
            int ballColsCount = ballColumns.Count;

            for (int i = 0; i < ballItems.Count; i++)
            {
                var item = ballItems[i];
                int colIndex = ballColumns[i % ballColsCount];
                int currentRow = (i / ballColsCount);

                var stack = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var ellipse = new Ellipse
                {
                    Width = 25,
                    Height = 25,
                    Fill = (Brush)new BrushConverter().ConvertFromString(item.Tag)
                };

                var btn = new Button
                {
                    Content = item.Equipped ? "Equipped" : "Equip",
                    Width = 80,
                    Height = 20,
                    Margin = new Thickness(0, 5, 0, 0),
                    Background = item.Equipped ? Brushes.DarkRed : Brushes.LightGray,
                    Foreground = item.Equipped ? Brushes.White : Brushes.Black,
                    Tag = "BallButton"
                };

                stack.Children.Add(ellipse);
                stack.Children.Add(btn);

                Grid.SetColumn(stack, colIndex);
                Grid.SetRow(stack, currentRow);
                Grid.SetRowSpan(stack, 2);
                GridDisplay.Children.Add(stack);


                if (item.Equipped)
                    SelectedBallBrush = ellipse.Fill;

                btn.Click += (s, e) =>
                {
                    var filter = Builders<StoreItem>.Filter.Eq(x => x.Username, Session.Username);
                    var update = Builders<StoreItem>.Update.Set(x => x.Equipped, false);
                    BallDesignCollection.UpdateMany(filter, update);

                    var filterEquip = Builders<StoreItem>.Filter.And(
                        Builders<StoreItem>.Filter.Eq(x => x.Username, Session.Username),
                        Builders<StoreItem>.Filter.Eq(x => x.Tag, item.Tag)
                    );
                    var updateEquip = Builders<StoreItem>.Update.Set(x => x.Equipped, true);
                    BallDesignCollection.UpdateOne(filterEquip, updateEquip);

                    foreach (var child in GridDisplay.Children.OfType<StackPanel>())
                    {
                        foreach (var b in child.Children.OfType<Button>().Where(b => (string)b.Tag == "BallButton"))
                        {
                            b.Content = "Equip";
                            b.Background = Brushes.LightGray;
                            b.Foreground = Brushes.Black;
                        }
                    }

                    btn.Content = "Equipped";
                    btn.Background = Brushes.DarkRed;
                    btn.Foreground = Brushes.White;

                    SelectedBallBrush = ellipse.Fill;
                    MessageBox.Show($"Ball selected: {item.Tag}");
                };
            }
        }
    }
}