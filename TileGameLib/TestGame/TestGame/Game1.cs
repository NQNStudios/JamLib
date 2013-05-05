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
using TileGameLib;
using System.IO;
using Microsoft.Xna.Framework.Storage;

namespace TestGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Camera camera;
        TileLayer layer;
        Cursor cursor;
        Pathfinder finder;

        Entity entity;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            ScreenHelper.Initialize(GraphicsDevice, graphics);
            camera = new Camera(new Vector2(ScreenHelper.Viewport.Width, ScreenHelper.Viewport.Height), null);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            layer = TileLayer.FromFile(Content, "layer.txt");

            Texture2D cursorTexture = Content.Load<Texture2D>("UI/Cursor_1");
            Texture2D arrowTexture = Content.Load<Texture2D>("UI/Arrows");
            Texture2D overlayTexture = Content.Load<Texture2D>("UI/Tile Overlays");

            Texture2D rogue = Content.Load<Texture2D>("Sprites/Rogue");
            Texture2D iconTexture = Content.Load<Texture2D>("UI/Phase Icons");
            Texture2D healthbarTexture = Content.Load<Texture2D>("UI/Health Bar");

            cursor = new Cursor(layer, cursorTexture, arrowTexture, overlayTexture, TimeSpan.FromSeconds(0.1), new Point(20, 20));
            cursor.AddProcess(new Buttons[] { Buttons.A }, new Process(onSelect));
            cursor.AddProcess(new Buttons[] { Buttons.X }, new Process(onTab));
            cursor.AddProcess(new Buttons[] { Buttons.B }, new Process(onExit));

            finder = new Pathfinder(layer);
            entity = new Entity("", "", 5, 0, 0, 5, 0, 1, 1, layer, new Point(20, 20), rogue, iconTexture, healthbarTexture);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            // Allows the game to exit
            if (padState.Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            cursor.Update(PlayerIndex.One, gameTime, camera);
            camera.ClampToArea(layer.WidthInPixels() + ScreenHelper.Viewport.TitleSafeArea.X, layer.HeightInPixels() + ScreenHelper.Viewport.TitleSafeArea.Y);
            entity.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, camera.TransformMatrix);

            layer.Draw(spriteBatch);

            cursor.DrawSquares(spriteBatch);
            cursor.DrawArrow(spriteBatch);
            entity.Draw(spriteBatch);
            cursor.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void onTab(Cursor cursor)
        {
            if (cursor.SelectedEntity != null)
            {
                cursor.SelectedEntity.EndPhase();
            }
        }

        void onExit(Cursor cursor)
        {
            cursor.SelectedEntity = null;
        }

        void onSelect(Cursor cursor)
        {
            if (cursor.SelectedEntity != null)
            {
                switch (cursor.SelectedEntity.Phase)
                {
                    case Phase.Move:
                        if (!cursor.SelectedEntity.Moving && cursor.Location != cursor.SelectedEntity.Position && cursor.SelectedEntity.CanMoveTo(cursor.Location))
                        {
                            cursor.SelectedEntity.MoveTo(cursor.Location);
                            cursor.SelectedEntity.EndPhase();
                        }
                        break; 

                    case Phase.Attack:
                        if (cursor.SelectedEntity.CanAttack(cursor.Location))
                            cursor.SelectedEntity.EndPhase();
                        break;

                    case Phase.Finished:
                        break;
                }
                
                cursor.SelectedEntity = null;
            }
            else if (cursor.Location == entity.Position && entity.Phase != Phase.Finished)
            {
                cursor.SelectedEntity = entity;
            }
        }
    }
}
