using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TileGameLib
{
    /// <summary>
    /// Class for tracking the area on the tile map that should be drawn to the screen.
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// The top left point to be drawn to the screen.
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        Vector2 size;

        public Camera(Vector2 size, Vector2? position)
        {
            this.size = size;
            if (position.HasValue)
                Position = position.Value;
        }

        public Matrix TransformMatrix
        {
            get { return Matrix.CreateTranslation(new Vector3(-Position, 0f)); }
        }

        #region Methods

        public void ClampToArea(int width, int height)
        {
            if (Position.X > width - size.X)
                Position.X = width - size.X;

            if (Position.Y > height - size.Y)
                Position.Y = height - size.Y;

            if (Position.X < 0)
                Position.X = 0;

            if (Position.Y < 0)
                Position.Y = 0;
        }

        public bool IsOnScreen(Rectangle rect)
        {
            Rectangle screen = new Rectangle((int)Position.X, (int)Position.Y, (int)size.X, (int)size.Y);

            return screen.Contains(rect);
        }

        #endregion
    }
}