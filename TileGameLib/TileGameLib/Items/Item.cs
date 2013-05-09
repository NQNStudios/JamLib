using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileGameLib
{
    public delegate void Action(Entity e, double multiplier);

    public class Item
    {
        protected string name;
        protected int count;

        public Action Use;

        public Item(string name, int count)
        {
            this.name = name;
            this.count = count;
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
    }
}
