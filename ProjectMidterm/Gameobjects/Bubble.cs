using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ProjectMidterm.GameObjects
{
    public class Bubble
    {
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; set; }
        public bool IsFired { get; private set; }
        public string BubbleType { get; private set; }

        public Bubble(Texture2D texture, Vector2 position, string bubbleType)
        {
            Texture = texture;
            Position = position;
            BubbleType = bubbleType;
            IsFired = false;
            Velocity = Vector2.Zero;
        }

        public void Update(GameTime gameTime)
        {
            if (IsFired)
            {
                Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> bubbleTextures)
        {
            spriteBatch.Draw(bubbleTextures[BubbleType], Position, Color.White);
        }

        public void Fire(Vector2 direction, float speed)
        {
            Velocity = direction * speed;
            IsFired = true;
        }
    }
}
