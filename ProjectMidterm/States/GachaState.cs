using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        private Texture2D _catHandTexture;
        private Texture2D _gachaTexture;

        private Vector2 _catHandPosition;
        private float _catHandScale = 0.5f; 
        private float _catHandSpeed = 2.5f;
        private Vector2 _catHandStartPosition;
        private Vector2 _catHandTargetPosition;
        private bool _isHandMovingDown = false;
        private bool _isHandMovingUp = false;
        private bool _handReachedBowl = false;

        private Rectangle _button1RollRect;
        private Rectangle _button10RollRect;
        private Rectangle _historyButtonRect;
        private Rectangle _menuButtonRect;

        private Dictionary<string, Texture2D> _fishSkins;
        private Dictionary<string, Texture2D> _fishShootSkins;

        public string SelectedFish { get; private set; } = "Goldfish";
        private List<string> _obtainedItems;
        private List<string> _currentRollItems;
        private int _currentItemIndex;

        private bool _isRolling = false;
        private bool _isMouseClicked = false;
        private bool _isViewingHistory = false;

        private float _animationAlpha = 0f;
        private float _animationScale = 0.5f;
        private float _animationSpeed = 0.05f;
        private GachaState _gacha;
        public HashSet<string> UnlockedFish { get; private set; } = new HashSet<string>();
        private Random _random;
        private string[] fishPool = { "Goldfish", "Pufferfish", "Rare Fish", "Legendary Fish" };

        public GachaState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            _gacha = this;
            _backgroundTexture = _content.Load<Texture2D>("bg");
            _font = _content.Load<SpriteFont>("Gamefont");
            _fishbowlTexture = _content.Load<Texture2D>("fishbowl");
            _button1RollTexture = _content.Load<Texture2D>("button");
            _button10RollTexture = _content.Load<Texture2D>("button");
            _historyButtonTexture = _content.Load<Texture2D>("button");
            _menuButtonTexture = _content.Load<Texture2D>("button");
            _gachaTexture = _content.Load<Texture2D>("GACHA_SKINS");

            _catHandTexture = _content.Load<Texture2D>("cat_hand");
            _catHandStartPosition = new Vector2(300, -50);
            _catHandTargetPosition = new Vector2(300, 120);
            _catHandPosition = _catHandStartPosition;


            _fishSkins = new Dictionary<string, Texture2D>
            {
                { "Goldfish", content.Load<Texture2D>("goldfish") },
                { "Pufferfish", content.Load<Texture2D>("pufferfish") },
                { "Rare Fish", content.Load<Texture2D>("rarefish") },
                { "Legendary Fish", content.Load<Texture2D>("legendaryfish") }
            };
            _fishShootSkins = new Dictionary<string, Texture2D>
            {
                { "Goldfish", content.Load<Texture2D>("goldfish_shoot") },
                { "Pufferfish", content.Load<Texture2D>("pufferfish_shoot") },
                { "Rare Fish", content.Load<Texture2D>("rarefish_shoot") },
                { "Legendary Fish", content.Load<Texture2D>("legendary_fish_shoot") }
            };

            _random = new Random();
            _obtainedItems = new List<string>();
            _currentRollItems = new List<string>();

            _button1RollRect = new Rectangle(100, 380, 200, 50);
            _button10RollRect = new Rectangle(500, 380, 200, 50);
            _historyButtonRect = new Rectangle(730, 20, 50, 50);
            _menuButtonRect = new Rectangle(20, 20, 50, 50);
        }
        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && _menuButtonRect.Contains(mouseState.Position))
            {
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content, _gacha));
            }
            if (_handReachedBowl && !_isRolling)
            {
                _isHandMovingUp = true;
                _handReachedBowl = false;
            }

            if (_isViewingHistory)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && !_isMouseClicked)
                {
                    _isMouseClicked = true;
                    _isViewingHistory = false;
                }
            }
            else if (_isRolling && _currentItemIndex < _currentRollItems.Count)
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
                _animationAlpha = MathHelper.Clamp(_animationAlpha + _animationSpeed, 0f, 1f);
                _animationScale = MathHelper.Clamp(_animationScale + _animationSpeed, 0.5f, 1f);
            }

            if (_isHandMovingDown)
            {
                _catHandPosition.Y += _catHandSpeed;
                if (_catHandPosition.Y >= _catHandTargetPosition.Y)
                {
                    _isHandMovingDown = false;
                    _handReachedBowl = true;
                }
            }
            else if (_isHandMovingUp)
            {
                _catHandPosition.Y -= _catHandSpeed;
                if (_catHandPosition.Y <= _catHandStartPosition.Y)
                {
                    _isHandMovingUp = false;
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
                spriteBatch.Draw(_backgroundTexture, _graphicsDevice.Viewport.Bounds, Color.White);
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
                spriteBatch.Draw(_backgroundTexture, _graphicsDevice.Viewport.Bounds, Color.White);
                spriteBatch.Draw(
                    _catHandTexture,
                    _catHandPosition,
                    null,
                    Color.White,
                    0f,
                    new Vector2(_catHandTexture.Width / 4, _catHandTexture.Height / 4),
                    _catHandScale,
                    SpriteEffects.None,
                    0f
                );
                spriteBatch.Draw(_fishbowlTexture, new Rectangle(170, 50, 450, 450), Color.White);
                //spriteBatch.DrawString(_font, "Welcome to Gacha Page!", new Vector2(300, 50), Color.Black);
                DrawButton(spriteBatch, _button1RollRect, "1 Roll");
                DrawButton(spriteBatch, _button10RollRect, "10 Rolls");
                spriteBatch.Draw(_gachaTexture, new Rectangle(263, -95, 280, 280), Color.White);

                spriteBatch.Draw(_historyButtonTexture, _historyButtonRect, Color.White);
                spriteBatch.Draw(_menuButtonTexture, _menuButtonRect, Color.White);

                if (_isRolling && _currentItemIndex < _currentRollItems.Count)
                {
                    string currentItem = _currentRollItems[_currentItemIndex];

                    if (_fishSkins.ContainsKey(currentItem))
                    {
                        Vector2 origin = new Vector2(150, 150);

                        spriteBatch.Draw(
                            _fishSkins[currentItem],
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
                }
            }

            spriteBatch.End();
        }

        public void PerformRoll(int rollCount)
        {
            _isHandMovingDown = true;
            _isHandMovingUp = false;
            _catHandPosition = _catHandStartPosition;
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
                        string rolledFish = rewards[j];
                        _currentRollItems.Add(rolledFish);
                        _obtainedItems.Add(rolledFish);

                        if (!UnlockedFish.Contains(rolledFish))
                        {
                            Debug.WriteLine($"New fish unlocked: {rolledFish}");
                            UnlockedFish.Add(rolledFish);
                        }
                        break;
                    }
                }
            }

            _isRolling = true;
            _currentItemIndex = 0;
            _animationAlpha = 0f;
            _animationScale = 0.5f;
        }
        public void SelectFish(string fishType)
        {
            if (_fishShootSkins.ContainsKey(fishType))
            {
                SelectedFish = fishType;
            }
        }
        private void DrawButton(SpriteBatch spriteBatch, Rectangle rect, string text)
        {
            spriteBatch.Draw(_button1RollTexture, rect, Color.White);
            spriteBatch.Draw(_button10RollTexture, rect, Color.White);

            Vector2 textSize = _font.MeasureString(text);
            Vector2 textPosition = new Vector2(
                rect.X + (rect.Width - textSize.X) / 2,
                rect.Y + (rect.Height - textSize.Y) / 2
            );

            spriteBatch.DrawString(_font, text, textPosition, Color.Black);
        }
        public Texture2D GetFishTexture(string fishType)
        {
            return _fishSkins.ContainsKey(fishType) ? _fishSkins[fishType] : _fishSkins["Goldfish"];
        }
        public Texture2D GetFishShootTexture()
        {
            return _fishShootSkins.ContainsKey(SelectedFish) ? _fishShootSkins[SelectedFish] : _fishShootSkins["Goldfish"];
        }
        public override void PostUpdate(GameTime gameTime) { }
    }
}
