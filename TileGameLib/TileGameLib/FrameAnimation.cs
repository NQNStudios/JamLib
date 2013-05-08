using System;

using Microsoft.Xna.Framework;

namespace TileGameLib
{
    public class FrameAnimation
    {
        Rectangle[] frames;
        int currentFrame = 0;

        float frameLength = .5f;
        float timer = 0f;

        bool once;
        public bool Finished = false;

        public int FramesPerSecond
        {
            get
            {
                return (int)(1f / frameLength);
            }
            set
            {
                frameLength = (float)Math.Max(1f / (float)value, .01f);
            }
        }

        public Rectangle CurrentRect
        {
            get { return frames[currentFrame]; }
        }

        public int CurrentFrame
        {
            get { return currentFrame; }
            set
            {
                currentFrame = (int)MathHelper.Clamp(value, 0, frames.Length - 1);
            }
        }

        public FrameAnimation(
            int frames,
            int frameWidth,
            int frameHeight,
            int xOffset,
            int yOffset)
        {
            this.frames = new Rectangle[frames];

            for (int i = 0; i < frames; i++)
            {
                Rectangle rect = new Rectangle();
                rect.Width = frameWidth;
                rect.Height = frameHeight;

                rect.X = xOffset + (i * frameWidth);
                rect.Y = yOffset;

                this.frames[i] = rect;
            }

            once = false;
        }

        public FrameAnimation(
            int frames,
            int frameWidth,
            int frameHeight,
            int xOffset,
            int yOffset,
            bool animateOnce)
            : this(frames, frameWidth, frameHeight, xOffset, yOffset)
        {
            once = animateOnce;
        }

        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= frameLength)
            {
                timer = 0;

                if (!once)
                    currentFrame = (currentFrame + 1) % frames.Length;
                else
                {
                    ++currentFrame;
                    if (currentFrame >= frames.Length)
                        Finished = true;
                }
            }
        }
    }
}
