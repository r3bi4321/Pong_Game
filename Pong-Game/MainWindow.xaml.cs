using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Shapes;

namespace Pong_Game
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer gameTimer;
        private bool UpKeyPressed, DownkeyPressed;
        private bool UpArrowPressed, DownArrowPressed;
        private float SpeedY1;
        private float SpeedY2;
        private readonly float Friction = 0.6f;
        private readonly float Speed;
        private double ballSpeedX = 4;
        private double ballSpeedY = 4;
        private bool gameGoing = false;
        private int scorePlayer1 = 0;
        private int scorePlayer2 = 0;
        private bool coinCollected = false;
        private readonly Random rnd = new Random();
        private DispatcherTimer coinSpawn;
        private List<Rectangle> activeCoins = new List<Rectangle>();
        private int coinCount1;
        private int coinCount2;
        private int lastTouchedBy = 0;
     
        public MainWindow (float speed)
        {
            InitializeComponent();

            Speed = speed; 

            gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(14)
            };
            gameTimer.Tick += GameTick;

            coinSpawn = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(25)
            };
            coinSpawn.Tick += SpawnCoin;
            coinSpawn.Start();

            this.Focusable = true;
            this.Focus();
        }

        public void KeyboardDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
                UpKeyPressed = true;
            if (e.Key == Key.S)
                DownkeyPressed = true;
            if (e.Key == Key.Up)
                UpArrowPressed = true;
            if (e.Key == Key.Down)
                DownArrowPressed = true;

            if (e.Key == Key.Space && !gameGoing)
            {
                gameGoing = true;
                gameTimer.Start();
            }
        }

        public void KeyboardUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
                UpKeyPressed = false;
            if (e.Key == Key.S)
                DownkeyPressed = false;
            if (e.Key == Key.Up)
                UpArrowPressed = false;
            if (e.Key == Key.Down)
                DownArrowPressed = false;
        }

        private void GameTick(object sender, EventArgs e)
        {
            if (!gameGoing)
            {
                gameTimer.Stop();
                ResetGame();
                return;
            }


            if (UpKeyPressed)
                SpeedY1 -= Speed;
            if (DownkeyPressed)
                SpeedY1 += Speed;

            SpeedY1 *= Friction;

            double newY1 = Canvas.GetTop(Player1) + SpeedY1;
            if (newY1 < 0) newY1 = 0;
            if (newY1 > GameScreen.ActualHeight - Player1.Height)
                newY1 = GameScreen.ActualHeight - Player1.Height;

            Canvas.SetTop(Player1, newY1);

            if (UpArrowPressed)
                SpeedY2 -= Speed;
            if (DownArrowPressed)
                SpeedY2 += Speed;

            SpeedY2 *= Friction;

            double newY2 = Canvas.GetTop(Player2) + SpeedY2;
            if (newY2 < 0) newY2 = 0;
            if (newY2 > GameScreen.ActualHeight - Player2.Height)
                newY2 = GameScreen.ActualHeight - Player2.Height;

            Canvas.SetTop(Player2, newY2);

            double ballX = Canvas.GetLeft(GameBall) + ballSpeedX;
            double ballY = Canvas.GetTop(GameBall) + ballSpeedY;

            if (ballY <= 0 || ballY >= GameScreen.ActualHeight - GameBall.Height)
            {
                ballSpeedY = -ballSpeedY;
            }

            for (int i = activeCoins.Count - 1; i >= 0; i--)
            {
                Rectangle coin = activeCoins[i];

                double coinX = Canvas.GetLeft(coin); 
                double coinY = Canvas.GetTop(coin);

                Rect coinRect = new Rect(coinX, coinY, coin.Width, coin.Height);
                Rect ballRect = new Rect(Canvas.GetLeft(GameBall), Canvas.GetTop(GameBall), GameBall.Width, GameBall.Height);

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

            if (ballX >= GameScreen.ActualWidth - GameBall.Width)
            {
                scorePlayer1++;
                UpdateScoreboard();
                gameGoing = false;
                return;
            }

            if (ballX <= 0)
            {
                scorePlayer2++;
                UpdateScoreboard();
                gameGoing = false;
                return;
            }

            if (ballX <= Canvas.GetLeft(Player1) + Player1.Width)
            {
                double p1Top = Canvas.GetTop(Player1);
                double p1Bottom = p1Top + Player1.Height;
                if (ballY + GameBall.Height >= p1Top && ballY <= p1Bottom)
                {
                    ballSpeedX = -ballSpeedX;
                    lastTouchedBy = 1;
                }
            }

            double p2Left = Canvas.GetLeft(Player2);
            if (ballX + GameBall.Width >= p2Left)
            {
                double p2Top = Canvas.GetTop(Player2);
                double p2Bottom = p2Top + Player2.Height;
                if (ballY + GameBall.Height >= p2Top && ballY <= p2Bottom)
                {
                    ballSpeedX = -ballSpeedX;
                    lastTouchedBy = 2;
                }
            }

            Canvas.SetLeft(GameBall, ballX);
            Canvas.SetTop(GameBall, ballY);
        }

        private void ResetGame()
        {
            Canvas.SetTop(Player1, (GameScreen.ActualHeight - Player1.Height) / 2);
            Canvas.SetTop(Player2, (GameScreen.ActualHeight - Player2.Height) / 2);

            Canvas.SetLeft(GameBall, (GameScreen.ActualWidth - GameBall.Width) / 2);
            Canvas.SetTop(GameBall, (GameScreen.ActualHeight - GameBall.Height) / 2);

            ballSpeedX = 4;
            ballSpeedY = 4;

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
            Rectangle r = new Rectangle
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
    }
}
