using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShmupLib.GameStates.Screens;
using ShmupLib;

namespace SuperFishHunter.Screens
{
    public class CreditScreen : TextScreen
    {
        public CreditScreen()
            : base(
            "Credits",
            "This game was made for the 7 Day Fishing Game Jam.",
            "",
            "Design & code by Nathaniel Nelson",
            "Art by Greg Mancini",
            "Sound effects made with bfxr",
            "",
            "Thanks for playing!")
        {
            OnExit += OnCancel;
        }

        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            ExitScreen();
            SoundManager.Play("Select");
        }
    }
}
