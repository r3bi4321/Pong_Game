using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Pong_Game
{
    public partial class StoreWindow : Window
    {
        private readonly DispatcherTimer timer;
        private readonly IMongoCollection<StoreItem> BallDesignCollection;
        private readonly IMongoCollection<StoreItem> PaddleDesignCollection;
        private readonly IMongoCollection<User> UsersCollection;

        public StoreWindow()
        {
            InitializeComponent();
            coinCountDisplay.Text = Session.Coins.ToString();

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("PongDB");

            BallDesignCollection = database.GetCollection<StoreItem>("BallDesign");
            PaddleDesignCollection = database.GetCollection<StoreItem>("PaddleDesign");
            UsersCollection = database.GetCollection<User>("Login");


            this.Loaded += StoreWindow_Loaded;
        }

        private void StoreWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MarkAlreadyBoughtItems();
        }

        private void BuyPaddleItem(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string tag = btn.Tag?.ToString() ?? "Unknown";

                if (PlayerOwnsItem(tag, isBall: false))
                {
                    btn.Content = "Sold";
                    MessageBox.Show("Item already bought.");
                    return;
                }

                string content = btn.Content?.ToString() ?? "";
                if (!int.TryParse(content.Split(' ')[0], out int price)) price = 0;

                if (price > Session.Coins)
                {
                    MessageBox.Show("Not enough Coins.");
                    return;
                }

                Session.Coins -= price;
                coinCountDisplay.Text = Session.Coins.ToString();

                var filterUser = Builders<User>.Filter.Eq(u => u.Username, Session.Username);
                var updateUser = Builders<User>.Update.Set(u => u.Coins, Session.Coins);
                UsersCollection.UpdateOne(filterUser, updateUser);

                var item = new StoreItem
                {
                    Username = Session.Username,
                    Tag = tag,
                    Price = price,
                    Bought = true,
                    Equipped = false
                };
                PaddleDesignCollection.InsertOne(item);

                ChangeBackgroundAndContentOfBoughtItem(btn);
            }
        }

        public void BuyGameBallItem(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                string tag = btn.Tag?.ToString() ?? "Unknown";

                if (PlayerOwnsItem(tag, isBall: true))
                {
                    ChangeBackgroundAndContentOfBoughtItem(btn);
                    MessageBox.Show("Item already bought");
                    return;
                }

                string content = btn.Content?.ToString() ?? "";
                if (!int.TryParse(content.Split(' ')[0], out int price)) price = 0;

                if (price > Session.Coins)
                {
                    MessageBox.Show("Not enough Coins.");
                    return;
                }

                Session.Coins -= price;
                coinCountDisplay.Text = Session.Coins.ToString();

                var filterUser = Builders<User>.Filter.Eq(u => u.Username, Session.Username);
                var updateUser = Builders<User>.Update.Set(u => u.Coins, Session.Coins);
                UsersCollection.UpdateOne(filterUser, updateUser);

                var item = new StoreItem
                {
                    Username = Session.Username,
                    Tag = tag,
                    Price = price,
                    Bought = true,
                    Equipped = false
                };
                BallDesignCollection.InsertOne(item);

                ChangeBackgroundAndContentOfBoughtItem(btn);
            }
        }

        private void OpenStartWindow(object sender, RoutedEventArgs e)
        {
            Startseite startwindow = new();
            this.Visibility = Visibility.Hidden;
            startwindow.Show();
        }

        private void ChangeBackgroundAndContentOfBoughtItem(Button clickedButton)
        {
            if (clickedButton != null)
            {
                clickedButton.Content = "Sold";
                clickedButton.Background = Brushes.Gray;
                clickedButton.Foreground = Brushes.Black;
                clickedButton.IsEnabled = false;
            }
        }

        private void GoToItemEquipment(object sender, RoutedEventArgs e)
        {
            AdjustLooks adjustLooks = new();
            this.Visibility = Visibility.Hidden;
            adjustLooks.Show();
        }

        private bool PlayerOwnsItem(string tag, bool isBall)
        {
            if (isBall)
            {
                return BallDesignCollection
                    .Find(x => x.Username == Session.Username && x.Tag == tag && x.Bought)
                    .Any();
            }
            else
            {
                return PaddleDesignCollection
                    .Find(x => x.Username == Session.Username && x.Tag == tag && x.Bought)
                    .Any();
            }
        }

        private void MarkAlreadyBoughtItems()
        {
            try
            {
                if (Store == null) return;

                // Get all bought tags from both collections
                var boughtTags = new HashSet<string>(
                    BallDesignCollection
                        .Find(x => x.Username == Session.Username && x.Bought)
                        .Project(x => x.Tag)
                        .ToList()
                        .Concat(
                            PaddleDesignCollection
                                .Find(x => x.Username == Session.Username && x.Bought)
                                .Project(x => x.Tag)
                                .ToList()
                        ),
                    StringComparer.OrdinalIgnoreCase);

                // Find all Buttons inside the Store panel
                var allButtons = FindVisualChildren<Button>(Store).ToList();

                foreach (var btn in allButtons)
                {
                    var tag = (btn.Tag ?? "").ToString().Trim();
                    if (string.IsNullOrEmpty(tag)) continue;

                    if (boughtTags.Contains(tag))
                        ChangeBackgroundAndContentOfBoughtItem(btn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while marking bought items: " + ex.Message);
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t) yield return t;

                foreach (var childOfChild in FindVisualChildren<T>(child))
                    yield return childOfChild;
            }
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

        [BsonElement("Equipped")]
        public bool Equipped { get; set; }
    }
}
