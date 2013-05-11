using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TileGameLib
{
    public class Consumable : Item
    {
        #region Fields

        static Dictionary<string, Action> types = new Dictionary<string, Action>();
        static bool init = false;

        double amount;
        int count;

        #endregion

        public override string ToString()
        {
            return name + " x" + count.ToString();
        }

        #region Initialization

        public Consumable(string name, double amount, int count, string type)
            : base(name)
        {
            if (!init)
                initializeTypes();

            this.amount = amount;
            this.count = count;

            if (types.ContainsKey(type))
            {
                Use += types[type];
            }
        }

        void initializeTypes()
        {
            types.Add("Heal", new Action(heal));

            init = true;
        }

        #endregion

        #region File Initialization

        public static Consumable FromFile(string filename)
        {
            string name;
            double amount;
            int count;
            string type;

            using (StreamReader sr = new StreamReader(filename))
            {
                if (sr.ReadLine() != "[Consumable]")
                    throw new ArgumentException("Not a Consumable file");

                sr.ReadLine();
                name = sr.ReadLine();
                sr.ReadLine();
                amount = double.Parse(sr.ReadLine());
                sr.ReadLine();
                count = int.Parse(sr.ReadLine());
                sr.ReadLine();
                type = sr.ReadLine();

                sr.Close();
            }

            Consumable c = new Consumable(name, amount, count, type);
            return c;
        }

        #endregion

        #region Actions

        void heal(Entity e, double multiplier)
        {
            if (count > 0)
            {
                e.Health.Heal(amount);
                --count;
            }
        }

        #endregion
    }
}
