using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShmupLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SuperFishHunter
{
    public class Hunter : Player
    {
        StatBar air;
        bool airEnabled = true;
        string shotSound;

        public StatBar Air
        {
            get { return air; }
            set { air = value; }
        }

        public Hunter(Texture2D backTexture, Texture2D frontTexture, int health, string damageSound, int air, Sprite sprite, uint collisionDamage, float speed, Texture2D bulletTexture, string shotSound, Vector2 shotOffset, int bulletHits, uint bulletDamage, float bulletSpeed, float shotTime)
            : base(backTexture, frontTexture, health, damageSound, sprite, collisionDamage, speed, false, true, bulletTexture, shotOffset, bulletHits, bulletDamage, bulletSpeed, shotTime)
        {
            this.air = new StatBar(air);
            if (air == 0)
                airEnabled = false;

            this.shotSound = shotSound;

            invTime = 0.4f;
        }

        float elapsedBreath = 0;
        float breathInterval = 1;
        public override void Update(GameTime gameTime, EntityManager manager)
        {
            base.Update(gameTime, manager);

            if (airEnabled)
            {
                elapsedBreath += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (elapsedBreath >= breathInterval)
                {
                    elapsedBreath = 0;
                    air.Damage(1);

                    if (air.IsEmpty)
                    {
                        die(manager);
                        return;
                    }
                }
            }
        }

        protected override void shoot(EntityManager manager)
        {
            base.shoot(manager);

            SoundManager.Play(shotSound);
        }

        public override void Draw(SpriteBatch spriteBatch, EntityManager manager)
        {
            base.Draw(spriteBatch, manager);
        }

        public override void DrawBars(SpriteBatch spriteBatch)
        {
            base.DrawBars(spriteBatch);

            if (airEnabled)
                DrawTheBar(spriteBatch, air, Color.Blue);
        }
    }
}
