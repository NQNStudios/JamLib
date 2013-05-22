using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShmupLib
{
    public enum AnimationType
    {
        Loop,
        Bounce,
        Once
    }

    public class Sprite
    {
        #region Fields

        public Texture2D Texture;
        public Color[] Pixels;

        AnimationType type;
        protected List<Rectangle> frames = new List<Rectangle>();
        int frameWidth = 0;
        int frameHeight = 0;
        int currentFrame;
        float frameTime = 0.1f;
        float timeForCurrentFrame = 0.0f;

        float layer = 0f;

        Color tintColor = Color.White;
        float tintTime;
        float elapsedTint;

        SpriteEffects effect = SpriteEffects.None;
        
        public int CollisionRadius = 0;
        public int BoundingXPadding = 0;
        public int BoundingYPadding = 0;

        protected Vector2 location = Vector2.Zero;
        protected Vector2 velocity = Vector2.Zero;

        #endregion

        #region Constructor

        public Sprite(Vector2 location, Texture2D texture, Rectangle initialFrame, int frames, AnimationType animType, float layer)
        {
            this.location = location;
            Texture = texture;
            Pixels = new Color[texture.Width * texture.Height];
            Texture.GetData<Color>(Pixels);
            AddFrames(initialFrame.X, initialFrame.Y, initialFrame.Width, initialFrame.Height, frames);
            frameWidth = initialFrame.Width;
            frameHeight = initialFrame.Height;
            type = animType;
            this.layer = layer;
        }

        public Sprite(Vector2 location, Texture2D texture, Rectangle initialFrame, int frames, AnimationType animType)
            : this(location, texture, initialFrame, frames, animType, 0f)
        {
        }

        public Sprite(Vector2 location, Texture2D texture, AnimationType animType)
        {
            this.location = location;
            Texture = texture;
            Pixels = new Color[texture.Width * texture.Height];
            Texture.GetData<Color>(Pixels);
            this.AddFrame(new Rectangle(0, 0, texture.Width, texture.Height));
            frameWidth = texture.Width;
            frameHeight = texture.Height;
            type = animType;
        }

        #endregion

        #region Properties

        public bool Done
        {
            get { return type == AnimationType.Once && currentFrame == frames.Count - 1; }
        }

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

        #region Sprite Effects

        public SpriteEffects Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        /// <summary>
        /// Gives the sprite a temporary tint color.
        /// </summary>
        /// <param name="tint"></param>
        /// <param name="time">If -1, the tint will remain until a new one is applied.</param>
        public void SetTint(Color tint, float time)
        {
            elapsedTint = 0;
            tintTime = time;
            tintColor = tint;
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
            int left = Math.Max(rect1.Left, rect2.Left);
            int right = Math.Min(rect1.Right, rect2.Right);

            for (int y = top; y < bottom; y++)
                for (int x = left; x < right; x++)
                {
                    Color color1 = Pixels[(x - rect1.Left + currentFrame * Source.Width) + 
                        (y - rect1.Top) * Texture.Width];
                    Color color2 = other.Pixels[(x - rect2.Left + other.currentFrame * other.Source.Width) +
                        (y - rect2.Top) * other.Texture.Width];
                       
                    if (color1.A != 0 && color2.A != 0)
                        return true;
                }

            return false;
        }

        #endregion

        #region Frames

        public void AddFrame(Rectangle frameRectangle)
        {
            frames.Add(frameRectangle);
        }

        public void AddFrames(int x, int y, int width, int height, int frameCount)
        {
            for (int i = 0; i < frameCount; i++)
            {
                Rectangle rect = new Rectangle(x + (width * i), y, width, height);
                AddFrame(rect);
            }
        }

        #endregion

        #region Update/Draw

        int step = 1;
        public virtual void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeForCurrentFrame += elapsed;

            if (timeForCurrentFrame >= FrameTime)
            {
                switch (type)
                {
                    case AnimationType.Loop:
                        currentFrame = (currentFrame + step) % (frames.Count);
                        break;

                    case AnimationType.Bounce:
                        if (currentFrame < 0 || currentFrame >= frames.Count)
                            step *= -1;

                        currentFrame += step;
                        break;

                    case AnimationType.Once:
                        if (currentFrame < frames.Count - 1)
                            currentFrame++;
                        break;
                }
                timeForCurrentFrame = 0.0f;
            }

            if (tintColor != Color.White && tintTime != 1)
            {
                elapsedTint += elapsed;

                if (elapsedTint >= tintTime)
                {
                    tintColor = Color.White;
                    tintTime = 0f;
                    elapsedTint = 0f;
                }
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
                layer);
        }

        #endregion
    }
}
