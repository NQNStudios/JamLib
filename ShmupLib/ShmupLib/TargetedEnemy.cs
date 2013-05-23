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

        public TargetedEnemy(int health, Sprite sprite, uint collisionDamage, float speed, Entity target)
            : base(health, sprite, collisionDamage, speed)
        {
            this.target = target;
        }

        public override void Behavior()
        {
            Vector2 velocity;
            if (target.Health.IsEmpty)
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
