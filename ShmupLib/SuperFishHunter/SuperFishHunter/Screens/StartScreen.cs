using ShmupLib.GameStates.Screens;
using ShmupLib.GameStates.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using SuperFishHunter;
using SuperFishHunter.Screens;
using System;

#if XBOX

public class StartScreen : MenuScreen
{
    public StartScreen() :
        base("")
    {
        MenuEntry enter = new MenuEntry("Press A");

        enter.Selected += entry;

        MenuEntries.Add(enter);
    }

    bool entered = false;
    PlayerIndex entryIndex;

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
 
        if (entered)
        {
            SignedInGamer gamer = Gamer.SignedInGamers[entryIndex];

            if ((gamer == null || gamer.IsGuest) && !Guide.IsVisible)
            {
                try
                {
                    Guide.ShowSignIn(4, false);
                }
                catch
                {
                    Console.WriteLine("caught ya");
                }
            }

            else
            {
                (Manager.Game as Game1).needStorageDevice = true;
                (Manager.Game as Game1).ControllingIndex = entryIndex;
                ExitScreen();
                Manager.AddScreen(new MainMenuScreen("Super Fish Hunter!"), entryIndex);
                entered = false;
            }
            return;
        }
    }

    void entry(object sender, PlayerIndexEventArgs e)
    {
        entered = true;
        entryIndex = e.PlayerIndex;
    }
}

#endif