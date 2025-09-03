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
    public partial class SinglePlayer : Window
    {
        private readonly DispatcherTimer gameTimer;
        private bool UpKeyPressed, DownKeyPressed;
        private float SpeedY1;
        private readonly float Friction = 0.6f;
        private readonly float Speed = 3f;
        private double ballSpeedX = 4;
        private double ballSpeedY = 4;
        private bool gameGoing = false;
        private int scorePlayer1 = 0;
        private int scorePlayer2 = 0;
        private readonly Random rnd = new();
        private readonly DispatcherTimer coinSpawn;
        private readonly List<Rectangle> activeCoins = [];
        private int coinCount1;
        private int lastTouchedBy = 0;
        public double nextBallX;
        private double nextBallY;
        private readonly int ImpactX;
        private readonly int ImpactY;
        private readonly double startSpeed;
        private readonly double pointsNeeded;
        private readonly IMongoCollection<BsonDocument> ResultCollection;
        private double botErrorOffset = 0;
        private int botErrorCounter = 0;
        private readonly Random rng = new Random();

        public SinglePlayer(double startSpeedValue, double pointsToWin)
        {
            InitializeComponent();

            GameBall.Fill = AdjustLooks.SelectedBallBrush;
            Player1.Fill = AdjustLooks.SelectedPaddleBrush;
            Bot.Fill = AdjustLooks.SelectedPaddleBrush;

            startSpeed = startSpeedValue;
            pointsNeeded = pointsToWin;

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

            this.KeyDown += KeyPressedForMovement;
            this.KeyUp += KeyboardUp;
            this.SizeChanged += Windowchange;
            this.Focusable = true;
            this.Focus();

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("PongDB");
            ResultCollection = database.GetCollection<BsonDocument>("Results");
        }

        public void KeyPressedForMovement(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W) UpKeyPressed = true;
            if (e.Key == Key.S) DownKeyPressed = true;

            if (e.Key == Key.Space && !gameGoing)
            {
                ResetGame();
                gameGoing = true;
                gameTimer.Start();
            }
        }

        public void KeyboardUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W) UpKeyPressed = false;
            if (e.Key == Key.S) DownKeyPressed = false;
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
            if (DownKeyPressed) SpeedY1 += Speed;
            SpeedY1 *= Friction;

            MovementPlayer();

            double currentBotY = Canvas.GetTop(Bot) + Bot.Height / 2;
            double targetY;

            if (ballSpeedX > 0)
            {
                targetY = Canvas.GetTop(GameBall) + GameBall.Height / 2;
            }
            else
            {
                targetY = SinglePlayerWindow.ActualHeight / 2;
            }

            SetBotPosition(currentBotY, targetY);

            BallAndCoinUpdate();
        }

        public void SetBotPosition(double currentBotY, double targetY)
        {
          
            botErrorCounter++;
            if (botErrorCounter > 50)
            {
                botErrorOffset = rng.NextDouble() * 80 - 20; 
                botErrorCounter = 0;
            }

            double adjustedTargetY = targetY + botErrorOffset;

            double botDiff = adjustedTargetY - currentBotY;
            double maxBotSpeed = 4; 
            botDiff = Math.Clamp(botDiff, -maxBotSpeed, maxBotSpeed);

            double newY2 = Canvas.GetTop(Bot) + botDiff;
            newY2 = Math.Clamp(newY2, 0, SinglePlayerWindow.ActualHeight - Bot.Height);
            Canvas.SetTop(Bot, newY2);
        }

        public void MovementPlayer()
        {
            double newY1 = Canvas.GetTop(Player1) + SpeedY1;
            newY1 = Math.Clamp(newY1, 0, SinglePlayerWindow.ActualHeight - Player1.Height);
            Canvas.SetTop(Player1, newY1);
        }

        private void BallAndCoinUpdate()
        {
            nextBallX = Canvas.GetLeft(GameBall) + ballSpeedX;
            nextBallY = Canvas.GetTop(GameBall) + ballSpeedY;

            if (nextBallY <= 0 || nextBallY >= SinglePlayerWindow.ActualHeight - GameBall.Height)
                ballSpeedY = -ballSpeedY;

            for (int i = activeCoins.Count - 1; i >= 0; i--)
            {
                Rectangle coin = activeCoins[i];
                Rect coinRect = new(Canvas.GetLeft(coin), Canvas.GetTop(coin), coin.Width, coin.Height);
                Rect ballRect = new(nextBallX, nextBallY, GameBall.Width, GameBall.Height);

                if (coinRect.IntersectsWith(ballRect))
                {
                    SinglePlayerWindow.Children.Remove(coin);
                    activeCoins.RemoveAt(i);

                    if (lastTouchedBy == 1)
                        coinCount1++;

                    CoinCount1.Text = coinCount1.ToString();
                }
            }

            if (nextBallX >= SinglePlayerWindow.ActualWidth - GameBall.Width)
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

            double p2Left = Canvas.GetLeft(Bot);
            if (nextBallX + GameBall.Width >= p2Left)
            {
                double p2Top = Canvas.GetTop(Bot);
                double p2Bottom = p2Top + Bot.Height;

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
            string winnerText = winningPlayer == 1 ? "Spieler 1" : "Bot";
            Endpage endPage = new(winnerText);
            endPage.Show();
        }

        private void SpawnCoin(object sender, EventArgs e)
        {
            Rectangle coin = new()
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Yellow
            };

            double x = rnd.Next(0, (int)(SinglePlayerWindow.ActualWidth - coin.Width));
            double y = rnd.Next(0, (int)(SinglePlayerWindow.ActualHeight - coin.Height));

            Canvas.SetLeft(coin, x);
            Canvas.SetTop(coin, y);

            SinglePlayerWindow.Children.Add(coin);
            activeCoins.Add(coin);
        }

        private void UpdateScoreboard()
        {
            ScorePlayer1.Text = scorePlayer1.ToString();
            ScoreBot.Text = scorePlayer2.ToString();
        }

        private void ResetGame()
        {
            Canvas.SetLeft(Player1, 0);
            Canvas.SetLeft(Bot, SinglePlayerWindow.ActualWidth - Bot.Width);
            Canvas.SetTop(Player1, (SinglePlayerWindow.ActualHeight - Player1.Height) / 2);
            Canvas.SetTop(Bot, (SinglePlayerWindow.ActualHeight - Bot.Height) / 2);
            Canvas.SetLeft(GameBall, (SinglePlayerWindow.ActualWidth - GameBall.Width) / 2);
            Canvas.SetTop(GameBall, (SinglePlayerWindow.ActualHeight - GameBall.Height) / 2);

            ballSpeedX = startSpeed;
            ballSpeedY = startSpeed;

            SpeedY1 = 0;
        }

        private void Windowchange(object sender, SizeChangedEventArgs e)
        {
            double rightPos = SinglePlayerWindow.ActualWidth - Bot.Width;
            if (rightPos < 0) rightPos = 0;
            Canvas.SetLeft(Bot, rightPos);

            double scoreRightPos = SinglePlayerWindow.ActualWidth - ScoreBot.Width - 40;
            if (scoreRightPos < 0) scoreRightPos = 0;
            Canvas.SetLeft(ScoreBot, scoreRightPos);
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
