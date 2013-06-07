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
        PlayerIndex index;

        Texture2D barTextureBack;
        Texture2D barTextureFront;
        float statBarX = 0f;

        bool horizontalMovement;
        bool verticalMovement;
        float speed;

        Texture2D bulletTexture;
        Vector2 shotOffset;
        int bulletHits;
        uint bulletDamage;
        float bulletSpeed;
        float shotTime;
        float elapsedShot = 0f;

        public Player(PlayerIndex playerIndex, Texture2D backTexture, Texture2D frontTexture, int health, string damageSound, Sprite sprite, uint collisionDamage, float speed, bool horizontal, bool vertical, Texture2D bulletTexture, Vector2 shotOffset, int bulletHits, uint bulletDamage, float bulletSpeed, float shotTime)
            : base("Player", "Players", health, damageSound, sprite, true, collisionDamage, "Enemies")
        {
            index = playerIndex;

            OnCollision += new Action1(collideWith);
            OnDamage += new Action(damageEffect);

            horizontalMovement = horizontal;
            verticalMovement = vertical;
            this.speed = speed;

            barTextureBack = backTexture;
            barTextureFront = frontTexture;

            this.bulletTexture = bulletTexture;
            this.shotOffset = shotOffset;
            this.bulletHits = bulletHits;
            this.bulletDamage = bulletDamage;
            this.bulletSpeed = bulletSpeed;
            this.shotTime = shotTime;
        }

        #region Update

        public override void Update(GameTime gameTime, EntityManager manager)
        {
            #if WINDOWS

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

            #region Shooting

            elapsedShot += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyState.IsKeyDown(Keys.Space) && elapsedShot > shotTime)
            {
                elapsedShot = 0f;

                shoot(manager);
            }

            #endregion

            #endif

            #if XBOX

            GamePadState padState = GamePad.GetState(index);

            #region Movement

            Vector2 velocity = Vector2.Zero;

            if (horizontalMovement)
            {
                velocity.X = padState.ThumbSticks.Left.X;
            }

            if (verticalMovement)
            {
                velocity.Y = -padState.ThumbSticks.Left.Y;
            }

            velocity *= speed;

            Sprite.Velocity = velocity;

            #endregion

            #region Shooting

            elapsedShot += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (padState.IsButtonDown(Buttons.A) && elapsedShot > shotTime)
            {
                elapsedShot = 0f;

                shoot(manager);
            }

            #endregion

            #endif

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

            base.Update(gameTime, manager);
        }

        protected virtual void shoot(EntityManager manager)
        {
            Rectangle bulletFrame = new Rectangle(0, 0, bulletTexture.Width, bulletTexture.Height);
            Vector2 bulletVelocity = new Vector2(bulletSpeed, 0f);
            Sprite bulletSprite = new Sprite(Sprite.Location + shotOffset, bulletTexture, bulletFrame, 1, AnimationType.Loop);

            Projectile p = new Projectile(bulletHits, bulletSprite, bulletDamage, bulletVelocity, "Enemies");

            manager.Add(p);
        }

        #endregion

        #region Draw

        public override void Draw(SpriteBatch spriteBatch, EntityManager manager)
        {
            Sprite.Draw(spriteBatch);
        }

        public virtual void DrawBars(SpriteBatch spriteBatch)
        {
            statBarX = ScreenHelper.TitleSafeArea.X;

            DrawTheBar(spriteBatch, health, Color.Red);
        }

        protected void DrawTheBar(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, StatBar b, Color color)
        {
            int x = (int)statBarX;
#if WINDOWS
            int y = (int)(ScreenHelper.Viewport.Height - 32 - barTextureFront.Height);
#endif
#if XBOX
            int y = (int)(ScreenHelper.TitleSafeArea.Bottom - barTextureFront.Height);
#endif

            statBarX += barTextureBack.Width + barTextureBack.Width / 8;

            Rectangle backRect = new Rectangle(x, y, (int)(barTextureBack.Width * b.Fraction), barTextureBack.Height);
            Rectangle frontRect = new Rectangle(x, y, barTextureFront.Width, barTextureFront.Height);

            spriteBatch.Draw(barTextureBack, backRect, null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            spriteBatch.Draw(barTextureFront, frontRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }

        #endregion
    }
}
 