using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;

namespace ProjectMidterm.States
{
    public class MenuState : State
    {
        private Texture2D _backgroundTexture;
        private Texture2D _buttonTexture;
        private Texture2D _catpicTexture;
        private Texture2D _logoTexture;
        private Song _backgroundMusic;

        private Rectangle _startButtonRect;
        private Rectangle _optionsButtonRect;
        private Rectangle _gachaButtonRect;
        private Rectangle _fishSelectionButtonRect;
        private Rectangle _exitButtonRect;

        private SpriteFont _font;
        private MouseState _currentMouseState;
        private MouseState _previousMouseState;
        private GachaState _gacha;

        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GachaState gacha)
            : base(game, graphicsDevice, content)
        {
            _gacha = gacha;

            _backgroundTexture = _content.Load<Texture2D>("bg");
            _catpicTexture = _content.Load<Texture2D>("cat");
            _logoTexture = _content.Load<Texture2D>("logo");
            _buttonTexture = _content.Load<Texture2D>("button");
            _font = _content.Load<SpriteFont>("Gamefont");
            _backgroundMusic = _content.Load<Song>("PokemonSoundtrack");

            MediaPlayer.Volume = OptionsState.GetSavedMusicVolume();
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_backgroundMusic);

            int buttonWidth = 200, buttonHeight = 50, startX = 550, startY = 80, buttonSpacing = 70;

            _startButtonRect = new Rectangle(startX, startY, buttonWidth, buttonHeight);
            _fishSelectionButtonRect = new Rectangle(startX, startY + buttonSpacing, buttonWidth, buttonHeight);
            _gachaButtonRect = new Rectangle(startX, startY + buttonSpacing * 2, buttonWidth, buttonHeight);
            _optionsButtonRect = new Rectangle(startX, startY + buttonSpacing * 3, buttonWidth, buttonHeight);
            _exitButtonRect = new Rectangle(startX, startY + buttonSpacing * 4, buttonWidth, buttonHeight);
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
                    _game.ChangeState(new GameplayState(_game, _graphicsDevice, _content, _gacha));
                }
                else if (_fishSelectionButtonRect.Contains(mousePosition))
                {
                    _game.ChangeState(new FishSelectionState(_game, _graphicsDevice, _content, _gacha));
                }
                else if (_gachaButtonRect.Contains(mousePosition))
                {
                    _game.ChangeState(new GachaState(_game, _graphicsDevice, _content));
                }
                else if (_optionsButtonRect.Contains(mousePosition))
                {
                    _game.ChangeState(new OptionsState(_game, _graphicsDevice, _content, _gacha));
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
            spriteBatch.Draw(_logoTexture, new Rectangle(160, 35, 200, 200), Color.White);
            spriteBatch.Draw(_catpicTexture, new Rectangle(90, 180, 400, 300), Color.White);

            DrawButton(spriteBatch, _startButtonRect, "START");
            DrawButton(spriteBatch, _fishSelectionButtonRect, "FISH SKINS");
            DrawButton(spriteBatch, _gachaButtonRect, "GACHA SKINS");
            DrawButton(spriteBatch, _optionsButtonRect, "OPTIONS");
            DrawButton(spriteBatch, _exitButtonRect, "EXIT");

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