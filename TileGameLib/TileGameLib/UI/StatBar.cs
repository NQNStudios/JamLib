using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileGameLib
{
    public struct StatBar
    {
        double current;
        double max;

        public double Current
        {
            get { return current; }
        }

        public double Max
        {
            get { return max; }
        }

        public double Fraction
        {
            get { return current / max; }
        }

        public bool IsEmpty
        {
            get
            {
                return current <= 0;
            }
        }

        public void Damage(double amount)
        {
            current = Math.Max(current - amount, 0);
        }

        public void Heal(double amount)
        {
            current = Math.Min(current + amount, max);
        }

        public void Reset()
        {
            current = max;
        }

        public double SetMax(double value)
        {
            double temp = max;
            max = value;
            return temp;
        }

        public StatBar(double max)
        {
            this.max = current = max;
        }
    }
}
