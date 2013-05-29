using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ShmupLib
{
    public class Powerup : Entity
    {
        float speed;
        protected int amount;

        public int Amount
        {
            get { return amount; }
        }

        public Powerup(int amount, string damageSound, Sprite sprite, float speed)
            : base("", "Powerups", 1, damageSound, sprite, true, 0, "Players")
        {
            this.speed = speed;
            this.amount = amount;

            OnCollision += new Action1(collision);
        }

        public override void Update(GameTime gameTime, EntityManager manager)
        {
            Sprite.Velocity = new Vector2(-speed, 0);

            base.Update(gameTime, manager);
        }

        void collision(Entity e)
        {
            Damage(1);
        }
    }
}
