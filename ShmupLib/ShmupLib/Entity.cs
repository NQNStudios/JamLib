using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShmupLib
{
    public delegate void Action();
    public delegate void Action1(Entity e);
    public delegate void Action2(Entity e1, Entity e2);

    public class Entity
    {
        StatBar health;


    }
}
