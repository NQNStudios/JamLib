using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ShmupLib
{
    public class Projectile : Entity
    {
        public Projectile(int hits, Sprite sprite, uint collisionDamage, Vector2 velocity, params string[] collisionGroups)
            : base("", "Projectiles", hits, sprite, true, collisionDamage, collisionGroups)
        {
            OnCollision += new Action1(collide);
            Sprite.Velocity = velocity;
        }

        void collide(Entity e)
        {
            Damage(1);
        }
    }
}
