using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ProjectMidterm.States
{
    public class GachaState : State
    {
        private Texture2D _backgroundTexture;
        private SpriteFont _font;
        private Texture2D _fishbowlTexture;
        private Texture2D _button1RollTexture;
        private Texture2D _button10RollTexture;
        private Texture2D _historyButtonTexture;
        private Texture2D _menuButtonTexture;

        private Rectangle _button1RollRect;
        private Rectangle _button10RollRect;
        private Rectangle _historyButtonRect;
        private Rectangle _menuButtonRect;

        private Dictionary<string, Texture2D> _itemTextures;
        private List<string> _obtainedItems;
        private List<string> _currentRollItems;
        private int _currentItemIndex;
        private Random _random;

        private bool _isRolling = false;
        private bool _isMouseClicked = false;
        private bool _isViewingHistory = false;

        private float _animationAlpha = 0f;
        private float _animationScale = 0.5f;
        private float _animationSpeed = 0.05f;
        public Dictionary<string, Texture2D> ItemTextures => _itemTextures;

        public GachaState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            _backgroundTexture = _content.Load<Texture2D>("bg");
            _font = _content.Load<SpriteFont>("Gamefont");
            _fishbowlTexture = _content.Load<Texture2D>("fishbowl");
            _button1RollTexture = _content.Load<Texture2D>("button");
            _button10RollTexture = _content.Load<Texture2D>("button");
            _historyButtonTexture = _content.Load<Texture2D>("button");
            _menuButtonTexture = _content.Load<Texture2D>("button");

            // Load item textures
            _itemTextures = new Dictionary<string, Texture2D>
            {
                { "Goldfish", _content.Load<Texture2D>("goldfish") },
                { "Pufferfish", _content.Load<Texture2D>("pufferfish") },
                { "Rare Fish", _content.Load<Texture2D>("goldfish") },
                { "Legendary Fish", _content.Load<Texture2D>("goldfish") }
            };
            _random = new Random();
            _obtainedItems = new List<string>();
            _currentRollItems = new List<string>();

            _button1RollRect = new Rectangle(100, 380, 200, 50);
            _button10RollRect = new Rectangle(500, 380, 200, 50);
            _historyButtonRect = new Rectangle(680, 20, 50, 50);
            _menuButtonRect = new Rectangle(20, 20, 50, 50);

        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && _menuButtonRect.Contains(mouseState.Position))
            {
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content)); // กลับไปที่ Menu
            }
            if (_isViewingHistory)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && !_isMouseClicked)
                {
                    _isMouseClicked = true;
                    _isViewingHistory = false;
                }
            }
            else if (_isRolling)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && !_isMouseClicked)
                {
                    _isMouseClicked = true; 
                    _currentItemIndex++; 

                    _animationAlpha = 0f; 
                    _animationScale = 0.5f; 

                    if (_currentItemIndex >= _currentRollItems.Count)
                    {
                        _isRolling = false;
                        _currentRollItems.Clear();
                        _currentItemIndex = 0;
                    }
                }
            }
            else if (mouseState.LeftButton == ButtonState.Pressed && !_isMouseClicked)
            {
                var mousePos = mouseState.Position;

                if (_button1RollRect.Contains(mousePos))
                {
                    PerformRoll(1); 
                }
                else if (_button10RollRect.Contains(mousePos))
                {
                    PerformRoll(10); 
                }
                else if (_historyButtonRect.Contains(mousePos))
                {
                    _isViewingHistory = true;
                }
                _isMouseClicked = true; 
            }
            if (mouseState.LeftButton == ButtonState.Released)
            {
                _isMouseClicked = false;
            }

            if (_isRolling && _currentItemIndex < _currentRollItems.Count)
            {
                _animationAlpha = MathHelper.Clamp(_animationAlpha + _animationSpeed, 0f, 1f);
                _animationScale = MathHelper.Clamp(_animationScale + _animationSpeed, 0.5f, 1f);
            }
       }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            if (_isViewingHistory)
            {
                spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height), Color.White);
                spriteBatch.DrawString(_font, "Roll History:", new Vector2(345, 50), Color.Black);

                Vector2 historyPosition = new Vector2(100, 100);
                foreach (var item in _obtainedItems)
                {
                    spriteBatch.DrawString(_font, item, historyPosition, Color.Black);
                    historyPosition.Y += 30;
                }

                spriteBatch.DrawString(_font, "Click anywhere to return", new Vector2(325, 400), Color.Black);
            }
            else
            {
                    spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height), Color.White);
                    spriteBatch.Draw(_fishbowlTexture, new Rectangle(250, 50, 300, 300), Color.White);
                    spriteBatch.DrawString(_font, "Welcome to Gacha Page!", new Vector2(300, 50), Color.Black);
                    spriteBatch.Draw(_button1RollTexture, _button1RollRect, Color.White);
                    spriteBatch.Draw(_button10RollTexture, _button10RollRect, Color.White);
                    spriteBatch.Draw(_historyButtonTexture, _historyButtonRect, Color.White);
                    spriteBatch.Draw(_menuButtonTexture, _menuButtonRect, Color.White);
                    spriteBatch.DrawString(_font, "Home", new Vector2(_menuButtonRect.X + 7, _menuButtonRect.Y + 15), Color.Black);
                    spriteBatch.DrawString(_font, "1 roll", new Vector2(_button1RollRect.X + 80, _button1RollRect.Y + 15), Color.Black);
                    spriteBatch.DrawString(_font, "10 roll", new Vector2(_button10RollRect.X + 80, _button10RollRect.Y + 15), Color.Black);
                    spriteBatch.DrawString(_font, "History", new Vector2(_historyButtonRect.X + 2, _historyButtonRect.Y + 15), Color.Black);

                if (_isRolling && _currentItemIndex < _currentRollItems.Count)
                {
                    string currentItem = _currentRollItems[_currentItemIndex];

                    if (_itemTextures.ContainsKey(currentItem))
                    {
                        Vector2 origin = new Vector2(150, 150);

                        spriteBatch.Draw(
                            _itemTextures[currentItem],
                            new Vector2(250, 80),
                            null,
                            Color.White * _animationAlpha,
                            0f,
                            origin,
                            _animationScale,
                            SpriteEffects.None,
                            0f
                        );
                    }
                    spriteBatch.DrawString(_font, $"Roll: {_currentItemIndex + 1}", new Vector2(250, 400), Color.Black);
                }
            }

            spriteBatch.End();
        }
        public string LastRolledItem { get; private set; } = "";

        public void PerformRoll(int rollCount)
        {
            List<string> rewards = new List<string> { "Goldfish", "Pufferfish", "Rare Fish", "Legendary Fish" };
            List<int> probabilities = new List<int> { 60, 30, 9, 1 };

            _currentRollItems.Clear();

            for (int i = 0; i < rollCount; i++)
            {
                int roll = _random.Next(1, 101);
                int cumulative = 0;

                for (int j = 0; j < rewards.Count; j++)
                {
                    cumulative += probabilities[j];
                    if (roll <= cumulative)
                    {
                        _currentRollItems.Add(rewards[j]);
                        _obtainedItems.Add(rewards[j]);

                        LastRolledItem = rewards[j];

                        System.Console.WriteLine($"Rolled Fish: {LastRolledItem}");
                        break;
                    }
                }
            }

            _isRolling = true;
            _currentItemIndex = 0;

            _animationAlpha = 0f;
            _animationScale = 0.5f;
        }

        public override void PostUpdate(GameTime gameTime) { }

    }
}
