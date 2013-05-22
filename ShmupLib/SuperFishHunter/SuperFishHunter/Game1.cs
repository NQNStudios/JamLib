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
            manager = new EntityManager(Content.Load<Texture2D>("Textures/Front Health"), Content.Load<Texture2D>("Textures/Back Health"));

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


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (InGame)
            {
                elapsedSec += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            float difficulty = elapsedSec / 60f;

            int mooksToSpawn = floatToInt(difficulty / 10f);
            for (int i = 0; i < mooksToSpawn; i++)
            {
                manager.Add(MakeMook());
            }

            int thugsToSpawn = floatToInt(difficulty / 100f);
            for (int i = 0; i < thugsToSpawn; i++)
            {
                manager.Add(MakeThug());
            }

            int largeToSpawn = floatToInt(difficulty / 1000f);
            for (int i = 0; i < largeToSpawn; i++)
            {
                manager.Add(MakeLarge());
            }

            // TODO: Add your update logic here
            manager.Update(gameTime);

            base.Update(gameTime);
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
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);

            manager.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        static Random r = new Random();

        #region Spawn Helpers

        public  Entity MakePlayer()
        {
            Sprite s = new Sprite(new Vector2(ScreenHelper.TitleSafeArea.X, ScreenHelper.Viewport.Height / 2 - 8), Content.Load<Texture2D>("Textures/Rogue"), new Rectangle(0, 0, 32, 32), 2, AnimationType.Loop);
            return new Player(Content.Load<Texture2D>("Textures/Player Bar Front"), Content.Load<Texture2D>("Textures/Player Bar Back"), 5, s, 1, 200f, false, true, Content.Load<Texture2D>("Textures/Bullet"), 1, 1, 200, 0.25f);
        }

        private Entity MakeMook()
        {
            float y = (float)((r.NextDouble() * 2 - 1) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), Content.Load<Texture2D>("Textures/Small Fish"), AnimationType.Loop);

            return new Enemy(1, s, 1, 100f);
        }

        private Entity MakeThug()
        {
            float y = (float)((r.NextDouble() * 2 - 1) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), Content.Load<Texture2D>("Textures/Med Fish"), AnimationType.Loop);

            return new Enemy(2, s, 2, 100f);
        }

        private Entity MakeLarge()
        {
            float y = (float)((r.NextDouble() * 2 - 1) * ScreenHelper.Viewport.Height);
            Sprite s = new Sprite(new Vector2(ScreenHelper.Viewport.Width, y), Content.Load<Texture2D>("Textures/Large Fish"), AnimationType.Loop);

            return new Enemy(3, s, 2, 100f);
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
