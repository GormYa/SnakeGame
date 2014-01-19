using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Snake
{
    public partial class FormGame : Form
    {
        readonly List<SnakePart> _snakeParts = new List<SnakePart>();

        private const int TileWidth = 15;
        private const int TileHeight = 15;

        private int _maxTileW;
        private int _maxTileH;

        private int _score;
        private int _direction;

        SnakePart _food = new SnakePart();
        SnakePart _head;

        public FormGame()
        {
            InitializeComponent();
            gameTimer.Tick += Update;
            StartGame();
        }

        private void StartGame()
        {
            _maxTileW = pbCanvas.Size.Width / TileWidth;
            _maxTileH = pbCanvas.Size.Height / TileHeight;

            _score = 0;
            _direction = 0;

            _snakeParts.Add(new SnakePart { X = (_maxTileW / 2), Y = 0 });

            GenerateFood();

            gameTimer.Interval = 400;
            gameTimer.Start();
        }

        private void GenerateFood()
        {
            Random random = new Random();
            _food = new SnakePart
            {
                X = random.Next(0, _maxTileW),
                Y = random.Next(0, _maxTileH)
            };
        }

        private void Update(object sender, EventArgs e)
        {
            UpdateSnake();
            pbCanvas.Invalidate();
        }

        private void GameOver()
        {
            _snakeParts.Clear();
            gameTimer.Stop();

            DialogResult dialogResult = MessageBox.Show(@"Yılan öldü. Tekrar oynamak ister misiniz?", @"C# Snake Game", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                StartGame();
            }
            else
            {
                Application.Exit();
            }
        }

        private void UpdateSnake()
        {
            _head = _snakeParts[0];
            switch (_direction)
            {
                case 0: // Aşağı git
                    _snakeParts.RemoveAt(_snakeParts.Count - 1);
                    _snakeParts.Insert(0, new SnakePart { X = _head.X, Y = (_head.Y + 1) });
                    break;
                case 1: // Sola git
                    _snakeParts.RemoveAt(_snakeParts.Count - 1);
                    _snakeParts.Insert(0, new SnakePart { X = _head.X - 1, Y = _head.Y });
                    break;
                case 2: // Sağa git
                    _snakeParts.RemoveAt(_snakeParts.Count - 1);
                    _snakeParts.Insert(0, new SnakePart { X = _head.X + 1, Y = _head.Y });
                    break;
                case 3: // Yukarı git
                    _snakeParts.RemoveAt(_snakeParts.Count - 1);
                    _snakeParts.Insert(0, new SnakePart { X = _head.X, Y = _head.Y - 1 });
                    break;
            }

            _head = _snakeParts[0];

            // Kendine çarpan kısımların sayısını al
            int sp = _snakeParts.Count(s => s.X == _head.X && s.Y == _head.Y);
            
            if (sp > 1) // Kendine çarpan kısım varsa
            {
                GameOver();

            } // Kafası duvara çarpmışsa
            else if (_head.X < 0 || _head.X >= _maxTileW || _head.Y < 0 || _head.Y >= _maxTileH)
            {
                GameOver();

            } // Yem yemişse
            else if (_snakeParts[0].X == _food.X && _snakeParts[0].Y == _food.Y)
            {
                // Yılana kuyruk ekle
                _snakeParts.Add(new SnakePart{ X = _snakeParts[_snakeParts.Count - 1].X, Y = _snakeParts[_snakeParts.Count - 1].Y });

                // Puanı arttır ve yazdır
                _score++;
                lblScore.Text = string.Format("Puan: {0}", _score);

                // Hızı arttır (timer interval düşür) ve ekrana yazdır
                gameTimer.Interval = gameTimer.Interval - 10;
                lblSpeed.Text = string.Format("Hız: {0}", ((410 - gameTimer.Interval) / 10));

                // Yeni yem oluştur
                GenerateFood();
            }
        }

        private void FormGame_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down && !_direction.Equals(3))
            {
                _direction = 0;
            }
            else if (e.KeyCode == Keys.Left && !_direction.Equals(2))
            {
                _direction = 1;
            }
            else if (e.KeyCode == Keys.Right && !_direction.Equals(1))
            {
                _direction = 2;
            }
            else if (e.KeyCode == Keys.Up && !_direction.Equals(0))
            {
                _direction = 3;
            }
        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            canvas.FillRectangle(Brushes.Red, new Rectangle(_snakeParts[0].X * TileWidth, _snakeParts[0].Y * TileHeight, TileWidth, TileHeight));

            for (int i = 1; i < _snakeParts.Count; i++)
            {
                Brush snakeColor = Brushes.Black;
                canvas.FillRectangle(snakeColor, new Rectangle(_snakeParts[i].X * TileWidth, _snakeParts[i].Y * TileHeight, TileWidth, TileHeight));
            }

            canvas.FillRectangle(Brushes.SeaGreen, new Rectangle(_food.X * TileWidth, _food.Y * TileHeight, TileWidth, TileHeight));
        }
    }
}
