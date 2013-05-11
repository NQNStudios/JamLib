using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TileGameLib
{
    public delegate void Action(Entity e, double multiplier);

    public class Item
    {
        protected string name;

        public Action Use;

        public virtual string ToString()
        {
            return name;
        }

        public Item(string name)
        {
            this.name = name;
        }

        public void UseOn(Entity e)
        {
            UseOn(e, 1);
        }

        public void UseOn(Entity e, double multiplier)
        {
            if (Use == null)
                return;

            Use(e, multiplier);
        }

        #region File Initialization

        public static Item FromFile(string filename, string group)
        {
            string type;
            string name = "Content/Items/" + filename;
            using (StreamReader sr = new StreamReader(name))
            {
                type = sr.ReadLine();
                sr.Close();
            }

            switch (type)
            {
                case "[Weapon]":
                    return Weapon.FromFile(name, group);

                case "[Consumable]":
                    return Consumable.FromFile(name);

                default:
                    return null;
            }
        }

        #endregion
    }
}
