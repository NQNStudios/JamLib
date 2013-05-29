using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShmupLib;

namespace SuperFishHunter
{
    public class Bubble : Powerup
    {
        public Bubble(int amount, Sprite sprite, float speed)
            : base(amount, "Bubble", sprite, speed)
        {
            OnCollision += new Action1(collide);
            OnDeath += bubbleSound;
        }

        void bubbleSound(Entity e)
        {
            SoundManager.Play("Bubble");
        }

        void collide(Entity e)
        {
            (e as Hunter).Air= new StatBar((e as Hunter).Air.CurrentValue + amount, (e as Hunter).Air.MaxValue);
        }
    }
}
