using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectMidterm.States
{
    public class CongratulationState : State
    {
        private Texture2D _backgroundTexture;
        private SpriteFont _font;
        private Texture2D _menuButtonTexture;
        private Rectangle _menuButtonRect;
        private GachaState _gacha;

        public CongratulationState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GachaState gacha)
            : base(game, graphicsDevice, content)
        {
            _gacha = gacha;
            _backgroundTexture = new Texture2D(graphicsDevice, 1, 1);
            _backgroundTexture.SetData(new[] { Color.Black }); // ทำให้เป็นพื้นหลังสีดำ

            _font = _content.Load<SpriteFont>("Gamefont");
            _menuButtonTexture = _content.Load<Texture2D>("button");
            _menuButtonRect = new Rectangle(300, 400, 200, 50); // ปุ่มกลับไปหน้าเมนู
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && _menuButtonRect.Contains(mouseState.Position))
            {
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content, _gacha));
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, 800, 600), Color.White);
            Vector2 textSize = _font.MeasureString("CONGRATULATIONS!");
            Vector2 textPosition = new Vector2((800 - textSize.X) / 2, 250);
            spriteBatch.DrawString(_font, "CONGRATULATIONS!", textPosition, Color.Yellow);

            spriteBatch.Draw(_menuButtonTexture, _menuButtonRect, Color.White);
            Vector2 buttonTextSize = _font.MeasureString("Back to Menu");
            Vector2 buttonTextPosition = new Vector2(_menuButtonRect.X + (_menuButtonRect.Width - buttonTextSize.X) / 2,
                                                     _menuButtonRect.Y + (_menuButtonRect.Height - buttonTextSize.Y) / 2);
            spriteBatch.DrawString(_font, "Back to Menu", buttonTextPosition, Color.Black);

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime) { }
    }
}
