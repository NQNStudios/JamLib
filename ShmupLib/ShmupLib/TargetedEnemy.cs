using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ShmupLib
{
    public class TargetedEnemy : Enemy
    {
        Entity target;
        bool hitTarget;

        public TargetedEnemy(int health, string damageSound, Sprite sprite, uint collisionDamage, float speed, Entity target)
            : base(health, damageSound, sprite, collisionDamage, speed)
        {
            this.target = target;
            OnCollision += coll;
        }

        private void coll(Entity e)
        {
            if (e == target)
                hitTarget = true;
        }

        public override void Behavior()
        {
            Vector2 velocity;
            if (hitTarget || target.Health.IsEmpty)
            {
                velocity = new Vector2(-speed, 0);
            }
            else
            {
                velocity = target.Sprite.Location - Sprite.Location;
                velocity.Normalize();
                velocity *= speed;
            }

            Sprite.Velocity = velocity;
        }
    }
}
