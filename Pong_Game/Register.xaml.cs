using System;
using System.Windows;
using System.Windows.Controls;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Pong_Game
{
    public partial class Register : Window
    {
        private readonly IMongoCollection<BsonDocument> usersCollection;

        public Register()
        {
            InitializeComponent();

            var client = new MongoClient("mongodb://localhost:27017"); 
            var database = client.GetDatabase("PongDB");
            usersCollection = database.GetCollection<BsonDocument>("Login");
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
           
            var existingUser = usersCollection.Find(u => u["Username"] == username).FirstOrDefault();
            if (existingUser != null)
            {
                MessageBox.Show("Username already exists.");
                return;
            }

            var userDoc = new BsonDocument
            {

                { "Username", username },
                { "Password", password },
                {"Coins", 0 }
            };

            usersCollection.InsertOne(userDoc);
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
            TextBox registerUser = sender as TextBox;
            registerUser.Clear(); 
        }

        private void setPassword(object sender, RoutedEventArgs e)
        {
            TextBox setPassword = sender as TextBox;
            setPassword.Clear(); 
        }

        private void confirmPassword(object sender, RoutedEventArgs e)
        {
            TextBox confirmPassword = sender as TextBox;
            confirmPassword.Clear(); 
        }
    }
}
