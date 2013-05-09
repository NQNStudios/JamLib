using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileGameLib
{
    public class Weapon : Item
    {
        double damage;
        int minRange;
        int maxRange;

        public int MinRange
        {
            get { return minRange; }
        }

        public int MaxRange
        {
            get { return maxRange; }
        }

        public Weapon(string name, double damage, int minRange, int maxRange)
            : base(name, 1)
        {
            this.damage = damage;
            this.minRange = minRange;
            this.maxRange = maxRange;

            Use += new Action(attack);
        }

        void attack(Entity e, double multiplier)
        {
            e.Health.Damage(damage * multiplier);
        }
    }
}
