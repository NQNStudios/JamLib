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

namespace SuperFishHunter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public EntityManager manager;

        ScreenManager screenManager;

        Texture2D frontHealthBar;
        Texture2D backHealthBar;

        Texture2D playerTexture;
        Texture2D playerBarFront;
        Texture2D playerBarBack;

        Texture2D arrowTexture;

        Texture2D bloodTexture;

        Texture2D smallFish;
        Texture2D medFish;
        Texture2D largeFish;
        Texture2D moby;

        Texture2D smallBubble;
        Texture2D medBubble;
        Texture2D bigBubble;

        Texture2D lifeSaver;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
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

            backHealthBar = Content.Load<Texture2D>("Textures/Back Health");
            frontHealthBar = Content.Load<Texture2D>("Textures/Front Health");

            playerTexture = Content.Load<Texture2D>("Textures/Diver");
            playerBarBack = Content.Load<Texture2D>("Textures/Player Bar Back");
            playerBarFront = Content.Load<Texture2D>("Textures/Player Bar Front");

            arrowTexture = Content.Load<Texture2D>("Textures/Arrow");

            bloodTexture = Content.Load<Texture2D>("Textures/Blood");

            smallFish = Content.Load<Texture2D>("Textures/Small Fish");
            medFish = Content.Load<Texture2D>("Textures/Med Fish");
            largeFish = Content.Load<Texture2D>("Textures/Large Fish");
            moby = Content.Load<Texture2D>("Textures/Moby Dick");

            smallBubble = Content.Load<Texture2D>("Textures/Small Bubble");
            medBubble = Content.Load<Texture2D>("Textures/Medium Bubble");
            bigBubble = Content.Load<Texture2D>("Textures/Big Bubble");

            lifeSaver = Content.Load<Texture2D>("Textures/Life Saver");

            manager = new EntityManager(frontHealthBar, backHealthBar);

            screenManager.AddScreen(new MainMenuScreen("Super Fish Hunter!"), PlayerIndex.One);
        }

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

            if (InGame)
            {
                elapsedSec += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    manager.Remove(manager.Get("Player"));
                }

                if (manager.Get("Player") == null)
                {
                    elapsedSec = 5f;
                    screenManager.AddScreen(new MainMenuScreen("Super Fish Hunter!"), PlayerIndex.One);
                    InGame = false;
                }
            }

            float difficulty = elapsedSec / 60f;

            int mooksToSpawn = floatToInt(Math.Min(difficulty / 20f, maxMooks));
            for (int i = 0; i < mooksToSpawn; i++)
            {
                manager.Add(MakeMook());
            }

            if (InGame)
            {
                int thugsToSpawn = floatToInt(Math.Min(difficulty / 100f, maxThugs));
                for (int i = 0; i < thugsToSpawn; i++)
                {
                    manager.Add(MakeThug());
                }

                int largeToSpawn = floatToInt(Math.Min(difficulty / 1000f, maxLarge));
                for (int i = 0; i < largeToSpawn; i++)
                {
                    manager.Add(MakeLarge());
                }
            }

            int smallBub = floatToInt(0.005f);
            for (int i = 0; i < smallBub; i++)
            {
                manager.Add(MakeSmallBubble());
            }

            int medBub = floatToInt(0.0005f);
            for (int i = 0; i < medBub; i++)
            {
                manager.Add(MakeMedBubble());
            }

            int bigBub = floatToInt(0.00005f);
            for (int i = 0; i < bigBub; i++)
            {
                manager.Add(MakeBigBubble());
            }

            int lifeSavers = floatToInt(0.0005f);
            for (int i = 0; i < lifeSavers; i++)
            {
                manager.Add(MakeLifeSaver());
            }

            // TODO: Add your update logic here
            manager.Update(gameTime);
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

            manager.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        static Random r = new Random();

        #region Spawn Helpers

        public  Entity MakePlayer()
        {
            Sprite s = new Sprite(new Vector2(ScreenHelper.TitleSafeArea.X, ScreenHelper.Viewport.Height / 2 - 8), playerTexture, AnimationType.None);

            Entity e =  new Hunter(playerBarFront, playerBarBack, 5, 20, s, 1, 200f, arrowTexture, new Vector2(123, 45), 1, 1, 400, 0.25f);
            e.OnDeath += new Action1(SpawnBlood);
            return e;
        }

        private Entity MakeMook()
        {
            float y = (float)((r.NextDouble()) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), smallFish, AnimationType.None);

            Entity e = new Enemy(1, s, 1, 200f);
            e.OnDeath += new Action1(SpawnBlood);
            return e;
        }

        private Entity MakeThug()
        {
            float y = (float)((r.NextDouble()) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), medFish, AnimationType.None);

            Entity e = new TargetedEnemy(2, s, 2, 400, manager.Get("Player"));
            e.OnDeath += new Action1(SpawnBlood);
            return e;
        }

        private Entity MakeLarge()
        {
            float y = (float)((r.NextDouble()) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), largeFish, AnimationType.None);

            Entity e = new Enemy(3, s, 2, 100f);
            e.OnDeath += new Action1(SpawnBlood);
            return e;
        }

        private Entity MakeSmallBubble()
        {
            float y = (float)((r.NextDouble()) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), smallBubble, AnimationType.None);

            Entity e = new Bubble(1, s, 200f);
            return e;
        }

        private Entity MakeMedBubble()
        {
            float y = (float)((r.NextDouble()) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), medBubble, AnimationType.None);

            Entity e = new Bubble(5, s, 200f);
            return e;
        }

        private Entity MakeBigBubble()
        {
            float y = (float)((r.NextDouble()) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), bigBubble, AnimationType.None);

            Entity e = new Bubble(20, s, 200f);
            return e;
        }
        
        private Entity MakeLifeSaver()
        {
            float y = (float)((r.NextDouble()) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), lifeSaver, AnimationType.None);

            Entity e = new HealthPack(5, s, 200f);
            return e;
        }

        private void SpawnBlood(Entity e)
        {
            Vector2 location = e.Sprite.Center - new Vector2(bloodTexture.Width, bloodTexture.Height) / 2;

            Sprite s = new Sprite(location, bloodTexture, AnimationType.None);

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
