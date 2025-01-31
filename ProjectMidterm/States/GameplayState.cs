using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
namespace ProjectMidterm.States
{
    public class GameplayState : State
    {
        private Texture2D _backgroundTexture;
        private Dictionary<string, Texture2D> _bubbleTextures;

        private Texture2D _fishTexture;
        private Vector2 _fishPosition;
        private float _fishRotation;
        private Vector2 _currentBubblePosition;
        private Vector2 _bubbleDirection;
        private Dictionary<Vector2, string> _bubbleGrid;
        private bool _isBubbleFired;
        private string _currentBubbleColor;
        private string _nextBubbleColor;
        private Random _random;
        private int _score;
        private int _gridWidth = 8;
        private int _gridHeight = 10;
        private int _bubbleSize = 32;
        private int _playAreaX = 200;  // ขอบซ้ายของพื้นที่เล่น
        private int _playAreaWidth = 400;  // กว้างของพื้นที่เล่น
        private Texture2D _handdownTexture;
        private Vector2 _handdownPosition;
        private float _timeSinceLastDrop;
        private float _dropInterval = 5f; // 10 วินาที
        private int _dropDistance = 32; // ระยะที่ Bubble ขยับลงมาแต่ละครั้ง

        private bool _isGameOver = false;
        private SpriteFont _gameOverFont;
        private GachaState _gacha;

        private SoundEffect _shootingSound; // เสียงยิงบับเบิ้ล

        private Rectangle _menuButtonRect;
        private Texture2D _menuButtonTexture;

        public GameplayState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GachaState gacha)
            : base(game, graphicsDevice, content)
        {
            _gacha = gacha;
            _backgroundTexture = _content.Load<Texture2D>("bg");
            _fishTexture = _gacha.GetFishShootTexture();
            _handdownTexture = _content.Load<Texture2D>("Handdown");
            _gameOverFont = _content.Load<SpriteFont>("Gamefont");
            _menuButtonTexture = _content.Load<Texture2D>("button");

            _menuButtonRect = new Rectangle(20, 20, 50, 50);


            _handdownPosition = new Vector2(150, -500); // อยู่ตรงกลางบนสุดของจอ


            _bubbleTextures = new Dictionary<string, Texture2D>
            {
                { "bubble1", _content.Load<Texture2D>("Bubble_1") },
                { "bubble2", _content.Load<Texture2D>("Bubble_2") },
                { "bubble3", _content.Load<Texture2D>("Bubble_3") },
                { "bubble4", _content.Load<Texture2D>("Bubble_4") },
                { "bubble5", _content.Load<Texture2D>("Bubble_5") }
            };

            _shootingSound = _content.Load<SoundEffect>("Shootingeffect"); // โหลดเสียงยิงบับเบิ้ล


            _fishPosition = new Vector2(400, 450);
            _fishRotation = 0f;
            _currentBubblePosition = _fishPosition;
            _bubbleDirection = Vector2.Zero;
            _isBubbleFired = false;
            _bubbleGrid = new Dictionary<Vector2, string>();
            _random = new Random();
            _score = 0;

            GenerateInitialGrid();
            LoadNewBubble();
        }

        private void GenerateInitialGrid()
        {
            string[] colors = { "bubble1", "bubble2", "bubble3", "bubble4", "bubble5" };
            int startX = (800 - (_gridWidth * _bubbleSize)) / 2; // คำนวณจุดเริ่มให้บับเบิ้ลอยู่ตรงกลาง
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < _gridWidth; x++)
                {
                    Vector2 position = new Vector2(startX + x * _bubbleSize, 50 + y * _bubbleSize);
                    _bubbleGrid[position] = colors[_random.Next(colors.Length)];
                }
            }
        }


        private void LoadNewBubble()
        {
            string[] colors = { "bubble1", "bubble2", "bubble3", "bubble4", "bubble5" };

            if (string.IsNullOrEmpty(_currentBubbleColor))
            {
                _currentBubbleColor = colors[_random.Next(colors.Length)];
                _nextBubbleColor = colors[_random.Next(colors.Length)];
            }
            else
            {
                _currentBubbleColor = _nextBubbleColor;
                _nextBubbleColor = colors[_random.Next(colors.Length)];
            }
        }

        public override void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && _menuButtonRect.Contains(mouseState.Position))
            {
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content, _gacha));
            }
            if (state.IsKeyDown(Keys.Left))
                _fishRotation -= 0.05f;
            if (state.IsKeyDown(Keys.Right))
                _fishRotation += 0.05f;


            _timeSinceLastDrop += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeSinceLastDrop >= _dropInterval)
            {
                MoveBubblesDown();
                _timeSinceLastDrop = 0f;
            }


            _fishRotation = MathHelper.Clamp(_fishRotation, -MathHelper.ToRadians(85), MathHelper.ToRadians(85));


            if (!_isBubbleFired && state.IsKeyDown(Keys.Space))
            {
                FireBubble();
            }

            if (_isBubbleFired)
            {
                _currentBubblePosition += _bubbleDirection * 10f;

                foreach (var position in _bubbleGrid.Keys)
                {
                    if (Vector2.Distance(_currentBubblePosition, position) < _bubbleSize)
                    {
                        AttachBubbleToGrid(_currentBubblePosition, _currentBubbleColor);
                        _isBubbleFired = false;
                        LoadNewBubble();
                        return;
                    }
                }

                // **เพิ่มกำแพงชิ่งให้เฉพาะตรงกลาง**
                if (_currentBubblePosition.X < _playAreaX || _currentBubblePosition.X > (_playAreaX + _playAreaWidth))
                {
                    _bubbleDirection.X *= -1;
                }

                if (_currentBubblePosition.Y < 50)
                {
                    AttachBubbleToGrid(_currentBubblePosition, _currentBubbleColor);
                    _isBubbleFired = false;
                    LoadNewBubble();
                }
            }
        }

        private void MoveBubblesDown()
        {
            if (_isGameOver) return; // ถ้าเกมจบแล้วให้หยุดอัปเดต

            Dictionary<Vector2, string> newGrid = new Dictionary<Vector2, string>();

            foreach (var bubble in _bubbleGrid)
            {
                Vector2 newPosition = new Vector2(bubble.Key.X, bubble.Key.Y + _dropDistance);
                newGrid[newPosition] = bubble.Value;
            }

            _bubbleGrid = newGrid;

            // ขยับ Handdown ลงมา
            _handdownPosition.Y += _dropDistance;

            // ตรวจสอบว่า Bubble แตะปลาหรือยัง
            foreach (var bubble in _bubbleGrid.Keys)
            {
                if (bubble.Y >= _fishPosition.Y - _bubbleSize)
                {
                    _isGameOver = true; // ตั้งค่า Game Over
                }
            }
        }



        private void FireBubble()
        {
            if (!_isBubbleFired)
            {
                _isBubbleFired = true;
                _bubbleDirection = new Vector2((float)Math.Sin(_fishRotation), -(float)Math.Cos(_fishRotation));
                _currentBubblePosition = _fishPosition;

                _shootingSound.Play();

            }
        }

        private void AttachBubbleToGrid(Vector2 bubblePosition, string color)
        {
            Vector2 nearestBubble = FindClosestBubble(bubblePosition);

            if (nearestBubble != Vector2.Zero)
            {
                Vector2 snappedPosition = GetNearestEmptyAdjacentPosition(nearestBubble);

                if (!_bubbleGrid.ContainsKey(snappedPosition))
                {
                    _bubbleGrid[snappedPosition] = color;
                    CheckMatches(snappedPosition, color);
                }
            }
        }

        private Vector2 FindClosestBubble(Vector2 position)
        {
            Vector2 closest = Vector2.Zero;
            float minDistance = float.MaxValue;

            foreach (var bubble in _bubbleGrid.Keys)
            {
                float distance = Vector2.Distance(position, bubble);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = bubble;
                }
            }

            return closest;
        }

        private Vector2 GetNearestEmptyAdjacentPosition(Vector2 basePosition)
        {
            Vector2[] directions = {
                new Vector2(-_bubbleSize, 0),
                new Vector2(_bubbleSize, 0),
                new Vector2(0, -_bubbleSize),
                new Vector2(0, _bubbleSize),
                new Vector2(-_bubbleSize / 2, -_bubbleSize),
                new Vector2(_bubbleSize / 2, -_bubbleSize)
            };

            foreach (var dir in directions)
            {
                Vector2 newPosition = basePosition + dir;
                if (!_bubbleGrid.ContainsKey(newPosition))
                {
                    return newPosition;
                }
            }

            return basePosition;
        }

        private void CheckMatches(Vector2 startPosition, string color)
        {
            HashSet<Vector2> matchedBubbles = new HashSet<Vector2>();

            FindMatchingBubbles(startPosition, color, matchedBubbles);

            if (matchedBubbles.Count >= 3)
            {
                foreach (var bubble in matchedBubbles)
                {
                    _bubbleGrid.Remove(bubble);
                }

                _score += matchedBubbles.Count * 10;
            }
        }

        private void FindMatchingBubbles(Vector2 position, string color, HashSet<Vector2> matchedBubbles)
        {
            if (!_bubbleGrid.ContainsKey(position)) return;
            if (matchedBubbles.Contains(position)) return;
            if (_bubbleGrid[position] != color) return;

            matchedBubbles.Add(position);

            foreach (var neighbor in GetNeighbors(position))
            {
                FindMatchingBubbles(neighbor, color, matchedBubbles);
            }
        }

        private List<Vector2> GetNeighbors(Vector2 position)
        {
            List<Vector2> neighbors = new List<Vector2>();

            Vector2[] directions = {
                new Vector2(-_bubbleSize, 0),
                new Vector2(_bubbleSize, 0),
                new Vector2(0, -_bubbleSize),
                new Vector2(0, _bubbleSize),
                new Vector2(-_bubbleSize / 2, -_bubbleSize),
                new Vector2(_bubbleSize / 2, -_bubbleSize)
            };

            foreach (var direction in directions)
            {
                Vector2 neighborPos = position + direction;
                if (_bubbleGrid.ContainsKey(neighborPos))
                {
                    neighbors.Add(neighborPos);
                }
            }

            return neighbors;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, 800, 600), Color.White);
            spriteBatch.Draw(_menuButtonTexture, _menuButtonRect, Color.White);

            // วาดขอบเขตให้ Bubble ชิ่ง
            Texture2D boundary = new Texture2D(_graphicsDevice, 1, 1);
            boundary.SetData(new[] { Color.White });
            spriteBatch.Draw(boundary, new Rectangle(_playAreaX - 5, 50, 5, 500), Color.Black);
            spriteBatch.Draw(boundary, new Rectangle(_playAreaX + _playAreaWidth, 50, 5, 500), Color.Black);

            // วาดฟองอากาศทั้งหมด
            foreach (var bubble in _bubbleGrid)
            {
                spriteBatch.Draw(
                    _bubbleTextures[bubble.Value],
                    new Rectangle((int)bubble.Key.X, (int)bubble.Key.Y, (int)(_bubbleSize * 1.5f), (int)(_bubbleSize * 1.5f)),
                    Color.White
                );
            }

            spriteBatch.Draw(_handdownTexture, _handdownPosition, Color.White);
            spriteBatch.Draw(
                _fishTexture,
                _fishPosition,
                null,
                Color.White,
                _fishRotation,
                new Vector2(_fishTexture.Width / 2, _fishTexture.Height / 2),
                0.3f,
                SpriteEffects.None,
                0f
            );

            if (_isBubbleFired)
            {
                spriteBatch.Draw(_bubbleTextures[_currentBubbleColor], new Rectangle((int)_currentBubblePosition.X, (int)_currentBubblePosition.Y, _bubbleSize, _bubbleSize), Color.White);
            }

            spriteBatch.DrawString(_content.Load<SpriteFont>("Gamefont"), $"Score: {_score}", new Vector2(10, 10), Color.White);
            spriteBatch.Draw(_bubbleTextures[_currentBubbleColor], new Rectangle(380, 400, _bubbleSize, _bubbleSize), Color.White);
            spriteBatch.Draw(_bubbleTextures[_nextBubbleColor], new Rectangle(440, 400, _bubbleSize / 2, _bubbleSize / 2), Color.White);

            // แสดงข้อความ "Game Over" เมื่อแพ้
            if (_isGameOver)
            {
                Vector2 textSize = _gameOverFont.MeasureString("GAME OVER");
                Vector2 textPosition = new Vector2((800 - textSize.X) / 2, 250);
                spriteBatch.DrawString(_gameOverFont, "GAME OVER", textPosition, Color.Red);
            }

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime) { }

    }
}