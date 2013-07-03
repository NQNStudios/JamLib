using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShmupLib.GameStates.Screens;
using ShmupLib.GameStates.Input;
using Microsoft.Xna.Framework;
using ShmupLib;

namespace SuperFishHunter.Screens
{
    public class VictoryScreen : TextScreen
    {
        public VictoryScreen()
            : base("Victory!", "After all these years,", "you have finally killed the White Whale.", "Your revenge is complete.", "The sea is now free of this beast.", "", "Infinite Air Unlocked")
        {
            OnExit += OnCancel;
        }

        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            ExitScreen();
            Manager.AddScreen(new MainMenuScreen("Super Fish Hunter!"), ControllingPlayer);
            SoundManager.Play("Select");
        }
    }
}
