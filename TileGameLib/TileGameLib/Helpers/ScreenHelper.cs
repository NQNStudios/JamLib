using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TileGameLib
{
    public static class ScreenHelper
    {
        static GraphicsDevice _graphicsDevice;
        static GraphicsDeviceManager _graphics;

        public static SpriteFont Font;
        public static Camera Camera;

        public static void Initialize(GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics)
        {
            _graphicsDevice = graphicsDevice;
            _graphics = graphics;
        }

        public static Viewport Viewport
        {
            get { return _graphicsDevice.Viewport; }
        }

        public static GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        public static GraphicsDeviceManager Graphics
        {
            get { return _graphics; }
        }
    }
}
