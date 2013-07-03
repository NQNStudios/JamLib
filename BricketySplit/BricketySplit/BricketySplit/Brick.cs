using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameLibrary.Helpers;

namespace BricketySplit
{
    public class Brick
    {
        public static int BrickWidth = 64;
        public static int BrickHeight = 16;
        public static int BrickOffset = 32;

        Texture2D texture;

        Vector2 location;
        Vector2 velocity;

        Wall wall;
        int lane;
        public int height;

        public Rectangle Bounds
        {
            get { return new Rectangle((int)location.X, (int)location.Y, BrickWidth, BrickHeight); }
        }

        public Brick(Texture2D texture, Vector2 location, Wall wall, int lane, int height)
        {
            this.texture = texture;
            this.location = location;

            velocity = new Vector2(0, 200);

            this.wall = wall;
            this.lane = lane;
            this.height = height;
        }

        public void Update(GameTime gameTime, List<Brick> otherBricks)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            location += velocity * elapsed;

            if (location.Y > ScreenHelper.Viewport.Height - BrickHeight)
            {
                location.Y = ScreenHelper.Viewport.Height - BrickHeight;

                wall.Lanes[lane].Occupied = false;

                return;
            }

            foreach (Brick brick in otherBricks)
            {
                if (Bounds != brick.Bounds && Bounds.Intersects(brick.Bounds))
                {
                    location.Y = brick.location.Y - BrickHeight;

                    wall.Lanes[lane].Occupied = false;

                    return;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Bounds, Color.White);
        }
    }
}
