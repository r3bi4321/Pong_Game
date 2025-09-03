using MongoDB.Bson;
using MongoDB.Driver;
using Pong_Game.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Pong_Game
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer gameTimer;
        private bool UpKeyPressed, DownkeyPressed;
        private bool UpArrowPressed, DownArrowPressed;
        private float SpeedY1;
        private float SpeedY2;
        private readonly float Friction = 0.6f;
        private readonly float Speed = 1.5f;
        private double ballSpeedX;
        private double ballSpeedY;
        private readonly double startSpeed;
        private bool gameGoing = false;
        private int scorePlayer1 = 0;
        private int scorePlayer2 = 0;
        private readonly int pointsNeeded;
        private bool coinCollected = false;
        private readonly Random rnd = new();
        private readonly DispatcherTimer coinSpawn;
        private readonly List<Rectangle> activeCoins = new();
        private int coinCount1;
        private int coinCount2;
        private int lastTouchedBy = 0;
        private readonly IMongoCollection<BsonDocument> ResultCollection;
       

        public MainWindow(int startSpeedValue, int pointsToWin)
        {
            InitializeComponent();

            GameBall.Fill = AdjustLooks.SelectedBallBrush;
            Player1.Fill = AdjustLooks.SelectedPaddleBrush;
            Player2.Fill = AdjustLooks.SelectedPaddleBrush;
            Player1.Name = Session.Username;

            startSpeed = startSpeedValue;
            pointsNeeded = pointsToWin;

            ballSpeedX = startSpeed;
            ballSpeedY = startSpeed;

            gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(14)
            };
            gameTimer.Tick += GameTick;

            coinSpawn = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(25)
            };
            coinSpawn.Tick += SpawnCoin;
            coinSpawn.Start();

            this.Focusable = true;
            this.Focus();

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("PongDB");
            ResultCollection = database.GetCollection<BsonDocument>("Results");
        }

        public void KeyboardDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W) UpKeyPressed = true;
            if (e.Key == Key.S) DownkeyPressed = true;
            if (e.Key == Key.Up) UpArrowPressed = true;
            if (e.Key == Key.Down) DownArrowPressed = true;

            if (e.Key == Key.Space && !gameGoing)
            {
                gameGoing = true;
                gameTimer.Start();
            }
        }

        public void KeyboardUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W) UpKeyPressed = false;
            if (e.Key == Key.S) DownkeyPressed = false;
            if (e.Key == Key.Up) UpArrowPressed = false;
            if (e.Key == Key.Down) DownArrowPressed = false;
        }

        private void GameTick(object sender, EventArgs e)
        {
            if (!gameGoing)
            {
                gameTimer.Stop();
                ResetGame();
                return;
            }

            if (UpKeyPressed) SpeedY1 -= Speed;
            if (DownkeyPressed) SpeedY1 += Speed;
            SpeedY1 *= Friction;

            double newY1 = Math.Clamp(Canvas.GetTop(Player1) + SpeedY1, 0, GameScreen.ActualHeight - Player1.Height);
            Canvas.SetTop(Player1, newY1);

            if (UpArrowPressed) SpeedY2 -= Speed;
            if (DownArrowPressed) SpeedY2 += Speed;
            SpeedY2 *= Friction;

            double newY2 = Math.Clamp(Canvas.GetTop(Player2) + SpeedY2, 0, GameScreen.ActualHeight - Player2.Height);
            Canvas.SetTop(Player2, newY2);

            BallAndCoinUpdate();
        }

        private void BallAndCoinUpdate()
        {
            double nextBallX = Canvas.GetLeft(GameBall) + ballSpeedX;
            double nextBallY = Canvas.GetTop(GameBall) + ballSpeedY;

            if (nextBallY <= 0 || nextBallY >= GameScreen.ActualHeight - GameBall.Height)
                ballSpeedY = -ballSpeedY;

            for (int i = activeCoins.Count - 1; i >= 0; i--)
            {
                Rectangle coin = activeCoins[i];
                Rect coinRect = new(Canvas.GetLeft(coin), Canvas.GetTop(coin), coin.Width, coin.Height);
                Rect ballRect = new(nextBallX, nextBallY, GameBall.Width, GameBall.Height);

                if (coinRect.IntersectsWith(ballRect))
                {
                    GameScreen.Children.Remove(coin);
                    activeCoins.RemoveAt(i);
                    coinCollected = true;

                    if (lastTouchedBy == 1)
                    {
                        coinCount1++;
                        CoinCount1.Text = coinCount1.ToString();
                    }
                    else if (lastTouchedBy == 2)
                    {
                        coinCount2++;
                        CoinCount2.Text = coinCount2.ToString();
                    }
                }
            }

            if (nextBallX >= GameScreen.ActualWidth - GameBall.Width)
            {
                scorePlayer1++;
                UpdateScoreboard();

                if (scorePlayer1 >= pointsNeeded)
                {
                    EndGame(1);
                    return;
                }

                gameGoing = false;
                return;
            }

            if (nextBallX <= 0)
            {
                scorePlayer2++;
                UpdateScoreboard();

                if (scorePlayer2 >= pointsNeeded)
                {
                    EndGame(2);
                    return;
                }

                gameGoing = false;
                return;
            }

            double p1Right = Canvas.GetLeft(Player1) + Player1.Width;
            if (nextBallX <= p1Right)
            {
                double p1Top = Canvas.GetTop(Player1);
                double p1Bottom = p1Top + Player1.Height;

                if (nextBallY + GameBall.Height >= p1Top && nextBallY <= p1Bottom)
                {
                    ballSpeedX = Math.Abs(ballSpeedX);
                    lastTouchedBy = 1;
                    nextBallX = p1Right + 1;
                }
            }

            double p2Left = Canvas.GetLeft(Player2);
            if (nextBallX + GameBall.Width >= p2Left)
            {
                double p2Top = Canvas.GetTop(Player2);
                double p2Bottom = p2Top + Player2.Height;

                if (nextBallY + GameBall.Height >= p2Top && nextBallY <= p2Bottom)
                {
                    ballSpeedX = -Math.Abs(ballSpeedX);
                    lastTouchedBy = 2;
                    nextBallX = p2Left - GameBall.Width - 1;
                }
            }

            Canvas.SetLeft(GameBall, nextBallX);
            Canvas.SetTop(GameBall, nextBallY);
        }

        private void EndGame(int winningPlayer)
        {
            gameTimer.Stop();
            gameGoing = false;

            saveResultToDB(scorePlayer1, scorePlayer2);
            CoinManager.AddCoinsToUser(Session.Username, coinCount1);

            this.Visibility = Visibility.Hidden;

            string winnerText = winningPlayer == 1 ? "Spieler 1" : "Spieler 2";
            Endpage endPage = new(winnerText);
            endPage.Show();
        }

        private void ResetGame()
        {
            Canvas.SetLeft(Player1, 0);
            Canvas.SetLeft(Player2, GameScreen.ActualWidth - Player2.Width);

            Canvas.SetTop(Player1, (GameScreen.ActualHeight - Player1.Height) / 2);
            Canvas.SetTop(Player2, (GameScreen.ActualHeight - Player2.Height) / 2);
            Canvas.SetLeft(GameBall, (GameScreen.ActualWidth - GameBall.Width) / 2);
            Canvas.SetTop(GameBall, (GameScreen.ActualHeight - GameBall.Height) / 2);

            ballSpeedX = startSpeed;
            ballSpeedY = startSpeed;
            SpeedY1 = 0;
            SpeedY2 = 0;
            gameGoing = false;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double rightPos = GameScreen.ActualWidth - Player2.Width;
            if (rightPos < 0) rightPos = 0;
            Canvas.SetLeft(Player2, rightPos);

            double scoreRightPos = GameScreen.ActualWidth - ScorePlayer2.Width - 40;
            if (scoreRightPos < 0) scoreRightPos = 0;
            Canvas.SetLeft(ScorePlayer2, scoreRightPos);

            double centerX = (GameScreen.ActualWidth - GameBall.Width) / 2;
            double centerY = (GameScreen.ActualHeight - GameBall.Height) / 2;
            Canvas.SetLeft(GameBall, centerX);
            Canvas.SetTop(GameBall, centerY);

            double coins2 = GameScreen.ActualWidth - CoinCount2.Width - 40;
            if (coins2 < 0) coins2 = 0;
            Canvas.SetLeft(CoinCount2, coins2);
        }

        private void SpawnCoin(object sender, EventArgs e)
        {
            Rectangle r = new()
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Yellow
            };

            double x = rnd.Next(0, (int)(GameScreen.ActualWidth - r.Width));
            double y = rnd.Next(0, (int)(GameScreen.ActualHeight - r.Height));

            Canvas.SetLeft(r, x);
            Canvas.SetTop(r, y);

            GameScreen.Children.Add(r);
            activeCoins.Add(r);
        }

        private void UpdateScoreboard()
        {
            ScorePlayer1.Text = scorePlayer1.ToString();
            ScorePlayer2.Text = scorePlayer2.ToString();
        }

        public void setPaddleSize(double width, double height)
        {
            Player1.Width = width;
            Player1.Height = height;
        }

        public void setBallSize(double width, double height)
        {
            GameBall.Width = width;
            GameBall.Height = height;
        }

        public void saveResultToDB(int scorePlayer1, int scorePlayer2)
        {
            try
            {
                var resultDoc = new BsonDocument
                {
                    { "Player1Score", scorePlayer1 },
                    { "Player2Score", scorePlayer2 },
                    { "Date", DateTime.UtcNow.AddHours(2)}
                };

                ResultCollection.InsertOne(resultDoc);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving result: " + ex.Message);
            }
        }
    }
}
