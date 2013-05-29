using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ShmupLib.GameStates.Input;
using Microsoft.Xna.Framework.Input;

namespace ShmupLib.GameStates.Screens
{
    public class TextScreen : GameScreen
    {
        string title;
        string[] lines;

        InputAction menuCancel;

        public TextScreen(string title, params string[] lines)
        {
            this.title = title;
            this.lines = lines;

            menuCancel = new InputAction(
                new Buttons[] { Buttons.A, Buttons.Start, Buttons.B, Buttons.Back },
                new Keys[] { Keys.Escape, Keys.Space },
                true);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            base.HandleInput(gameTime, input);
            
            PlayerIndex idx;

            if (menuCancel.Evaluate(input, null, out idx))
            {
                CallExit();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            //Make the menu slide into place during transitions, using a power
            //curve to make things look more interesting (this makes the movement
            //slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            Vector2 titlePosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height * 0.1736111111111111f);
            Vector2 titleOrigin = ScreenManager.TitleFont.MeasureString(title) / 2;
            Color titleColor = Color.Blue * TransitionAlpha;
            float titleScale = 1f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(ScreenManager.TitleFont, title, titlePosition, titleColor, 0,
                titleOrigin, titleScale, SpriteEffects.None, 0);

            Vector2 position = new Vector2(0f, ScreenHelper.Viewport.Height * 0.3472222222222222f);
            
            for (int i = 0; i < lines.Length; i++)
            {
                //set the left margin
                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2;
                position.X -= ((ScreenManager.Font.MeasureString(lines[i]).X)) / 2;

                spriteBatch.DrawString(ScreenManager.Font, lines[i], position, Color.White);

                position.Y += ScreenManager.Font.LineSpacing;
            }

            spriteBatch.End();
        }
    }
}
