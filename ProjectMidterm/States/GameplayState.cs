using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ProjectMidterm.States
{
    public class GameplayState : State
    {
        private Texture2D _backgroundTexture;
        private Texture2D _fish;
        private Texture2D _bubble;

        private Vector2 _fishPosition;
        private float _fishRotation;
        private Vector2 _currentBubblePosition;
        private Vector2 _bubbleDirection;
        private List<Vector2> _grid;

        private bool _isBubbleFired;
        private int _score;

        public GameplayState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            _backgroundTexture = _content.Load<Texture2D>("bg");
            _fish = _content.Load<Texture2D>("fish");
            _bubble = _content.Load<Texture2D>("bubble");

            _fishPosition = new Vector2(400, 400);
            _fishRotation = 0f;
            _currentBubblePosition = _fishPosition;
            _bubbleDirection = Vector2.Zero;
            _isBubbleFired = false;
            _grid = new List<Vector2>();

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    _grid.Add(new Vector2(50 + x * 50, 50 + y * 50));
                }
            }

            _score = 0;
        }
        public override void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();

            // หมุนปลา (แมว)
            if (state.IsKeyDown(Keys.Left))
                _fishRotation -= 0.05f; // หมุนไปทางซ้าย
            if (state.IsKeyDown(Keys.Right))
                _fishRotation += 0.05f; // หมุนไปทางขวา

            // ยิงฟอง
            if (!_isBubbleFired && state.IsKeyDown(Keys.Space))
            {
                System.Console.WriteLine("Bubble fired!");
                _isBubbleFired = true;
                _bubbleDirection = new Vector2((float)System.Math.Sin(_fishRotation), -(float)System.Math.Cos(_fishRotation)); // กำหนดทิศทางยิง
                _currentBubblePosition = _fishPosition;
            }


            // อัปเดตตำแหน่งฟอง
            if (_isBubbleFired)
            {
                _currentBubblePosition += _bubbleDirection * 5f;

                // ตรวจสอบการชนกับ grid
                for (int i = 0; i < _grid.Count; i++)
                {
                    if (Vector2.Distance(_currentBubblePosition, _grid[i]) < 25)
                    {
                        // Debug: พิมพ์ข้อความเมื่อชน
                        System.Console.WriteLine($"Bubble hit grid at index {i}, Position: {_grid[i]}");

                        _grid.Add(_currentBubblePosition);
                        _currentBubblePosition = _fishPosition;
                        _isBubbleFired = false;
                        CheckMatch();
                        break;
                    }
                }

                // ฟองออกนอกจอ
                if (_currentBubblePosition.Y < 0 || _currentBubblePosition.X < 0 || _currentBubblePosition.X > 780)
                {
                    _currentBubblePosition = _fishPosition;
                    _isBubbleFired = false;
                }
            }
        }

        private void CheckMatch()
        {
            var visited = new HashSet<Vector2>();
            var toRemove = new List<Vector2>();

            // ตรวจสอบ Cluster ของ Bubble ที่ชน
            ExploreCluster(_currentBubblePosition, visited, toRemove);

            // Debug: แสดงขนาดของ Cluster
            System.Console.WriteLine($"Cluster size: {toRemove.Count}");

            // ลบ Bubble หาก Cluster มีขนาดมากกว่า 3
            if (toRemove.Count >= 3)
            {
                foreach (var bubble in toRemove)
                {
                    System.Console.WriteLine($"Removing bubble at position {bubble}");
                    _grid.Remove(bubble);
                }

                _score += toRemove.Count * 10; // เพิ่มคะแนนตามจำนวน Bubble ที่ลบ
            }
        }


        private void ExploreCluster(Vector2 start, HashSet<Vector2> visited, List<Vector2> cluster)
        {
            var stack = new Stack<Vector2>();
            stack.Push(FindClosest(start)); // หา Bubble ที่ใกล้ที่สุดใน Grid

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (visited.Contains(current))
                    continue;

                visited.Add(current);
                cluster.Add(current);

                // ตรวจสอบเพื่อนบ้าน
                foreach (var neighbor in GetNeighbors(current))
                {
                    if (!visited.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                    }
                }
            }
        }

        private Vector2 FindClosest(Vector2 position)
        {
            Vector2 closest = Vector2.Zero;
            float minDistance = float.MaxValue;

            foreach (var bubble in _grid)
            {
                float distance = Vector2.Distance(position, bubble);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = bubble;
                }
            }

            return closest;
        }


        private List<Vector2> GetNeighbors(Vector2 position)
        {
            var neighbors = new List<Vector2>();
            float threshold = 30f; // ปรับระยะให้เหมาะสมกับขนาด Bubble

            foreach (var bubble in _grid)
            {
                if (Vector2.Distance(bubble, position) < threshold && bubble != position)
                {
                    neighbors.Add(bubble);
                }
            }

            return neighbors;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height), Color.White);
            spriteBatch.Draw(_fish, _fishPosition, null, Color.White, _fishRotation,new Vector2(_fish.Width / 2, _fish.Height / 2), 0.2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_bubble, new Rectangle((int)_currentBubblePosition.X, (int)_currentBubblePosition.Y, 20, 20), Color.White);

            foreach (var bubble in _grid)
            {
                spriteBatch.Draw(_bubble, new Rectangle((int)bubble.X, (int)bubble.Y, 20, 20), Color.White);
            }

            spriteBatch.DrawString(_content.Load<SpriteFont>("Gamefont"), $"Score: {_score}", new Vector2(10, 10), Color.White);
            spriteBatch.End();
        }
        public override void PostUpdate(GameTime gameTime) { }
    }
}
