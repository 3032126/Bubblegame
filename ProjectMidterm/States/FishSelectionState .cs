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
        private Dictionary<string, Rectangle> _fishRects;
        private Texture2D _backgroundTexture;
        private SpriteFont _font;
        private Texture2D _lockedFishTexture;
        private Texture2D _rainbowLockedTexture;
        private Rectangle _menuButtonRect;
        private Texture2D _menuButtonTexture;
        private Texture2D _fishselecTexture;
        private Texture2D _woodTexture;

        public FishSelectionState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, GachaState gacha)
            : base(game, graphicsDevice, content)
        {
            _gacha = gacha;
            _unlockedFish = new List<string>(_gacha.UnlockedFish);

            _backgroundTexture = _content.Load<Texture2D>("bg");
            _font = _content.Load<SpriteFont>("Gamefont");
            _menuButtonTexture = _content.Load<Texture2D>("button");
            _lockedFishTexture = _content.Load<Texture2D>("locked_fish");
            _rainbowLockedTexture = _content.Load<Texture2D>("rainbow_fish");
            _fishselecTexture = _content.Load<Texture2D>("FISH_SKINS");
            _woodTexture = _content.Load<Texture2D>("wood 1");

            _menuButtonRect = new Rectangle(20, 20, 50, 50);

            _fishRects = new Dictionary<string, Rectangle>
            {
                { "Goldfish", new Rectangle(212, 80, 180, 180) },
                { "Pufferfish", new Rectangle(412, 80, 180, 180) },
                { "Rare Fish", new Rectangle(212, 280, 180, 180) },
                { "Legendary Fish", new Rectangle(412, 280, 180, 180) }
            };
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && _menuButtonRect.Contains(mouseState.Position))
            {
                _game.ChangeState(new MenuState(_game, _graphicsDevice, _content, _gacha));
            }

            foreach (var fish in _fishRects)
            {
                if (_gacha.UnlockedFish.Contains(fish.Key) && fish.Value.Contains(mouseState.Position) &&
                    mouseState.LeftButton == ButtonState.Pressed)
                {
                    _gacha.SelectFish(fish.Key);
                    _game.ChangeState(new GameplayState(_game, _graphicsDevice, _content, _gacha));
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_backgroundTexture, _graphicsDevice.Viewport.Bounds, Color.White);
            spriteBatch.Draw(_woodTexture, new Rectangle(147, 5, 500, 500), Color.White);
            spriteBatch.Draw(_menuButtonTexture, _menuButtonRect, Color.White);
            spriteBatch.Draw(_fishselecTexture, new Rectangle(263, -95, 280, 280), Color.White);

            foreach (var fish in _fishRects)
            {
                if (_gacha.UnlockedFish.Contains(fish.Key))
                {
                    spriteBatch.Draw(_gacha.GetFishTexture(fish.Key), fish.Value, Color.White);
                }
                else
                {
                    if (fish.Key == "Legendary Fish")
                        spriteBatch.Draw(_rainbowLockedTexture, fish.Value, Color.White);
                    else
                        spriteBatch.Draw(_lockedFishTexture, fish.Value, Color.White);
                }
            }

            spriteBatch.End();
        }

        public override void PostUpdate(GameTime gameTime) { }
    }
}
