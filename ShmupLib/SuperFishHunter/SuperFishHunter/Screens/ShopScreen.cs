using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShmupLib.GameStates.Screens;
using ShmupLib.GameStates.Input;
using ShmupLib.GameStates;

#if XBOX
using Microsoft.Xna.Framework.Storage;
#endif

namespace SuperFishHunter.Screens
{
    public class ShopScreen : MenuScreen
    {
        Game1 theGame;

        MenuEntry healthUpgrade;
        MenuEntry airUpgrade;
        MenuEntry speedUpgrade;
        MenuEntry bulletHits;
        MenuEntry bulletDamage;
        MenuEntry bulletSpeed;
        MenuEntry shotTime;

        MenuEntry restartEntry;

        public ShopScreen()
            : base("Shop")
        {
            healthUpgrade = new MenuEntry("");
            airUpgrade = new MenuEntry("");
            speedUpgrade = new MenuEntry("");
            bulletHits = new MenuEntry("");
            bulletDamage = new MenuEntry("");
            bulletSpeed = new MenuEntry("");
            shotTime = new MenuEntry("");

            restartEntry = new MenuEntry("Start Over");

            healthUpgrade.Selected += upgradeHealth;
            airUpgrade.Selected += upgradeAir;
            speedUpgrade.Selected += upgradeSpeed;
            bulletHits.Selected += upgradeHits;
            bulletDamage.Selected += upgradeDamage;
            bulletSpeed.Selected += upgradeBulletSpeed;
            shotTime.Selected += upgradeShotTime;

            restartEntry.Selected += startOver;

            SelectionChangeSound = "Select";
            SelectionSound = "Select";
            CancelSound = "Select";
        }

        public override void Activate()
        {
            base.Activate();

            theGame = Manager.Game as Game1;
            
            updateText();

            if (theGame.health < theGame.maxHealth)
                MenuEntries.Add(healthUpgrade);
            if (theGame.air != 0 && theGame.air < theGame.maxAir)
                MenuEntries.Add(airUpgrade);
            if (theGame.speed < theGame.maxSpeed)
                MenuEntries.Add(speedUpgrade);
            if (theGame.bulletDamage < theGame.maxBulletDamage)
                MenuEntries.Add(bulletDamage);
            if (theGame.bulletHits < theGame.maxBulletHits)
                MenuEntries.Add(bulletHits);
            if (theGame.bulletSpeed < theGame.maxBulletSpeed)
                MenuEntries.Add(bulletSpeed);
            if (theGame.shotTime > theGame.minShotTime)
                MenuEntries.Add(shotTime);

            //MenuEntries.Add(restartEntry);
        }

        void updateText()
        {
            healthUpgrade.Text = "Upgrade Health: $" + theGame.healthCost;
            airUpgrade.Text = "Upgrade Air: $" + theGame.airCost;
            speedUpgrade.Text = "Upgrade Speed: $" + theGame.speedCost;
            bulletHits.Text = "Upgrade Bullet Penetration: $" + theGame.hitsCost;
            bulletDamage.Text = "Upgrade Bullet Damage: $" + theGame.damageCost;
            bulletSpeed.Text = "Upgrade Bullet Speed: $" + theGame.bulletSpeedCost;
            shotTime.Text = "Upgrade Fire Rate: $" + theGame.shotTimeCost;
        }

        void upgradeHealth(object sender, PlayerIndexEventArgs e)
        {
            if (theGame.coins >= theGame.healthCost)
            {
                theGame.health += 2;
                theGame.coins -= theGame.healthCost;

                theGame.healthCost += theGame.healthCost / 2;

                if (theGame.health >= theGame.maxHealth)
                    MenuEntries.Remove(healthUpgrade);

                updateText();
            }
        }

        void upgradeAir(object sender, PlayerIndexEventArgs e)
        {
            if (theGame.coins >= theGame.airCost)
            {
                theGame.air += 10;
                theGame.coins -= theGame.airCost;

                theGame.airCost += theGame.airCost / 2;

                if (theGame.air >= theGame.maxAir)
                    MenuEntries.Remove(airUpgrade);

                updateText();
            }
        }

        void upgradeSpeed(object sender, PlayerIndexEventArgs e)
        {
            if (theGame.coins >= theGame.speedCost)
            {
                theGame.speed += 50;
                theGame.coins -= theGame.speedCost;

                theGame.speedCost += theGame.speedCost / 2;

                if (theGame.speed >= theGame.maxSpeed)
                    MenuEntries.Remove(speedUpgrade);

                updateText();
            }
        }

        void upgradeHits(object sender, PlayerIndexEventArgs e)
        {
            if (theGame.coins >= theGame.hitsCost)
            {
                theGame.bulletHits += 1;
                theGame.coins -= theGame.hitsCost;

                theGame.hitsCost += theGame.hitsCost / 2;

                if (theGame.bulletHits >= theGame.maxBulletHits)
                    MenuEntries.Remove(bulletHits);

                updateText();
            }
        }

        void upgradeDamage(object sender, PlayerIndexEventArgs e)
        {
            if (theGame.coins >= theGame.damageCost)
            {
                theGame.bulletDamage += 1;
                theGame.coins -= theGame.damageCost;

                theGame.damageCost += theGame.damageCost / 2;

                if (theGame.bulletDamage >= theGame.maxBulletDamage)
                    MenuEntries.Remove(bulletDamage);

                updateText();
            }
        }

        void upgradeBulletSpeed(object sender, PlayerIndexEventArgs e)
        {
            if (theGame.coins >= theGame.bulletSpeedCost)
            {
                theGame.bulletSpeed += 50f;
                theGame.coins -= theGame.bulletSpeedCost;

                theGame.bulletSpeedCost += theGame.bulletSpeedCost / 2;

                if (theGame.bulletSpeed >= theGame.maxBulletSpeed)
                    MenuEntries.Remove(bulletSpeed);

                updateText();
            }
        }

        void upgradeShotTime(object sender, PlayerIndexEventArgs e)
        {
            if (theGame.coins >= theGame.shotTimeCost)
            {
                theGame.shotTime -= 0.05f;
                theGame.coins -= theGame.shotTimeCost;

                theGame.shotTimeCost += theGame.shotTimeCost / 2;

                if (theGame.shotTime <= theGame.minShotTime)
                    MenuEntries.Remove(shotTime);

                updateText();
            }
        }

        void startOver(object sender, PlayerIndexEventArgs e)
        {
            #if WINDOWS
            theGame.InitialSave();

            theGame.ReadSave();
            #endif

            #if XBOX
            StorageContainer c = ScreenManager.GetContainer();

            theGame.InitializeXbox(c);

            c.Dispose();
            #endif

            updateText();
        }
    }
}
