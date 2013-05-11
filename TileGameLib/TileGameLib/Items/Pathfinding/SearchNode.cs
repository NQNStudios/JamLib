using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileGameLib
{
    internal class SearchNode
    {
        public Point Position;
        public bool Passable;

        public SearchNode Parent;
        public bool InOpenList;
        public bool InClosedList;

        public float DistanceToGoal;
        public float DistanceTraveled;

        public SearchNode[] Neighbors;
    }
}
