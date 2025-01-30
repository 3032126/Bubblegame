using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics; 

namespace ProjectMidterm.States
{
    public class FishSelectionState : State
    {
        private GachaState _gacha;
        private List<string> _unlockedFish;
        private int _selectedIndex;
        private Texture2D _backgroundTexture;
        private SpriteFont _font;
        private Rectangle _leftButtonRect;
        private Rectangle _rightButtonRect;
        private Rectangle _confirmButtonRect;
        private MouseState _previousMouseState;
        private Texture2D _menuButtonTexture;
        private Rectangle _menuButtonRect;
        private bool _showConfirmWindow = false;
        private Rectangle _confirmYesRect;
        private Rectangle _confirmNoRect;

        public FishSelectionState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GachaState gacha)
      : base(game, graphicsDevice, content)
        {
            if (gacha == null)
                throw new ArgumentNullException(nameof(gacha), "GachaState is null! Make sure it's passed correctly.");

            _gacha = gacha;
            _unlockedFish = new List<string>(_gacha.UnlockedFish);

            if (_unlockedFish.Count == 0)
            {
                Debug.WriteLine("Warning: No fish unlocked in GachaState!");
            }

            _selectedIndex = _unlockedFish.Count > 0 ? 0 : -1; // Prevent out-of-range errors

            Debug.WriteLine($"FishSelectionState initialized with {_unlockedFish.Count} unlocked fish.");
            _backgroundTexture = _content.Load<Texture2D>("bg");
            _font = _content.Load<SpriteFont>("Gamefont");
            _menuButtonTexture = _content.Load<Texture2D>("button");

            _leftButtonRect = new Rectangle(100, 250, 50, 50);
            _rightButtonRect = new Rectangle(650, 250, 50, 50);
            _confirmButtonRect = new Rectangle(300, 400, 200, 50);
            _menuButtonRect = new Rectangle(20, 20, 50, 50);
            _confirmYesRect = new Rectangle(320, 350, 80, 40);
            _confirmNoRect = new Rectangle(420, 350, 80, 40);
            _menuButtonRect = new Rectangle(20, 20, 50, 50);

        }
        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            // Go back to the menu when clicking the menu button
            if (mouseState.LeftButton == ButtonState.Pressed && _menuButtonRect.Contains(mouseState.Position))
            {
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content, _gacha));
            }

            // Prevent errors if there are no fish unlocked
            if (_unlockedFish.Count == 0)
            {
                return;
            }

            var keyboardState = Keyboard.GetState();

            if (!_showConfirmWindow)
            {
                if ((keyboardState.IsKeyDown(Keys.Right) ||
                    (mouseState.LeftButton == ButtonState.Pressed && _rightButtonRect.Contains(mouseState.Position))) &&
                    _previousMouseState.LeftButton == ButtonState.Released)
                {
                    _selectedIndex = (_selectedIndex + 1) % _unlockedFish.Count;
                }

                if ((keyboardState.IsKeyDown(Keys.Left) ||
                    (mouseState.LeftButton == ButtonState.Pressed && _leftButtonRect.Contains(mouseState.Position))) &&
                    _previousMouseState.LeftButton == ButtonState.Released)
                {
                    _selectedIndex = (_selectedIndex - 1 + _unlockedFish.Count) % _unlockedFish.Count;
                }

                if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    Point mousePosition = mouseState.Position;

                    // Click to select a fish and show confirmation window
                    if (new Rectangle(350, 200, 100, 100).Contains(mousePosition))
                    {
                        _showConfirmWindow = true;
                    }
                }
            }
            else
            {
                // Handle Confirmation Window
                if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    Point mousePosition = mouseState.Position;

                    if (_confirmYesRect.Contains(mousePosition))
                    {
                        _gacha.SelectFish(_unlockedFish[_selectedIndex]);
                        _game.ChangeState(new GameplayState(_game, _graphicsDevice, _content, _gacha));
                    }
                    else if (_confirmNoRect.Contains(mousePosition))
                    {
                        _showConfirmWindow = false;
                    }
                }
            }

            _previousMouseState = mouseState;
        }


        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_backgroundTexture, _graphicsDevice.Viewport.Bounds, Color.White);
            spriteBatch.DrawString(_font, "Select Your Fish", new Vector2(300, 50), Color.Black);
            spriteBatch.Draw(_menuButtonTexture, _menuButtonRect, Color.White);

            if (_unlockedFish.Count == 0)
            {
                spriteBatch.DrawString(_font, "No Fish Unlocked", new Vector2(300, 150), Color.Black);
            }
            else
            {
                string selectedFish = _unlockedFish[_selectedIndex];
                Texture2D fishTexture = _gacha.GetFishTexture(selectedFish);

                spriteBatch.Draw(
                    fishTexture,
                    new Rectangle(350, 200, 100, 100),
                    Color.White
                );

                // Draw Left and Right Navigation Buttons
                spriteBatch.DrawString(_font, "<", new Vector2(_leftButtonRect.X, _leftButtonRect.Y), Color.Black);
                spriteBatch.DrawString(_font, ">", new Vector2(_rightButtonRect.X, _rightButtonRect.Y), Color.Black);
                spriteBatch.DrawString(_font, "Confirm", new Vector2(_confirmButtonRect.X + 50, _confirmButtonRect.Y + 10), Color.Black);
            }

            // Show Confirmation Window
            if (_showConfirmWindow)
            {
                spriteBatch.Draw(_backgroundTexture, new Rectangle(250, 150, 300, 200), Color.White * 0.8f);
                spriteBatch.DrawString(_font, "Use this fish?", new Vector2(320, 200), Color.Black);
                spriteBatch.DrawString(_font, "Yes", new Vector2(_confirmYesRect.X + 25, _confirmYesRect.Y + 10), Color.Green);
                spriteBatch.DrawString(_font, "No", new Vector2(_confirmNoRect.X + 30, _confirmNoRect.Y + 10), Color.Red);
            }

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime) { }
    }
}
