

using Microsoft.Xna.Framework;
using System;

namespace ShmupLib.GameStates.Input
{
    /// <summary>
    /// Custom event argument which includes the index of the player who triggered
    /// the event.
    /// </summary>
    public class PlayerIndexEventArgs : EventArgs
    {
        private PlayerIndex playerIndex;

        public PlayerIndexEventArgs(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }

        /// <summary>
        /// Gets the index of the player who triggered this event.
        /// </summary>
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }
    }
}