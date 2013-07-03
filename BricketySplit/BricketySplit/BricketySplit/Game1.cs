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
using GameLibrary.Helpers;
using System.IO;

namespace BricketySplit
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static Rectangle Intersection(Rectangle r1, Rectangle r2)
        {
            if (r1.Intersects(r2))
            {
                int x, y;

                x = Math.Max(r1.X, r2.X);
                y = Math.Max(r1.Y, r2.Y);

                int x_overlap, y_overlap;

                x_overlap = Math.Abs(Math.Min(r1.X, r2.X) - Math.Max(r1.X, r2.X));
                y_overlap = Math.Abs(Math.Min(r1.Y, r2.Y) - Math.Max(r1.Y, r2.Y));

                return new Rectangle(x, y, x_overlap, y_overlap);
            }
            else
            {
                return Rectangle.Empty;
            }
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D brickTexture;
        Texture2D playerTexture;
        Texture2D bulletTexture;
        Texture2D monsterTexture;

        List<Brick> bricks = new List<Brick>();
        List<Monster> monsters = new List<Monster>();
        List<Bullet> bullets = new List<Bullet>();
        Player player;

        Wall wall;
        Camera camera;

        float elapsed = 0f;
        float elapsedMonster = 0f;
        
        float brickTime = 0.5f;
        float monsterTime = 1.5f;

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
            ScreenHelper.Initialize(graphics, GraphicsDevice);

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
            brickTexture = Content.Load<Texture2D>("Brick");
            playerTexture = Content.Load<Texture2D>("Player");
            monsterTexture = Content.Load<Texture2D>("Monster");
            bulletTexture = Content.Load<Texture2D>("Bullet");

            player = new Player(playerTexture, bulletTexture, new Vector2(ScreenHelper.Viewport.Width / 2 - playerTexture.Width / 2, ScreenHelper.Viewport.Height - playerTexture.Height));
            wall = new Wall(13, -1);

            camera = new Camera(GraphicsDevice);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            player.Update(gameTime, bricks, bullets);

            elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsed >= brickTime)
            {
                elapsed = 0f;

                SpawnBrick();
            }

            elapsedMonster += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedMonster >= monsterTime)
            {
                elapsedMonster = 0f;

                SpawnMonster();
            }

            foreach (Brick brick in bricks)
            {
                brick.Update(gameTime, bricks);
            }

            foreach (Monster monster in monsters)
            {
                monster.Update(gameTime, player, bricks);
            }

            foreach (Bullet bullet in bullets)
            {
                bullet.Update(gameTime, monsters);
            }

            if (wall.RowCompleted())
            {
                //for (int i = bricks.Count() - 1; i >= 0; i--)
                //{
                //    if (bricks[i].height == 1)
                //    {
                //        bricks.Remove(bricks[i]);
                //    }
                //}

                //foreach (Brick brick in bricks)
                //{
                //    brick.height = Math.Max(brick.height - 1, 0);
                //}

                //Lane.ToggleRemainder();
                //wall.DropLevel();
                camera.MoveCamera(new Vector2(0, -Brick.BrickHeight));
            }

            base.Update(gameTime);
        }

        static Random r = new Random();
        private void SpawnBrick()
        {
            int lane = -1;

            while (lane == -1)
            {
                int num = r.Next(wall.Lanes.Length);

                if (wall.CanBrickFallIn(num))
                    lane = num;
            }

            if (lane == -1)
                return;

            wall.AddBrick(lane);

            Vector2 location = new Vector2(-32 + lane * Brick.BrickWidth, -Brick.BrickHeight);

            if (wall.Lanes[lane].Offset)
            {
                location.X += Brick.BrickOffset;
            }

            Brick brick = new Brick(brickTexture, location, wall, lane, wall.Lanes[lane].Height);
            bricks.Add(brick);
        }

        private void SpawnMonster()
        {
            int x = r.Next(ScreenHelper.Viewport.Width);

            Monster monster = new Monster(monsterTexture, new Vector2(x, -32));
            
            monsters.Add(monster);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            player.Draw(spriteBatch);

            foreach (Brick brick in bricks)
            {
                brick.Draw(spriteBatch);
            }

            foreach (Monster monster in monsters)
            {
                monster.Draw(spriteBatch);
            }

            foreach (Bullet bullet in bullets)
            {
                bullet.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
