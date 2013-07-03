using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameLibrary.Helpers;

namespace BricketySplit
{
    public class Player
    {
        Texture2D texture;
        Texture2D bulletTexture;

        Vector2 location;
        Vector2 velocity;

        bool airborn = false;

        public Rectangle Bounds
        {
            get { return new Rectangle((int)location.X, (int)location.Y, texture.Width, texture.Height); }
        }

        public Player(Texture2D texture, Texture2D bulletTexture, Vector2 location)
        {
            this.texture = texture;
            this.bulletTexture = bulletTexture;
            this.location = location;
        }

        public void Update(GameTime gameTime, List<Brick> bricks, List<Bullet> bullets)
        {
            KeyboardState keyState = Keyboard.GetState();

            velocity.X = 0;

            if (keyState.IsKeyDown(Keys.Left))
            {
                velocity.X = -250;
            }

            if (keyState.IsKeyDown(Keys.Right))
            {
                velocity.X = 250;
            }

            if (keyState.IsKeyDown(Keys.X))
            {
                if (Math.Abs(velocity.X) != 0)
                    bullets.Add(new Bullet(bulletTexture, location, new Vector2(velocity.X * 5, 0)));
            }

            if (keyState.IsKeyDown(Keys.Z) && !airborn)
            {
                velocity.Y = -500;
            }

            velocity.Y += 20; //Gravity

            location += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

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
