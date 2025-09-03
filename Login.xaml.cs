using MongoDB.Bson;
using MongoDB.Driver;
using System.Windows;
using System.Windows.Controls;

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
            string password = Password.Password;

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
