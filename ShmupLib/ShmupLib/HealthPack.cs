using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShmupLib
{
    public class HealthPack : Powerup
    {
        public HealthPack(int amount, Sprite sprite, float speed)
            : base(amount, sprite, speed)
        {
            OnCollision += new Action1(collide);
        }

        void collide(Entity e)
        {
            e.Health = new StatBar(e.Health.CurrentValue + amount, e.Health.MaxValue);

            Damage(1);
        }
    }
}
