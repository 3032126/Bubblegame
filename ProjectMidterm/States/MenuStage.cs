using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace ProjectMidterm.States
{
    public class MenuState : State
    {
        private Texture2D _backgroundTexture;
        private Texture2D _buttonTexture;

        private Rectangle _startButtonRect;
        private Rectangle _optionsButtonRect;
        private Rectangle _gachaButtonRect;
        private Rectangle _exitButtonRect;
        private Rectangle _creditsButtonRect;

        private SpriteFont _font;

        private MouseState _currentMouseState;
        private MouseState _previousMouseState;

        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            _backgroundTexture = _content.Load<Texture2D>("bg");
            _buttonTexture = _content.Load<Texture2D>("button");
            _font = _content.Load<SpriteFont>("Gamefont");

            _startButtonRect = new Rectangle(500, 120, 200, 50);
            _optionsButtonRect = new Rectangle(500, 190, 200, 50);
            _gachaButtonRect = new Rectangle(500, 260, 200, 50);
            _creditsButtonRect = new Rectangle(500, 330, 200, 50);
            _exitButtonRect = new Rectangle(500, 400, 200, 50);

        }

        public override void Update(GameTime gameTime)
        {
            _currentMouseState = Mouse.GetState();

            if (_currentMouseState.LeftButton == ButtonState.Pressed &&
                _previousMouseState.LeftButton == ButtonState.Released)
            {
                Point mousePosition = _currentMouseState.Position;

                if (_startButtonRect.Contains(mousePosition))
                {
                    _game.ChangeState(new GameplayState(_game, _graphicsDevice, _content));
                }
                else if (_optionsButtonRect.Contains(mousePosition))
                {
                    //_game.ChangeState(new OptionsState(_game, _graphicsDevice, _content));
                }
                else if (_gachaButtonRect.Contains(mousePosition))
                {
                    _game.ChangeState(new GachaState(_game, _graphicsDevice, _content));
                }
                else if (_creditsButtonRect.Contains(mousePosition))
                {
                    //_game.ChangeState(new CreditsStates(_game, _graphicsDevice, _content));
                }
                else if (_exitButtonRect.Contains(mousePosition))
                {
                    _game.Exit();
                }
            }

            _previousMouseState = _currentMouseState;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(_backgroundTexture, _graphicsDevice.Viewport.Bounds, Color.White);

            DrawButton(spriteBatch, _startButtonRect, "Start");
            DrawButton(spriteBatch, _optionsButtonRect, "Options");
            DrawButton(spriteBatch, _gachaButtonRect, "Gacha");
            DrawButton(spriteBatch, _creditsButtonRect, "Credits");
            DrawButton(spriteBatch, _exitButtonRect, "Exit");
            spriteBatch.End();

        }

        private void DrawButton(SpriteBatch spriteBatch, Rectangle rect, string text)
        {
            spriteBatch.Draw(_buttonTexture, rect, Color.White);

            Vector2 textSize = _font.MeasureString(text);
            Vector2 textPosition = new Vector2(
                rect.X + (rect.Width - textSize.X) / 2,
                rect.Y + (rect.Height - textSize.Y) / 2
            );

            spriteBatch.DrawString(_font, text, textPosition, Color.Black);
        }

        public override void PostUpdate(GameTime gameTime) { }
    }
}
