using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace TileGameLib
{
    public static class Utility
    {
        #region IO

        public static int[,] FromCSV(string filename)
        {
            int[,] grid;
            List<string[]> rows = new List<string[]>();

            using (StreamReader sr = new StreamReader(filename))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    rows.Add(line.Split(','));
                }

                sr.Close();
            }

            grid = new int[rows[0].Length, rows.Count()];

            for (int i = 0; i < rows.Count(); ++i)
            {
                for (int j = 0; j < rows[i].Length; ++j)
                {
                    grid[j, i] = int.Parse(rows[i][j]);
                }
            }

            return grid;
        }

        #endregion

        public static bool Vector2Intersects(Vector2 p, Rectangle r)
        {
            if (p.X < r.X || p.Y < r.Y || p.X > r.X + r.Width || p.Y > r.Y + r.Height)
                return false;

            return true;
        }

        public static bool PointIntersects(Point p, Rectangle r)
        {
            return Vector2Intersects(new Vector2(p.X, p.Y), r);
        }

        public static Rectangle Intersection(Rectangle r1, Rectangle r2)
        {
            if (r1.Intersects(r2))
            {
                int x, y;

                x = Math.Max(r1.X, r2.X);
                y = Math.Max(r1.Y, r2.Y);

                int x_overlap, y_overlap;

                x_overlap = Math.Abs(Math.Min(r1.X,r2.X) - Math.Max(r1.X,r2.X));
                y_overlap = Math.Abs(Math.Min(r1.Y,r2.Y) - Math.Max(r1.Y,r2.Y));

                return new Rectangle(x, y, x_overlap, y_overlap);
            }
            else
            {
                return Rectangle.Empty;
            }
        }

        #region Conversion

        public static Point ToPoint(Vector2 vec)
        {
            return new Point((int)vec.X, (int)vec.Y);
        }

        public static Vector2 ToVector2(Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        #endregion
    }
}
