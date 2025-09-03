using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Windows;
using System.Windows.Controls;

namespace Pong_Game
{
    public partial class Register : Window
    {
        private readonly IMongoCollection<User> usersCollection;

        public Register()
        {
            InitializeComponent();

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("PongDB");
            usersCollection = database.GetCollection<User>("Login");
        }

        private void RegisterUser(object sender, RoutedEventArgs e)
        {
            string username = Username.Text.Trim();
            string password = Password.Text.Trim();
            string confirmPassword = ConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            var existingUser = usersCollection.Find(u => u.Username == username).FirstOrDefault();
            if (existingUser != null)
            {
                MessageBox.Show("Username already exists.");
                return;
            }

            var newUser = new User
            {
                Username = username,
                Password = password,
                Coins = 0
            };

            usersCollection.InsertOne(newUser);
            MessageBox.Show("Registration successful!");

            Login login = new();
            this.Visibility = Visibility.Hidden;
            login.Show();
        }

        private void goToLogin(object sender, RoutedEventArgs e)
        {
            Login login = new();
            this.Visibility = Visibility.Hidden;
            login.Show();
        }

        private void registerUsername(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb) tb.Clear();
        }

        private void setPassword(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb) tb.Clear();
        }
        private void confirmPassword(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb) tb.Clear();
        }
    }

    public class User
    {
        [BsonElement("Username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("Password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("Coins")]
        public int Coins { get; set; }
    }
}
