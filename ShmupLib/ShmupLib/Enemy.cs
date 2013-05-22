using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ShmupLib
{
    public class Enemy : Entity
    {
        float speed;

        public Enemy(int health, Sprite sprite, uint collisionDamage, float speed)
            : base("", "Enemies", health, sprite, true, collisionDamage, "Players", "Projectiles")
        {
            this.speed = speed;
            OnCollision += new Action1(collideWith);
            OnDamage += new Action(damageEffect);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, EntityManager manager)
        {
            Behavior();

            base.Update(gameTime, manager);
        }

        public virtual void Behavior()
        {
            Sprite.Velocity = new Vector2(-speed, 0);
        }
    }
}
