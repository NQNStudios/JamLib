using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ShmupLib;
using Action = ShmupLib.Action;
using ShmupLib.GameStates;
using SuperFishHunter.Screens;
using System.IO;

#if XBOX
using Microsoft.Xna.Framework.Storage;
#endif

namespace SuperFishHunter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #if XBOX
        private IAsyncResult result;
        bool needResult = true;
        public bool needStorageDevice = false;
        #endif

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public EntityManager manager;

        ScreenManager screenManager;

        Texture2D backdrop;

        Texture2D frontHealthBar;
        Texture2D backHealthBar;

        Texture2D playerPistol;
        Texture2D playerBarFront;
        Texture2D playerBarBack;

        Texture2D arrowTexture;
        Texture2D fireTexture;

        Texture2D bloodTexture;

        Texture2D smallFish;
        Texture2D medFish;
        Texture2D largeFish;
        Texture2D moby;

        Texture2D smallBubble;
        Texture2D medBubble;
        Texture2D bigBubble;

        Texture2D lifeSaver;
        Texture2D coin;

        public PlayerIndex ControllingIndex;

        string folderPath = @"Content/";

        public int coins = 0;

        int score = 0;
        int highScore = 0;

        public int health = 2;
        public int air = 20;
        public uint collisionDamage = 1;
        public float speed = 200f;
        public int bulletHits = 1;
        public uint bulletDamage = 1;
        public float bulletSpeed = 400f;
        public float shotTime = 0.25f;

        public int maxHealth = 20;
        public int maxAir = 100;
        public float maxSpeed = 400f;
        public int maxBulletHits = 3;
        public uint maxBulletDamage = 3;
        public float maxBulletSpeed = 600f;
        public float minShotTime = 0.15f;

        public int healthCost = 50;
        public int airCost = 100;
        public int speedCost = 150;
        public int hitsCost = 500;
        public int damageCost = 500;
        public int bulletSpeedCost = 500;
        public int shotTimeCost = 500;

        bool boss;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            Components.Add(new GamerServicesComponent(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ScreenHelper.Initialize(graphics, GraphicsDevice);
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            backdrop = Content.Load<Texture2D>("Textures/Backdrop");

            backHealthBar = Content.Load<Texture2D>("Textures/Back Health");
            frontHealthBar = Content.Load<Texture2D>("Textures/Front Health");

            playerPistol = Content.Load<Texture2D>("Textures/Diver");
            playerBarBack = Content.Load<Texture2D>("Textures/Player Bar Back");
            playerBarFront = Content.Load<Texture2D>("Textures/Player Bar Front");

            arrowTexture = Content.Load<Texture2D>("Textures/Arrow");
            fireTexture = Content.Load<Texture2D>("Textures/Fire");

            bloodTexture = Content.Load<Texture2D>("Textures/Blood");

            smallFish = Content.Load<Texture2D>("Textures/Small Fish");
            medFish = Content.Load<Texture2D>("Textures/Med Fish");
            largeFish = Content.Load<Texture2D>("Textures/Large Fish");
            moby = Content.Load<Texture2D>("Textures/Moby Dick");

            smallBubble = Content.Load<Texture2D>("Textures/Small Bubble");
            medBubble = Content.Load<Texture2D>("Textures/Medium Bubble");
            bigBubble = Content.Load<Texture2D>("Textures/Big Bubble");

            lifeSaver = Content.Load<Texture2D>("Textures/Life Saver");

            coin = Content.Load<Texture2D>("Textures/Coin Sprite");

            SoundManager.Add("Select", Content.Load<SoundEffect>("Sounds/Blip_Select6"));
            SoundManager.Add("Splode", Content.Load<SoundEffect>("Sounds/Explosion4"));
            SoundManager.Add("Damage", Content.Load<SoundEffect>("Sounds/Hit_Hurt"));
            SoundManager.Add("Shot", Content.Load<SoundEffect>("Sounds/Laser_Shoot6"));
            SoundManager.Add("Coin", Content.Load<SoundEffect>("Sounds/Pickup_Coin20"));
            SoundManager.Add("Bubble", Content.Load<SoundEffect>("Sounds/Powerup"));
            SoundManager.Add("Health", Content.Load<SoundEffect>("Sounds/Powerup6"));

            SoundManager.Volume = 1f;

            #if WINDOWS

            if (!Directory.Exists(folderPath) || !File.Exists(folderPath + "/save.txt"))
            {
                InitialSave();
            }

            ReadSave();

            #endif

            manager = new EntityManager(frontHealthBar, backHealthBar);

#if WINDOWS
            screenManager.AddScreen(new MainMenuScreen("Super Fish Hunter!"), null);
#endif

#if XBOX
            screenManager.AddScreen(new StartScreen(), null);
#endif
        }

        #region IO

        public void InitialSave()
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            using (StreamWriter writer = new StreamWriter(File.Open(folderPath + "/save.txt", FileMode.Create)))
            {
                WriteTheSave(writer);
            }
        }
        
        #if WINDOWS
        public void WriteSave()
        {
            using (StreamWriter writer = new StreamWriter(File.Open(folderPath + "/save.txt", FileMode.Create)))
            {
                WriteTheSave(writer);
            }
        }

        public void ReadSave()
        {
            using (StreamReader reader = new StreamReader(File.Open(folderPath + "/save.txt", FileMode.Open)))
            {
                ReadTheSave(reader);
            }
        }

        #endif

        public void WriteTheSave(StreamWriter writer)
        {
            writer.WriteLine(coins); //coins
            writer.WriteLine(highScore); //high score
            writer.WriteLine(boss); //boss
            writer.WriteLine(health); //health
            writer.WriteLine(air); //air
            writer.WriteLine(collisionDamage); //collisionDamage
            writer.WriteLine(speed); //speed
            writer.WriteLine(bulletHits);
            writer.WriteLine(bulletDamage);
            writer.WriteLine(bulletSpeed);
            writer.WriteLine(shotTime);
            writer.WriteLine(healthCost); //health cost
            writer.WriteLine(airCost); //air cost
            writer.WriteLine(speedCost);
            writer.WriteLine(hitsCost);
            writer.WriteLine(damageCost);
            writer.WriteLine(bulletSpeedCost);
            writer.WriteLine(shotTimeCost);

            writer.Close();
        }

        private void ReadTheSave(StreamReader reader)
        {
            coins = int.Parse(reader.ReadLine());
            highScore = int.Parse(reader.ReadLine());
            boss = bool.Parse(reader.ReadLine());
            health = int.Parse(reader.ReadLine());
            air = int.Parse(reader.ReadLine());
            collisionDamage = uint.Parse(reader.ReadLine());
            speed = float.Parse(reader.ReadLine());
            bulletHits = int.Parse(reader.ReadLine());
            bulletDamage = uint.Parse(reader.ReadLine());
            bulletSpeed = float.Parse(reader.ReadLine());
            shotTime = float.Parse(reader.ReadLine());
            healthCost = int.Parse(reader.ReadLine());
            airCost = int.Parse(reader.ReadLine());
            speedCost = int.Parse(reader.ReadLine());
            hitsCost = int.Parse(reader.ReadLine());
            damageCost = int.Parse(reader.ReadLine());
            bulletSpeedCost = int.Parse(reader.ReadLine());
            shotTimeCost = int.Parse(reader.ReadLine());

            reader.Close();
        }

        #if XBOX
        public void InitializeXbox(StorageContainer c)
        {
            if (!c.FileExists("save.txt"))
            {
                c.CreateFile("save.txt").Close();
               
                StreamWriter writer = new StreamWriter(c.OpenFile("save.txt", FileMode.Open));

                WriteTheSave(writer);
            }

            StreamReader reader = new StreamReader(c.OpenFile("save.txt", FileMode.Open));
            ReadTheSave(reader);
        }
#endif

        #endregion

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #region Update

        public bool InGame = false;
        float elapsedSec = 5f;

        float maxMooks = 0.1f;
        float maxThugs = 0.05f;
        float maxLarge = 0.025f;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            #if XBOX //Storage Device stuff

            //UPDATE
            // Set the request flag
            if (needStorageDevice)
            {
                if (!Guide.IsVisible && needResult)
                {
                    try
                    {
                        result = StorageDevice.BeginShowSelector(
                                ControllingIndex, null, null);
                        needResult = false;
                    }
                    catch
                    {  
                    }
                }

                if (result != null && result.IsCompleted)
                {
                    StorageDevice device = StorageDevice.EndShowSelector(result);
                    if (device != null && device.IsConnected)
                    {
                        ScreenManager.Storage = device;

                        StorageContainer c = ScreenManager.GetContainer();
                        
                        if (c != null)
                        {
                            InitializeXbox(c);

                            c.Dispose();
                        }

                        needStorageDevice = false;
                    }
                    else
                    {
                        result = null;
                        needResult = true;
                    }
                }
            }

            #endif

            // TODO: Add your update logic here
            manager.Update(gameTime);

            if (InGame)
            {
                elapsedSec += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (score > highScore)
                    highScore = score;

                if (Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(ControllingIndex).IsButtonDown(Buttons.Start))
                {
                    Entity e = manager.Get("Player");
                    if (e != null)
                    {
                        e.Damage((uint)e.Health.CurrentValue);
                        manager.Remove(e);
                    }
                }

                if (manager.Get("Player") == null)
                {
                    Entity e = manager.Get("Boss");
                    if (e != null)
                    {
                        boss = false;
                        return;
                    }

                    elapsedSec = 5f;
                    screenManager.AddScreen(new MainMenuScreen("Super Fish Hunter!"), ControllingIndex);
                    score = 0;
                    InGame = false;
                }
            }

            float difficulty = elapsedSec / 60f;

            int mooksToSpawn = floatToInt(Math.Min(difficulty / 30f, maxMooks));
            for (int i = 0; i < mooksToSpawn; i++)
            {
                manager.Add(MakeMook());
            }

            if (InGame)
            {
                int thugsToSpawn = floatToInt(Math.Min(difficulty / 150f, maxThugs));
                for (int i = 0; i < thugsToSpawn; i++)
                {
                    manager.Add(MakeThug());
                }

                int largeToSpawn = floatToInt(Math.Min(difficulty / 1000f, maxLarge));
                for (int i = 0; i < largeToSpawn; i++)
                {
                    manager.Add(MakeLarge());
                }

                if (elapsedSec >= 100 && !boss)
                {
                    elapsedSec = 50;
                    manager.Add(MakeBoss());
                    boss = true;
                }
            }

            int smallBub = floatToInt(0.01f);
            for (int i = 0; i < smallBub; i++)
            {
                manager.Add(MakeSmallBubble());
            }

            int medBub = floatToInt(0.001f);
            for (int i = 0; i < medBub; i++)
            {
                manager.Add(MakeMedBubble());
            }

            int bigBub = floatToInt(0.0001f);
            for (int i = 0; i < bigBub; i++)
            {
                manager.Add(MakeBigBubble());
            }

            int lifeSavers = floatToInt(0.0005f);
            for (int i = 0; i < lifeSavers; i++)
            {
                manager.Add(MakeLifeSaver());
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(backdrop, ScreenHelper.Viewport.Bounds, Color.White);

            manager.Draw(spriteBatch);
            spriteBatch.DrawString(screenManager.Font, "$" + coins.ToString(), new Vector2(ScreenHelper.TitleSafeArea.Location.X, ScreenHelper.TitleSafeArea.Location.Y), Color.Green);

            if (InGame)
            {
                Vector2 size = screenManager.Font.MeasureString("High Score: " + highScore);
                size.Y = 0;
                Vector2 position = new Vector2(ScreenHelper.TitleSafeArea.Right, ScreenHelper.TitleSafeArea.Top) - size;

                spriteBatch.DrawString(screenManager.Font, "High score: " + highScore, position, Color.Yellow);

                position.X -= screenManager.Font.MeasureString("Score: " + score + " ").X;

                spriteBatch.DrawString(screenManager.Font, "Score: " + score + " ", position, Color.White);
            }

            Hunter h = manager.Get("Player") as Hunter;
            if (h != null)
                h.DrawBars(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        static Random r = new Random();

        #region Spawn Helpers

        public Entity MakePlayer()
        {
            Vector2 offset = new Vector2(123, 45);
            string shotSound = "Shot";

            Sprite s = new Sprite(new Vector2(ScreenHelper.TitleSafeArea.X, ScreenHelper.Viewport.Height / 2 - playerPistol.Height / 2), playerPistol, AnimationType.None);

            Entity e =  new Hunter(ControllingIndex, playerBarFront, playerBarBack, health, "Damage", air, s, collisionDamage, speed, arrowTexture, shotSound, offset, bulletHits, bulletDamage, bulletSpeed, shotTime);
            e.OnDeath += new Action1(SpawnBlood);
            return e;
        }

        private Entity MakeMook()
        {
            float y = (float)((r.NextDouble()) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), smallFish, AnimationType.None);

            Entity e = new Enemy(1, "Damage", s, 1, 200f);
            e.OnDeath += new Action1(SpawnBlood); e.OnDeath += new Action1(SpawnCoin);
            return e;
        }

        private Entity MakeThug()
        {
            float y = (float)((r.NextDouble()) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), medFish, AnimationType.None);

            Entity e = new TargetedEnemy(2, "Damage", s, 2, 400, manager.Get("Player"));
            e.OnDeath += new Action1(SpawnBlood); e.OnDeath += new Action1(SpawnCoin);
            return e;
        }

        private Entity MakeLarge()
        {
            float y = (float)((r.NextDouble()) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), largeFish, AnimationType.None);

            Entity e = new Enemy(3, "Damage", s, 2, 100f);
            e.OnDeath += new Action1(SpawnWhaleBlood); e.OnDeath += new Action1(SpawnCoin);
            return e;
        }

        float sharkLoc;
        private Entity MakeShark()
        {
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, sharkLoc), medFish, AnimationType.None);
            sharkLoc += (float)medFish.Height / 2;

            Entity e = new Enemy(500, "Damage", s, 100, 400);
            e.OnDeath += new Action1(SpawnBlood);
            e.OnRemove += new Action(endGame);
            e.Group = "Sharks";
            return e;
        }

        private Entity MakeBoss()
        {
            float y = (float)(ScreenHelper.Viewport.Height / 2 - moby.Height / 2);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), moby, AnimationType.None);

            Entity e = new Enemy(70, "Damage", s, 5, 100f);
            e.Tag = "Boss";
            e.OnDeath += new Action1(SpawnBossBlood); e.OnDeath += new Action1(SpawnCoin); e.OnDeath += BossDeath;
            e.OnRemove += SpawnSharks;

            return e;
        }

        private void SpawnSharks()
        {
            if (manager.Get("Player") != null)
            {
                sharkLoc = 0f;

                for (int i = 0; i < 20; i++)
                {
                    manager.Add(MakeShark());
                }

                boss = false;
            }
        }

        private void SpawnBossBlood(Entity e)
        {
            #region Bloods

            Vector2 location = e.Sprite.Center - new Vector2(bloodTexture.Width, bloodTexture.Height) / 2;

            Sprite s = new Sprite(location, bloodTexture, AnimationType.None);
            s.Velocity = new Vector2(-200, 0);

            manager.Add(new Particle(s, 1f));

            float x = 150;
            float y = 50;

            location = e.Sprite.Center + new Vector2(x, y) - new Vector2(bloodTexture.Width, bloodTexture.Height) / 2;

            s = new Sprite(location, bloodTexture, AnimationType.None);
            s.Velocity = new Vector2(-200, 0);

            manager.Add(new Particle(s, 1f));

            location = e.Sprite.Center + new Vector2(-x, y) - new Vector2(bloodTexture.Width, bloodTexture.Height) / 2;

            s = new Sprite(location, bloodTexture, AnimationType.None);
            s.Velocity = new Vector2(-200, 0);

            manager.Add(new Particle(s, 1f));

            location = e.Sprite.Center + new Vector2(-x, -y) - new Vector2(bloodTexture.Width, bloodTexture.Height) / 2;

            s = new Sprite(location, bloodTexture, AnimationType.None);
            s.Velocity = new Vector2(-200, 0);

            manager.Add(new Particle(s, 1f));

            location = e.Sprite.Center + new Vector2(x, -y) - new Vector2(bloodTexture.Width, bloodTexture.Height) / 2;

            s = new Sprite(location, bloodTexture, AnimationType.None);
            s.Velocity = new Vector2(-200, 0);

            manager.Add(new Particle(s, 1f));

            Vector2 dist = new Vector2(x, y);
            location = e.Sprite.Center + new Vector2(0, (float)dist.Length()) - new Vector2(bloodTexture.Width, bloodTexture.Height) / 2;

            s = new Sprite(location, bloodTexture, AnimationType.None);
            s.Velocity = new Vector2(-200, 0);

            manager.Add(new Particle(s, 1f));

            location = e.Sprite.Center + new Vector2(0, -(float)dist.Length()) - new Vector2(bloodTexture.Width, bloodTexture.Height) / 2;

            s = new Sprite(location, bloodTexture, AnimationType.None);
            s.Velocity = new Vector2(-200, 0);

            manager.Add(new Particle(s, 1f));

            #endregion
        }

        private void SpawnWhaleBlood(Entity e)
        {
            SoundManager.Play("Splode");

            {
                Vector2 location = e.Sprite.Center - new Vector2(bloodTexture.Width, bloodTexture.Height) / 2;
                location.X += bloodTexture.Width / 4;

                Sprite s = new Sprite(location, bloodTexture, AnimationType.None);
                s.Velocity = new Vector2(-200, 0);

                manager.Add(new Particle(s, 1f));
            }

            {
                Vector2 location = e.Sprite.Center - new Vector2(bloodTexture.Width, bloodTexture.Height) / 2;
                location.X -= bloodTexture.Width / 4;
                
                Sprite s = new Sprite(location, bloodTexture, AnimationType.None);
                s.Velocity = new Vector2(-200, 0);

                manager.Add(new Particle(s, 1f));
            }
        }

        private void BossDeath(Entity e)
        {
            screenManager.AddScreen(new VictoryScreen(), ControllingIndex);

            if (manager.Get("Player") != null)
            {
                manager.Remove(manager.Get("Player"));
            }

            air = 0; //Infinite air
            InGame = false;
            score = 0;
            elapsedSec = 5f;
        }

        private void endGame()
        {
            if (manager.Get("Player") != null)
                manager.Remove(manager.Get("Player"));
        }

        private Entity MakeSmallBubble()
        {
            float y = (float)((r.NextDouble()) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), smallBubble, AnimationType.None);

            Entity e = new Bubble(5, s, 200f);
            return e;
        }

        private Entity MakeMedBubble()
        {
            float y = (float)((r.NextDouble()) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), medBubble, AnimationType.None);

            Entity e = new Bubble(15, s, 200f);
            return e;
        }

        private Entity MakeBigBubble()
        {
            float y = (float)((r.NextDouble()) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), bigBubble, AnimationType.None);

            Entity e = new Bubble(33, s, 200f);
            return e;
        }
        
        private Entity MakeLifeSaver()
        {
            float y = (float)((r.NextDouble()) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), lifeSaver, AnimationType.None);

            Entity e = new HealthPack(5, s, 200f);
            return e;
        }

        private void SpawnCoin(Entity e)
        {
            score += e.Health.MaxValue * 10;

            Vector2 location = e.Sprite.Center - new Vector2(coin.Width, coin.Height) / 2;

            Sprite s = new Sprite(location, coin, AnimationType.None);

            Powerup p = new Powerup(e.Health.MaxValue * 5, "Coin", s, 200f);
            p.OnDeath += new Action1(giveCoin);
            manager.Add(p);
        }

        void giveCoin(Entity e)
        {
            coins += (e as Powerup).Amount;
        }

        private void SpawnBlood(Entity e)
        {
            SoundManager.Play("Splode");

            Vector2 location = e.Sprite.Center - new Vector2(bloodTexture.Width, bloodTexture.Height) / 2;

            Sprite s = new Sprite(location, bloodTexture, AnimationType.None);
            s.Velocity = new Vector2(-200, 0);

            manager.Add(new Particle(s, 1f));
        }

        #endregion

        #region Math Helpers

        public int floatToInt(float f)
        {
            float i = (int)f;

            float c = (float)r.NextDouble();
            if (c < f - i)
                ++i;

            return (int)i;
        }
        #endregion
    }
}
