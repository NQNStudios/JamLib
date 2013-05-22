using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ShmupLib
{
    public class Player : Entity
    {
        Texture2D barTextureBack;
        Texture2D barTextureFront;
        float statBarX = 0f;

        bool horizontalMovement;
        bool verticalMovement;
        float speed;

        Texture2D bulletTexture;
        int bulletHits;
        uint bulletDamage;
        float bulletSpeed;
        float shotTime;
        float elapsedShot = 0f;

        public Player(Texture2D backTexture, Texture2D frontTexture, int health, Sprite sprite, uint collisionDamage, float speed, bool horizontal, bool vertical, Texture2D bulletTexture, int bulletHits, uint bulletDamage, float bulletSpeed, float shotTime)
            : base("Player", "Players", health, sprite, true, collisionDamage, "Enemies")
        {
            OnCollision += new Action1(collideWith);
            OnDamage += new Action(damageEffect);

            horizontalMovement = horizontal;
            verticalMovement = vertical;
            this.speed = speed;

            barTextureBack = backTexture;
            barTextureFront = frontTexture;

            this.bulletTexture = bulletTexture;
            this.bulletHits = bulletHits;
            this.bulletDamage = bulletDamage;
            this.bulletSpeed = bulletSpeed;
            this.shotTime = shotTime;
        }

        #region Update

        public override void Update(GameTime gameTime, EntityManager manager)
        {
            KeyboardState keyState = Keyboard.GetState();

            #region Movement

            Vector2 velocity = Vector2.Zero;

            if (horizontalMovement)
            {
                if (keyState.IsKeyDown(Keys.Left))
                {
                    velocity.X = -1;
                }

                if (keyState.IsKeyDown(Keys.Right))
                {
                    velocity.X = 1;
                }
            }

            if (verticalMovement)
            {
                if (keyState.IsKeyDown(Keys.Up))
                {
                    velocity.Y = -1;
                }

                if (keyState.IsKeyDown(Keys.Down))
                {
                    velocity.Y = 1;
                }
            }

            if (velocity != Vector2.Zero)
                velocity.Normalize();

            velocity *= speed;

            Sprite.Velocity = velocity;

            #endregion

            #region Clamp

            Vector2 location = Sprite.Location;
            int width = Sprite.BoundingBoxRect.Width;
            int height = Sprite.BoundingBoxRect.Height;

            if (location.X < 0)
                location.X = 0;

            if (location.Y < 0)
                location.Y = 0;

            if (location.X > ScreenHelper.Viewport.Width - width)
                location.X = ScreenHelper.Viewport.Width - width;

            if (location.Y > ScreenHelper.Viewport.Height - height)
                location.Y = ScreenHelper.Viewport.Height - height;

            Sprite.Location = location;

            #endregion

            #region Shooting

            elapsedShot += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyState.IsKeyDown(Keys.Space) && elapsedShot > shotTime)
            {
                elapsedShot = 0f;

                Rectangle bulletFrame = new Rectangle(0, 0, bulletTexture.Width, bulletTexture.Height);
                Vector2 bulletVelocity = new Vector2(bulletSpeed, 0f);
                Sprite bulletSprite = new Sprite(Sprite.Center - new Vector2(bulletTexture.Width, bulletTexture.Height) / 2, bulletTexture, bulletFrame, 1, AnimationType.Loop);

                Projectile p = new Projectile(bulletHits, bulletSprite, bulletDamage, bulletVelocity, "Enemies");

                manager.Add(p);
            }

            #endregion

            base.Update(gameTime, manager);
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch, EntityManager manager)
        {
            statBarX = 0f;

            base.Draw(spriteBatch, manager);
        }

        protected override void DrawBar(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, StatBar b, Color color, EntityManager manager)
        {
            Texture2D backTexture = barTextureBack;
            Texture2D frontTexture = barTextureFront;

            int x = (int)statBarX;
            int y = (int)(ScreenHelper.TitleSafeArea.Height - frontTexture.Height);

            statBarX += backTexture.Width + backTexture.Width / 8;

            Rectangle backRect = new Rectangle(x, y, (int)(backTexture.Width * health.Fraction), backTexture.Height);
            Rectangle frontRect = new Rectangle(x, y, frontTexture.Width, frontTexture.Height);

            spriteBatch.Draw(manager.BackBarTexture, backRect, color);
            spriteBatch.Draw(manager.FrontBarTexture, frontRect, Color.White);
        }

        #endregion
    }
}
 