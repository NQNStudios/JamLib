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
        Entity owner;

        public Inventory(int capacity, Entity owner)
        {
            this.capacity = capacity;
            this.owner = owner;
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

        public void UseItem(Item item, double multiplier)
        {
            if (item is Weapon)
                owner.EquippedWeapon = item as Weapon;
            else
                item.Use(owner, multiplier);
        }

        public void UseItem(Item item)
        {
            UseItem(item, 1);
        }

        public List<Item> Items
        {
            get { return items; }
        }
    }
}
