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

        public StatBar Air
        {
            get { return air; }
            set { air = value; }
        }

        public Hunter(Texture2D backTexture, Texture2D frontTexture, int health, int air, Sprite sprite, uint collisionDamage, float speed, Texture2D bulletTexture, Vector2 shotOffset, int bulletHits, uint bulletDamage, float bulletSpeed, float shotTime)
            : base(backTexture, frontTexture, health, sprite, collisionDamage, speed, false, true, bulletTexture, shotOffset, bulletHits, bulletDamage, bulletSpeed, shotTime)
        {
            this.air = new StatBar(air);
        }

        float elapsedBreath = 0;
        float breathInterval = 1;
        public override void Update(GameTime gameTime, EntityManager manager)
        {
            base.Update(gameTime, manager);

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

        public override void Draw(SpriteBatch spriteBatch, EntityManager manager)
        {
            base.Draw(spriteBatch, manager);

            DrawBar(spriteBatch, air, Color.Blue, manager);
        }
    }
}
