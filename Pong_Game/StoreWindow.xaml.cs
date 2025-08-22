using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        private readonly IMongoCollection<StoreItem> BallDesignCollection;
        private readonly IMongoCollection<StoreItem> PaddleDesignCollection;

        public StoreWindow()
        {
            InitializeComponent();
            Store.SizeChanged += Store_SizeChanged;

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("PongDB");
            BallDesignCollection = database.GetCollection<StoreItem>("BallDesign");
            PaddleDesignCollection = database.GetCollection<StoreItem>("PaddleDesign");
        }

        private void buyPaddleItem(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string tag = btn.Tag?.ToString() ?? "Unknown";
                string content = btn.Content.ToString();

                int price = 0;
                if (int.TryParse(content.Split(' ')[0], out int parsedPrice))
                {
                    price = parsedPrice;
                }

                var item = new StoreItem
                {
                    Username = Session.Username,
                    Tag = tag,
                    Price = price,
                    Bought = true
                };

                PaddleDesignCollection.InsertOne(item);

                changeBackgroundAndContentOfBoughtItem(btn);
            }
        }

        public void buyGameBallItem(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string tag = btn.Tag?.ToString() ?? "Unknown";
                string content = btn.Content.ToString();

                int price = 0;
                if (int.TryParse(content.Split(' ')[0], out int parsedPrice))
                {
                    price = parsedPrice;
                }

                var item = new StoreItem
                {
                    Username = Session.Username,
                    Tag = tag,
                    Price = price,
                    Bought = true
                };

                BallDesignCollection.InsertOne(item);

                changeBackgroundAndContentOfBoughtItem(btn);
            }
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

        private void changeBackgroundAndContentOfBoughtItem(Button clickedButton)
        {
            if (clickedButton != null)
            {
                clickedButton.Content = "Sold";
                clickedButton.Background = Brushes.Gray;
                clickedButton.IsEnabled = false;
            }
        }

        private void goToItemEquipment(object sender, RoutedEventArgs e)
        {
            AdjustLooks adjustLooks = new();
            this.Visibility = Visibility.Hidden;
            adjustLooks.Show();
        }
    }

    public class StoreItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Username")]
        public string Username { get; set; }   

        [BsonElement("Tag")]
        public string Tag { get; set; }

        [BsonElement("Price")]
        public int Price { get; set; }

        [BsonElement("Bought")]
        public bool Bought { get; set; }
    }


}
