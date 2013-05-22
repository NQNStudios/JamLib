using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShmupLib.GameStates.Screens;
using Microsoft.Xna.Framework;
using ShmupLib.GameStates.Input;

namespace SuperFishHunter.Screens
{
    public class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen(string text)
            : base(text)
        {
            //Add menu entries
            MenuEntry playGame = new MenuEntry("Play Game");

            playGame.Selected += new EventHandler<PlayerIndexEventArgs>(PlayGame);

            MenuEntries.Add(playGame);
        }

        private void PlayGame(object sender, PlayerIndexEventArgs e)
        {
            Game1 game = (ScreenManager.Game as Game1);
            game.InGame = true;
            game.manager.Add(game.MakePlayer());
            ExitScreen();
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }
    }
}
