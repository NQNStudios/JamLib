﻿using ShmupLib.GameStates.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ShmupLib.GameStates.Screens
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can move up and
    /// down to select an entry, or cancel to back out of the screen.
    /// </summary>
    public abstract class MenuScreen : GameScreen
    {
        #region Fields

        private List<MenuEntry> menuEntries = new List<MenuEntry>();
        private int selectedEntry = 0;
        private string menuTitle;

#if XBOX || WINDOWS
        private InputAction menuUp;
        private InputAction menuDown;
        private InputAction menuSelect;
        private InputAction menuCancel;
#endif
        string selectionChangeSound = "";
        string selectionSound = "";
        string cancelSound = "";

        #endregion Fields

        #region Properties

        protected string SelectionChangeSound
        {
            get { return selectionChangeSound; }
            set { selectionChangeSound = value; }
        }

        protected string SelectionSound
        {
            get { return selectionSound; }
            set { selectionSound = value; }
        }

        protected string CancelSound
        {
            get { return cancelSound; }
            set { cancelSound = value; }
        }

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add or change the menu
        /// contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Constructs a new menu screen with the specified title.
        /// </summary>
        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            menuUp = new InputAction(
                new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
                new Keys[] { Keys.Up },
                true);

            menuDown = new InputAction(
                new Buttons[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
                new Keys[] { Keys.Down },
                true);

            menuSelect = new InputAction(
                new Buttons[] { Buttons.A, Buttons.Start },
                new Keys[] { Keys.Enter, Keys.Space },
                true);

            menuCancel = new InputAction(
                new Buttons[] { Buttons.B, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);
        }

        #endregion Initialization

        MouseState lastState;
        bool prevMouseCheck()
        {
            if (lastState == null) return false;
            return (lastState.LeftButton == ButtonState.Pressed) ? false : true;
        }

        #region Handle Input

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting or cancelling
        /// the menu.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            //Move to the previous menu entry?
            if (menuUp.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry--;

                if (!string.IsNullOrEmpty(selectionChangeSound))
                    SoundManager.Play(selectionChangeSound);

                if (selectedEntry < 0)
                {
                    selectedEntry = menuEntries.Count - 1;
                }
            }

            //Move to the next menu entry?
            if (menuDown.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry++;

                if (!string.IsNullOrEmpty(selectionChangeSound))
                    SoundManager.Play(selectionChangeSound);

                if (selectedEntry >= menuEntries.Count)
                {
                    selectedEntry = 0;
                }
            }

            if (menuSelect.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(selectedEntry, playerIndex);

                if (!string.IsNullOrEmpty(selectionSound))
                {
                    SoundManager.Play(selectionSound);
                }
            }

            else if (menuCancel.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);

                if (!string.IsNullOrEmpty(cancelSound))
                {
                    SoundManager.Play(cancelSound);
                }
            }
        }

        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }

        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        #endregion Handle Input

        #region Update & Draw

        /// <summary>
        /// Alows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            //Make the menu slide into place during transitions, using a power
            //curve to make things look more interesting (this makes the movement
            //slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            //Start at Y = 225; each X value is generated per entry
            Vector2 position = new Vector2(0f, ScreenHelper.Viewport.Height * 0.3472222222222222f);

            //update each menu entry's location in turn
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                //set the left margin
                position.X = Manager.GraphicsDevice.Viewport.Width / 2;
                position.X -= ((Manager.Font.MeasureString(menuEntry.Text).X) * menuEntry.Scale) / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                //set the entry's position
                menuEntry.Position = position;

                //Move down for the next entry the size of this entry
                position.Y += menuEntry.GetHeight(this);
            }
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
            bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            //Update each nested MenuEntry object
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(isSelected, gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Make sure the entries are in the right place before we draw them.
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = Manager.GraphicsDevice;
            SpriteBatch spriteBatch = Manager.SpriteBatch;
            SpriteFont font = Manager.Font;
            SpriteFont titleFont = Manager.TitleFont;

            spriteBatch.Begin();

            //Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            //Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, graphics.Viewport.Height * 0.1736111111111111f);
            Vector2 titleOrigin = titleFont.MeasureString(menuTitle) / 2;
            Color titleColor = Color.Blue * TransitionAlpha;
            float titleScale = 1f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(titleFont, menuTitle, titlePosition, titleColor, 0,
                titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }

        #endregion Update & Draw
    }
}