using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BricketySplit
{
    public class Bullet
    {
        Texture2D texture;
        Vector2 location;
        Vector2 velocity;

        public Rectangle Bounds
        {
            get { return new Rectangle((int)location.X, (int)location.Y, texture.Width, texture.Height); }
        }

        public Bullet(Texture2D texture, Vector2 location, Vector2 velocity)
        {
            this.texture = texture;
            this.location = location;
            this.velocity = velocity;
        }

        public void Update(GameTime gameTime, List<Monster> monsters)
        {
            location += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Monster monster in monsters)
            {
                if (Bounds.Intersects(monster.Bounds))
                {
                    monster.Die();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Bounds, Color.White);
        }
    }
}
