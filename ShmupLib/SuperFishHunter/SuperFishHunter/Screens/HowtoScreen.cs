using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShmupLib.GameStates.Screens;

namespace SuperFishHunter.Screens
{
    public class HowtoScreen : TextScreen
    {
        #if WINDOWS

        public HowtoScreen()
            : base(
            "How To Play",
            "Hold arrow keys to move up and down.", 
            "Hold the spacebar to fire your weapon.",
            "Try to avoid enemies. The red bar shows your health.",
            "The blue bar shows your oxygen supply. Bubbles replenish oxygen.",
            "Collect coins. Use these to buy weapons and upgrades.",
            "Survive long enough to kill the White Whale.")
        {
            OnExit += OnCancel;
        }

        #endif

        #if XBOX

        public HowtoScreen()
            : base(
            "How To Play",
            "Move with the left stick.", 
            "Hold A to fire your weapon.",
            "Try to avoid enemies. The red bar shows your health.",
            "The blue bar shows your oxygen supply. Bubbles replenish oxygen.",
            "Collect coins. Use these to buy weapons and upgrades.",
            "Survive long enough to kill the White Whale.")
        {
            OnExit += OnCancel;
        }

#endif

        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            ExitScreen();
        }
    }
}
