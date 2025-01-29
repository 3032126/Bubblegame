using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ProjectMidterm.GameObjects
{
    public class GridManager
    {
        private Dictionary<Vector2, Bubble> _bubbleGrid;
        private int _bubbleSize = 30; // ปรับขนาดให้ตรงกับขนาดที่ใช้จริง

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
    int column = (int)Math.Round((position.X - 50) / _bubbleSize);
    int row = (int)Math.Round((position.Y - 50) / (_bubbleSize * 0.85f)); // เพิ่มระยะห่างแนวตั้ง

    // ถ้าเป็นแถวเลขคี่ ให้เลื่อนมาทางขวาครึ่งหนึ่งของ `_bubbleSize`
    float offsetX = (row % 2 == 1) ? _bubbleSize / 2 : 0;

    float x = column * _bubbleSize + 50 + offsetX;
    float y = row * (_bubbleSize * 0.85f) + 50;

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
