using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pong_Game
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly IMongoCollection<BsonDocument> LoginCollection;

        public Login()
        {
            InitializeComponent();

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("PongDB");
            LoginCollection = database.GetCollection<BsonDocument>("Login");
        }

        private void LoginCheck(object sender, RoutedEventArgs e)
        {
            string username = Username.Text;
            string password = Password.Text;

            var filter = Builders<BsonDocument>.Filter.Eq("Username", username);
            var userDoc = LoginCollection.Find(filter).FirstOrDefault();

            if (userDoc == null)
            {
                MessageBox.Show("Benutzer existiert nicht.");
                return;
            }

            var storedPassword = userDoc.GetValue("Password").AsString;

            if (storedPassword == password)
            {
                
                Session.Username = userDoc.GetValue("Username").AsString;
                Session.Coins = userDoc.GetValue("Coins").AsInt32;
  

                Startseite startseite = new();
                this.Visibility = Visibility.Hidden;
                startseite.Show();
            }
            else
            {
                MessageBox.Show("Falsches Passwort.");
            }
        }


        private void Register_Click(object sender, RoutedEventArgs e)
        {
            Register register = new();
            this.Visibility = Visibility.Hidden;
            register.Show();
        }

        private void BackToStart_Click(object sender, RoutedEventArgs e)
        {
            Startseite startseite = new();
            this.Visibility = Visibility.Hidden;
            startseite.Show();
        }

        private void enterPassword(object sender, RoutedEventArgs e)
        {
            TextBox enterPassword = sender as TextBox;
            enterPassword.Clear();
        }

        private void enterUsername(object sender, RoutedEventArgs e)
        {
            TextBox enterUsername = sender as TextBox;
            enterUsername.Clear();
        }
    }

    public static class Session
        {
            public static string Username { get; set; }
            public static int Coins { get; set; }

            public static bool IsLoggedIn => !string.IsNullOrEmpty(Username);

            public static void Logout()
            {
                Username = null;
                Coins = 0;
            }
        
        }

}
