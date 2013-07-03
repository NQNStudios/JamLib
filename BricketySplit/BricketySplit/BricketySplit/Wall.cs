using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BricketySplit
{
    public struct Lane
    {
        public int Height;
        public bool Occupied;

        public bool Offset
        {
            get { return Height % 2 == 1; }
        }
    }

    public class Wall
    {
        int maxHeight;

        int row = 1;

        public Lane[] Lanes;

        public Wall(int width, int maxHeight)
        {
            this.maxHeight = maxHeight;

            Lanes = new Lane[width];
        }

        public void AddBrick(int lane)
        {
            Lanes[lane].Height++;
            Lanes[lane].Occupied = true;
        }

        public void DropLevel()
        {
            row++;
        }

        public bool RowCompleted()
        {
            if (Occupied)
                return false;

            for (int i = 0; i < Lanes.Length; ++i)
            {
                if (Lanes[i].Height < row)
                    return false;
            }

            return true;
        }

        private bool Occupied
        {
            get
            {
                for (int i = 0; i < Lanes.Length; ++i)
                {
                    if (Lanes[i].Occupied)
                        return false;
                }

                return true;
            }
        }

        public bool CanBrickFallIn(int lane)
        {
            if (Lanes[lane].Occupied)
                return false;

            if (maxHeight != -1 && Lanes[lane].Height == maxHeight)
                return false;

            if (Lanes[lane].Height == 0)
                return true;

            int supportingLane = SupportingLane(lane);

            if (supportingLane < 0 || supportingLane == Lanes.Length)
            {
                return true;
            }

            return (Lanes[supportingLane].Height >= Lanes[lane].Height);
        }

        private int SupportingLane(int lane)
        {
            return Lanes[lane].Offset ? lane - 1 : lane + 1;
        }
    }
}
