using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectMidterm.GameObjects
{
    public class Fish
    {
        private Texture2D _texture;
        public Vector2 Position { get; private set; }
        public float Rotation { get; private set; }

        public Fish(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;
            Rotation = 0f;
        }

        public void Update()
        {
            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Left))
                Rotation -= 0.05f;
            if (state.IsKeyDown(Keys.Right))
                Rotation += 0.05f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, null, Color.White, Rotation, new Vector2(_texture.Width / 2, _texture.Height / 2), 0.3f, SpriteEffects.None, 0f);
        }
    }
}
