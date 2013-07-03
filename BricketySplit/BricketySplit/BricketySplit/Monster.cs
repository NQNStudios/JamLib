using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameLibrary.Helpers;

namespace BricketySplit
{
    public class Monster
    {
        Texture2D texture;
        Vector2 location;
        Vector2 velocity;

        bool airborn = false;

        public Rectangle Bounds
        {
            get { return new Rectangle((int)location.X, (int)location.Y, texture.Width, texture.Height); }
        }

        public Monster(Texture2D texture, Vector2 location)
        {
            this.texture = texture;
            this.location = location;
        }

        public void Update(GameTime gameTime, Player player, List<Brick> bricks)
        {
     
            velocity.X = 0;

            if (!airborn)
            {
                if (player.Bounds.X < Bounds.X)
                {
                    velocity.X = -200;
                }

                if (player.Bounds.X > Bounds.X)
                {
                    velocity.X = 200;
                }
            }

            velocity.Y += 5; //Gravity

            location += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Bounds.Intersects(player.Bounds))
            {
                player.Die();
            }

            airborn = true;
            foreach (Brick brick in bricks)
            {
                if (Bounds.Intersects(brick.Bounds))
                {
                    Rectangle r = Game1.Intersection(Bounds, brick.Bounds);

                    if (r.Y <= Bounds.Y)
                    {
                        Die();
                    }

                    else if (r.Y <= brick.Bounds.Y)
                    {
                        location.Y = brick.Bounds.Y - texture.Height;
                        velocity.Y = 0;
                        airborn = false;
                    }

                    
                }
            }

            if (location.Y > ScreenHelper.Viewport.Height - texture.Height)
            {
                location.Y = ScreenHelper.Viewport.Height - texture.Height;
                airborn = false;
            }
        }

        public void Die()
        {
            location = new Vector2(200000, 0);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Bounds, Color.White);
        }
    }
}
