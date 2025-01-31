using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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
        private bool _showConfirmWindow = false;
        private Rectangle _confirmYesRect;
        private Rectangle _confirmNoRect;
        private MouseState _previousMouseState;
        private Rectangle _menuButtonRect;

        private Texture2D _menuButtonTexture;

        public FishSelectionState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GachaState gacha)
            : base(game, graphicsDevice, content)
        {
            _gacha = gacha;
            _unlockedFish = new List<string>(_gacha.UnlockedFish);
            _selectedIndex = _unlockedFish.Count > 0 ? 0 : -1;

            _backgroundTexture = _content.Load<Texture2D>("bg");
            _font = _content.Load<SpriteFont>("Gamefont");
            _menuButtonTexture = _content.Load<Texture2D>("button");

            _leftButtonRect = new Rectangle(100, 250, 50, 50);
            _rightButtonRect = new Rectangle(650, 250, 50, 50);
            _confirmButtonRect = new Rectangle(300, 400, 200, 50);
            _menuButtonRect = new Rectangle(20, 20, 50, 50);
            _confirmYesRect = new Rectangle(320, 350, 80, 40);
            _confirmNoRect = new Rectangle(420, 350, 80, 40);
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && _menuButtonRect.Contains(mouseState.Position))
            {
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content, _gacha));
            }
            if (_unlockedFish.Count == 0) return;
            if (!_showConfirmWindow)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
                {
                    Point mousePosition = mouseState.Position;

                    if (_leftButtonRect.Contains(mousePosition))
                    {
                        _selectedIndex = (_selectedIndex - 1 + _unlockedFish.Count) % _unlockedFish.Count;
                    }
                    else if (_rightButtonRect.Contains(mousePosition))
                    {
                        _selectedIndex = (_selectedIndex + 1) % _unlockedFish.Count;
                    }
                    else if (_confirmButtonRect.Contains(mousePosition))
                    {
                        _showConfirmWindow = true;
                    }
                }
            }
            else
            {
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
                    new Rectangle(250, 100, 300, 300),
                    Color.White
                );

                // Draw Left and Right Navigation Buttons
                spriteBatch.DrawString(_font, "<", new Vector2(_leftButtonRect.X, _leftButtonRect.Y), Color.Black);
                spriteBatch.DrawString(_font, ">", new Vector2(_rightButtonRect.X, _rightButtonRect.Y), Color.Black);
                spriteBatch.DrawString(_font, "Confirm", new Vector2(_confirmButtonRect.X + 50, _confirmButtonRect.Y + 10), Color.Black);
            }
            
            if (_showConfirmWindow)
            {
                spriteBatch.DrawString(_font, "Use this fish?", new Vector2(320, 200), Color.Black);
                spriteBatch.DrawString(_font, "Yes", new Vector2(_confirmYesRect.X + 25, _confirmYesRect.Y + 10), Color.Green);
                spriteBatch.DrawString(_font, "No", new Vector2(_confirmNoRect.X + 30, _confirmNoRect.Y + 10), Color.Red);
            }

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime) { }
    }
}