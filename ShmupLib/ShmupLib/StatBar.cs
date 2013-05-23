using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShmupLib
{
    public struct StatBar
    {
        #region Fields

        private int current;
        private int max;

        #endregion

        #region Properties

        /// <summary>
        /// The current value of the stat bar.
        /// </summary>
        public int CurrentValue
        {
            get { return current; }
            set
            {
                current = value;
                if (current < 0)
                    current = 0;
                if (current > MaxValue)
                    current = MaxValue;
            }
        }

        /// <summary>
        /// The maximum value of the stat bar.
        /// </summary>
        public int MaxValue
        {
            get { return max; }
            set
            {
                max = value;
                if (max < 0)
                    max = 0;
                if (CurrentValue > max)
                    CurrentValue = max;
            }
        }

        public double Fraction
        {
            get { return (double)current / max; }
        }

        public bool IsEmpty
        {
            get { return current <= 0; }
        }

        #endregion

        #region Constructor

        public StatBar(int value)
        {
            max = 0;
            current = 0;
            MaxValue = value;
            CurrentValue = value;
        }

        public StatBar(int current, int max)
        {
            this.max = 0;
            this.current = 0;
            MaxValue = max;
            CurrentValue = current;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds to the current value of the stat bar.
        /// </summary>
        /// <param name="value">The amount to add.</param>
        public void Heal(uint value)
        {
            CurrentValue += (int)value;
        }

        /// <summary>
        /// Subtracts from the current value of the stat bar.
        /// </summary>
        /// <param name="value">The amount to subtract.</param>
        public void Damage(uint value)
        {
            CurrentValue -= (int)value;
        }

        #endregion
    }
}
