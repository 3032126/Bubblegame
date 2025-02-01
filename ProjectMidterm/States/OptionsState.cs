using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System;

namespace ProjectMidterm.States
{
    public class OptionsState : State
    {
        private SpriteFont _font;
        private Rectangle _backButtonRect;
        private Rectangle _musicSliderRect;
        private Rectangle _sfxSliderRect;
        private MouseState _previousMouseState;
        private Texture2D _pixelTexture;
        private GachaState _gacha;

        private static float _savedMusicVolume = 1.0f;
        private static float _savedSfxVolume = 1.0f;
        private int _musicVolume;
        private int _sfxVolume;


        public OptionsState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GachaState gacha)
            : base(game, graphicsDevice, content)
        {
            _gacha = gacha ?? throw new ArgumentNullException(nameof(gacha), "GachaState is null!");

            _font = _content.Load<SpriteFont>("Gamefont");

            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            _backButtonRect = new Rectangle(20, 20, 50, 50);
            _musicSliderRect = new Rectangle(200, 150, 400, 20);
            _sfxSliderRect = new Rectangle(200, 220, 400, 20);

            _musicVolume = (int)(_savedMusicVolume * 100);
            _sfxVolume = (int)(_savedSfxVolume * 100);

        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && _backButtonRect.Contains(mouseState.Position))
            {
                _savedMusicVolume = _musicVolume / 100f;
                _savedSfxVolume = _sfxVolume / 100f;

                MediaPlayer.Volume = _savedMusicVolume;
                SoundEffect.MasterVolume = _savedSfxVolume;

                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content, _gacha));
            }

            if (mouseState.LeftButton == ButtonState.Pressed && _musicSliderRect.Contains(mouseState.Position))
            {
                float percentage = (mouseState.X - _musicSliderRect.X) / (float)_musicSliderRect.Width;
                _musicVolume = (int)Math.Clamp(percentage * 100, 0, 100);
                MediaPlayer.Volume = _musicVolume / 100f;
            }

            if (mouseState.LeftButton == ButtonState.Pressed && _sfxSliderRect.Contains(mouseState.Position))
            {
                float percentage = (mouseState.X - _sfxSliderRect.X) / (float)_sfxSliderRect.Width;
                _sfxVolume = (int)Math.Clamp(percentage * 100, 0, 100);
                SoundEffect.MasterVolume = _sfxVolume / 100f;
            }

            _previousMouseState = mouseState;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(_font, "Options", new Vector2(350, 50), Color.White);
            spriteBatch.DrawString(_font, "Music Volume: " + _musicVolume, new Vector2(200, 120), Color.White);
            spriteBatch.DrawString(_font, "SFX Volume: " + _sfxVolume, new Vector2(200, 190), Color.White);

            DrawRectangle(spriteBatch, _musicSliderRect, Color.Gray);
            DrawRectangle(spriteBatch, new Rectangle(_musicSliderRect.X, _musicSliderRect.Y, (int)(_musicVolume * _musicSliderRect.Width / 100), _musicSliderRect.Height), Color.Green);

            DrawRectangle(spriteBatch, _sfxSliderRect, Color.Gray);
            DrawRectangle(spriteBatch, new Rectangle(_sfxSliderRect.X, _sfxSliderRect.Y, (int)(_sfxVolume * _sfxSliderRect.Width / 100), _sfxSliderRect.Height), Color.Blue);

            spriteBatch.DrawString(_font, "Back", new Vector2(_backButtonRect.X + 5, _backButtonRect.Y + 5), Color.White);

            spriteBatch.End();
        }

        private void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            spriteBatch.Draw(_pixelTexture, rectangle, color);
        }

        public override void PostUpdate(GameTime gameTime) { }

        public static float GetSavedMusicVolume()
        {
            return _savedMusicVolume;
        }

        public static float GetSavedSfxVolume()
        {
            return _savedSfxVolume;
        }
    }
}
