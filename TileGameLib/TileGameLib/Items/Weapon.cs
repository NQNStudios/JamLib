using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileGameLib
{
    public class Weapon : Item
    {
        double amount;
        string group;
        bool healing;

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

        public bool Healing
        {
            get { return healing; }
        }

        public Weapon(string name, string group, double amount, int minRange, int maxRange, bool healing)
            : base(name, 1)
        {
            this.amount = amount;
            this.minRange = minRange;
            this.maxRange = maxRange;
            this.healing = healing;

            Use += new Action(attack);
        }

        public Weapon(string name, string group, double amount, int minRange, int maxRange)
            : this(name, group, amount, minRange, maxRange, false)
        {
        }

        public bool CanTarget(Entity e)
        {
            return ((healing && e.Group == group) || (!healing && e.Group != group));
        }

        void attack(Entity e, double multiplier)
        {
            if (!healing)
                e.Health.Damage(amount * multiplier);
            else
                e.Health.Heal(amount * multiplier);
        }
    }
}
