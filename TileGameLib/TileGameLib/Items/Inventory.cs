using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileGameLib
{
    public class Inventory
    {
        int capacity;
        List<Item> items = new List<Item>();

        public Inventory(int capacity)
        {
            this.capacity = capacity;
        }

        public Item Add(Item item)
        {
            if (items.Count() >= capacity)
            {
                return item;
            }

            else
            {
                items.Add(item);
                return null;
            }
        }

        public List<Item> Items
        {
            get { return items; }
        }
    }
}
