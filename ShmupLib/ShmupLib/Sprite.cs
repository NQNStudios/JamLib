using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShmupLib
{
    public class Sprite
    {
        #region Fields

        public Texture2D Texture;
        public Color[] Pixels;

        protected List<Rectangle> frames = new List<Rectangle>();
        int frameWidth = 0;
        int frameHeight = 0;
        int currentFrame;
        float frameTime = 0.1f;
        float timeForCurrentFrame = 0.0f;
        
        Color tintColor = Color.White;
        
        public int CollisionRadius = 0;
        public int BoundingXPadding = 0;
        public int BoundingYPadding = 0;

        protected Vector2 location = Vector2.Zero;
        protected Vector2 velocity = Vector2.Zero;

        #endregion

        #region Constructor

        public Sprite(Vector2 location, Texture2D texture, Rectangle initialFrame, Vector2 velocity)
        {
            this.location = location;
            Texture = texture;
            Pixels = new Color[texture.Width * texture.Height];
            Texture.GetData<Color>(Pixels);
            this.velocity = velocity;
            frames.Add(initialFrame);
            frameWidth = initialFrame.Width;
            frameHeight = initialFrame.Height;
        }

        #endregion

        #region Properties

        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Color TintColor
        {
            get { return tintColor; }
            set { tintColor = value; }
        }

        public int Frame
        {
            get { return currentFrame; }
            set { currentFrame = (int)MathHelper.Clamp(value, 0, frames.Count - 1); }
        }

        public float FrameTime
        {
            get { return frameTime; }
            set { frameTime = MathHelper.Max(0, value); }
        }

        public Rectangle Source
        {
            get { return frames[currentFrame]; }
        }

        public Rectangle Destination
        {
            get
            {
                return new Rectangle((int)location.X, (int)location.Y, frameWidth, frameHeight);
            }
        }

        public Vector2 Center
        {
            get
            {
                return location + new Vector2(frameWidth / 2, frameHeight / 2);
            }
        }

        public Rectangle BoundingBoxRect
        {
            get
            {
                return new Rectangle(
                    (int)location.X + BoundingXPadding,
                    (int)location.Y + BoundingYPadding,
                    frameWidth - (BoundingXPadding * 2),
                    frameHeight - (BoundingYPadding * 2));
            }
        }

        #endregion

        #region Collision

        public bool IsBoxColliding(Rectangle OtherBox)
        {
            return BoundingBoxRect.Intersects(OtherBox);
        }

        public bool IsCircleColliding(Vector2 otherCenter, float otherRadius)
        {
            if (Vector2.Distance(Center, otherCenter) < (CollisionRadius + otherRadius))
                return true;
            else
                return false;
        }

        public bool IsPixelColliding(Sprite other)
        {
            Rectangle rect1 = BoundingBoxRect;
            Rectangle rect2 = other.BoundingBoxRect;
            if (!IsBoxColliding(rect2))
                return false;

            int top = Math.Max(rect1.Top, rect2.Top);
            int bottom = Math.Min(rect1.Bottom, rect2.Bottom);
            int left = Math.Max(rect1.Left, rect2.Bottom);
            int right = Math.Min(rect1.Right, rect2.Right);

            for (int y = top; y < bottom; y++)
                for (int x = left; x < right; x++)
                {
                    Color color1 = Pixels[(x - rect1.Left) + 
                        (y - rect1.Top) * rect1.Width];
                    Color color2 = other.Pixels[(x - rect2.Left) +
                        (y - rect2.Top) * rect2.Width];

                    return (color1.A != 0 && color2.A != 0);
                }

            return false;
        }

        #endregion

        #region Frames

        public void AddFrame(Rectangle frameRectangle)
        {
            frames.Add(frameRectangle);
        }

        public void AddFrames(int x, int y, int width, int height, int frames)
        {
            for (int i = 0; i < frames; i++)
            {
                Rectangle rect = new Rectangle(x + (width * i), y, width, height);
                AddFrame(rect);
            }
        }

        #endregion

        #region Update/Draw

        public virtual void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeForCurrentFrame += elapsed;
            if (timeForCurrentFrame >= FrameTime)
            {
                currentFrame = (currentFrame + 1) % (frames.Count);
                timeForCurrentFrame = 0.0f;
            }
            location += (velocity * elapsed);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture,
                Center,
                Source,
                tintColor,
                0f,
                new Vector2(frameWidth / 2, frameHeight / 2),
                1.0f,
                SpriteEffects.None,
                0.0f);
        }

        #endregion
    }
}
