using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ProjectMidterm.GameObjects
{
    public class GridManager
    {
        private Dictionary<Vector2, Bubble> _bubbleGrid;
        private int _bubbleSize = 32;

        public GridManager()
        {
            _bubbleGrid = new Dictionary<Vector2, Bubble>();
        }

        public bool CheckCollision(Bubble bubble)
        {
            foreach (var b in _bubbleGrid.Keys)
            {
                if (Vector2.Distance(bubble.Position, b) < _bubbleSize)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddBubbleToGrid(Bubble bubble)
        {
            Vector2 snappedPosition = GetNearestGridPosition(bubble.Position);
            if (!_bubbleGrid.ContainsKey(snappedPosition))
            {
                _bubbleGrid[snappedPosition] = bubble;
            }
        }

        private Vector2 GetNearestGridPosition(Vector2 position)
        {
            float x = (float)Math.Round((position.X - 50) / _bubbleSize) * _bubbleSize + 50;
            float y = (float)Math.Round((position.Y - 50) / _bubbleSize) * _bubbleSize + 50;

            return new Vector2(x, y);
        }

        public void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> bubbleTextures)
        {
            foreach (var bubble in _bubbleGrid)
            {
                bubble.Value.Draw(spriteBatch, bubbleTextures);
            }
        }
    }
}
