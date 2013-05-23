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

        public Powerup(int amount, Sprite sprite, float speed)
            : base("", "Powerups", 1, sprite, true, 0, "Players")
        {
            this.speed = speed;
            this.amount = amount;
        }

        public override void Update(GameTime gameTime, EntityManager manager)
        {
            Sprite.Velocity = new Vector2(-speed, 0);

            base.Update(gameTime, manager);
        }
    }
}
