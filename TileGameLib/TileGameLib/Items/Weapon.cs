using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TileGameLib
{
    public class Weapon : Item
    {
        #region Fields

        double amount;
        bool healing;

        string group;

        int minRange;
        int maxRange;

        #endregion

        #region Properties

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

        #endregion

        #region Initialization

        public Weapon(string name, string group, double amount, int minRange, int maxRange, bool healing)
            : base(name)
        {
            this.amount = amount;
            this.minRange = minRange;
            this.maxRange = maxRange;
            this.healing = healing;
            this.group = group;

            Use += new Action(attack);
        }

        public Weapon(string name, string group, double amount, int minRange, int maxRange)
            : this(name, group, amount, minRange, maxRange, false)
        {
        }

        #endregion

        #region File Initialization

        public static Weapon FromFile(string filename, string group)
        {
            string name;
            double amount;
            int min;
            int max;

            using (StreamReader sr = new StreamReader(filename))
            {
                if (sr.ReadLine() != "[Weapon]")
                    throw new ArgumentException("Not a Weapon file");

                sr.ReadLine();
                name = sr.ReadLine();
                sr.ReadLine();
                amount = double.Parse(sr.ReadLine());
                sr.ReadLine();
                min = int.Parse(sr.ReadLine());
                sr.ReadLine();
                max = int.Parse(sr.ReadLine());

                sr.Close();
            }

            Weapon w = new Weapon(name, group, amount, min, max);
            return w;
        }

        #endregion

        #region Helpers

        public bool CanTarget(Entity e)
        {
            return ((healing && e.Group == group) || (!healing && e.Group != group));
        }

        #endregion

        #region Actions

        void attack(Entity e, double multiplier)
        {
            if (!healing)
                e.Health.Damage(amount * multiplier);
            else
                e.Health.Heal(amount * multiplier);
        }

        #endregion
    }
}
