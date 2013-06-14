using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShmupLib.GameStates.Screens;
using Microsoft.Xna.Framework;
using ShmupLib.GameStates.Input;
using ShmupLib.GameStates;
using System.IO;
#if XBOX
using Microsoft.Xna.Framework.Storage;
#endif

namespace SuperFishHunter.Screens
{
    public class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen(string text)
            : base(text)
        {
            //Add menu entries
            MenuEntry playGame = new MenuEntry("Play Game");
            MenuEntry shop = new MenuEntry("Shop");
            MenuEntry howtoPlay = new MenuEntry("How to Play");
            MenuEntry credits = new MenuEntry("Credits");
            MenuEntry exitGame = new MenuEntry("Exit Game");

            playGame.Selected += new EventHandler<PlayerIndexEventArgs>(PlayGame);
            shop.Selected += new EventHandler<PlayerIndexEventArgs>(Shop);
            howtoPlay.Selected += howtoplay;
            credits.Selected += openCredits;
            exitGame.Selected += OnCancel;

            MenuEntries.Add(playGame);
            MenuEntries.Add(shop);
            MenuEntries.Add(howtoPlay);
            MenuEntries.Add(credits);
            MenuEntries.Add(exitGame);

            SelectionChangeSound = "Select";
            SelectionSound = "Select";
        }

        private void PlayGame(object sender, PlayerIndexEventArgs e)
        {
            Game1 game = (Manager.Game as Game1);
            game.InGame = true;
            game.manager.Add(game.MakePlayer());
            ExitScreen();
        }

        private void Shop(object sender, PlayerIndexEventArgs e)
        {
            Manager.AddScreen(new ShopScreen(), ControllingPlayer);
        }

        private void howtoplay(object sender, PlayerIndexEventArgs e)
        {
            Manager.AddScreen(new HowtoScreen(), ControllingPlayer);
        }

        private void openCredits(object sender, PlayerIndexEventArgs e)
        {
            Manager.AddScreen(new CreditScreen(), ControllingPlayer);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            #if WINDOWS
            (Manager.Game as Game1).WriteSave();
            #endif
    
            #if XBOX

            StorageContainer c = ScreenManager.GetContainer();
            StreamWriter writer = new StreamWriter(c.OpenFile("save.txt", FileMode.Open));

            (Manager.Game as Game1).WriteTheSave(writer);

            c.Dispose();

            #endif

            Manager.Game.Exit();
        }
    }
}
