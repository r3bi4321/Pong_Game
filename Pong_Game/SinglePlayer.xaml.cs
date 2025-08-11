using System;
using System.Collections.Generic;
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
        private float SpeedY2;
        private readonly float Friction = 0.6f;
        private readonly float Speed = 3f;
        private double ballSpeedX = 4;
        private double ballSpeedY = 4;
        private bool gameGoing = false;
        private int scorePlayer1 = 0;
        private int scorePlayer2 = 0;
        private bool coinCollected = false;
        private readonly Random rnd = new();
        private readonly DispatcherTimer coinSpawn;
        private readonly List<Rectangle> activeCoins = [];
        private int coinCount1;
        private int lastTouchedBy = 0;

        public SinglePlayer()
        {
            InitializeComponent();

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
        }

        public void KeyboardDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W) UpKeyPressed = true;
            if (e.Key == Key.S) DownKeyPressed = true;
            if (e.Key == Key.Space && !gameGoing)
            {
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

            double newY1 = Canvas.GetTop(Player1) + SpeedY1;
            newY1 = Math.Clamp(newY1, 0, SinglePlayerWindow.ActualHeight - Player1.Height);
            Canvas.SetTop(Player1, newY1);

            
            double botTargetY = PredictBallImpactY();
            double currentBotY = Canvas.GetTop(Bot) + Bot.Height / 2;
            double botDiff = botTargetY - currentBotY;

            double maxBotSpeed = 8;
            botDiff = Math.Clamp(botDiff, -maxBotSpeed, maxBotSpeed);

            SpeedY2 = (float)botDiff;
            SpeedY2 *= Friction;

            double newY2 = Canvas.GetTop(Bot) + SpeedY2;
            newY2 = Math.Clamp(newY2, 0, SinglePlayerWindow.ActualHeight - Bot.Height);
            Canvas.SetTop(Bot, newY2);

            BallAndCoinUpdate();
        }

        private void BallAndCoinUpdate()
        {
            double nextBallX = Canvas.GetLeft(GameBall) + ballSpeedX;
            double nextBallY = Canvas.GetTop(GameBall) + ballSpeedY;

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
                    coinCollected = true;

                    if(lastTouchedBy== 1)
                    coinCount1++;
                    CoinCount1.Text = coinCount1.ToString();
                }
            }

            if (nextBallX >= SinglePlayerWindow.ActualWidth - GameBall.Width)
            {
                scorePlayer1++;
                UpdateScoreboard();
                gameGoing = false;
                return;
            }

            if (nextBallX <= 0)
            {
                scorePlayer2++;
                UpdateScoreboard();
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

        private double PredictBallImpactY()
        {
            double ballX = Canvas.GetLeft(GameBall);
            double ballY = Canvas.GetTop(GameBall);
            double speedX = ballSpeedX;
            double speedY = ballSpeedY;
            double paddleX = Canvas.GetLeft(Bot);

            while (ballX + GameBall.Width < paddleX)
            {
                ballX += speedX;
                ballY += speedY;

                if (ballY <= 0 || ballY >= SinglePlayerWindow.ActualHeight - GameBall.Height)
                {
                    speedY = -speedY;
                }
            }

            return ballY + GameBall.Height / 2 + rnd.Next(-10, 10);
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

            ballSpeedX = 4;
            ballSpeedY = 4;
            SpeedY1 = 0;
            SpeedY2 = 0;
            gameGoing = false;
        }
    }
}
