using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TileGameLib
{
    public enum Phase
    {
        Move = 0,
        Attack = 1,
        Finished = 2
    }

    public delegate void Behavior(Entity e);

    public class Entity
    {
        #region Fields

        public string Tag = "";
        public string Group = "";

        public StatBar Health;
        public StatBar Magic;
        public StatBar Stamina;

        int speed;
        int skill;
        int minAttackRange;
        int maxAttackRange;

        Phase phase;
        public Behavior Behavior;

        TileLayer layer;
        Point position;

        Texture2D texture;
        Texture2D iconTexture;
        Texture2D healthbarTexture;

        FrameAnimation animation;

        public EntityManager EntityManager;

        #endregion

        #region Properies

        public Phase Phase
        {
            get { return phase; }
        }

        public Point Position
        {
            get { return position; }
        }

        public bool Moving
        {
            get { return moving; }
        }

        #endregion

        #region Initialization

        public Entity(
            string tag, string group, 
            double health, double magic, double stamina, 
            int speed, int skill,
            int minAttack, int maxAttack,
            TileLayer layer, Point position, 
            Texture2D texture, Texture2D iconTexture, Texture2D healthbarTexture)
        {
            Tag = tag;
            Group = group;

            Health = new StatBar(health);
            Magic = new StatBar(magic);
            Stamina = new StatBar(stamina);

            this.speed = speed;
            this.skill = skill;

            minAttackRange = minAttack;
            maxAttackRange = maxAttack;

            this.layer = layer;
            this.position = position;

            this.texture = texture;
            this.iconTexture = iconTexture;
            this.healthbarTexture = healthbarTexture;

            animation = new FrameAnimation(2, 32, 32, 0, 0);
            animation.FramesPerSecond = 2;
        }

        #endregion

        #region Movement

        TimeSpan toMove = TimeSpan.FromSeconds(0.15);
        TimeSpan elapsed = TimeSpan.Zero;
        Point moveDest;
        Vector2 moveOffset;
        bool moving;

        Queue<Point> destinations;

        public void MoveTo(Point destination)
        {
            if (destination.X < 0 || destination.Y < 0 || destination.X >= layer.Width() || destination.Y >= layer.Height() || !layer.IsPassable(destination) || Moving)
                return;

            if (Group != "Player")
                layer.Pathfind.InitializeSearchNodes(layer);

            destinations = new Queue<Point>();

            List<Point> points = layer.Pathfind.FindPath(position, destination);

            for (int i = 0; i < points.Count(); ++i)
            {
                destinations.Enqueue(points[i]);
            }

            moveDest = destinations.Dequeue();
            moving = true;
            elapsed = toMove;
        }

        public bool CanMoveTo(Point p)
        {
            if (!layer.IsPassable(p)) return false;
            if (layer.Pathfind.MoveDistance(position, p) > speed) return false;
            if (EntityManager != null && EntityManager.EntityAt(p) != null) return false;
            return true;
        }

        public bool CanAttack(Point p)
        {
            int dist = layer.Pathfind.AttackDistance(position, p);

            return (dist >= minAttackRange && dist <= maxAttackRange);
        }

        #endregion

        #region Phase Shift

        public void EndPhase()
        {
            phase = (Phase)(MathHelper.Clamp((int)phase + 1, 0, 2));
        }

        public void EndTurn()
        {
            phase = Phase.Move;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            if (moving)
                updateMove(gameTime);

            animation.Update(gameTime);
        }

        #endregion

        #region Act

        public void Act()
        {
            if (Behavior != null)
                Behavior(this);
        }

        #endregion

        #region Helpers

        private void updateMove(GameTime gameTime)
        {
            elapsed -= gameTime.ElapsedGameTime;

            if (!layer.IsPassable(moveDest))
            {
                elapsed = TimeSpan.Zero;
                moving = false;
                moveOffset = Vector2.Zero;
            }

            if (elapsed <= TimeSpan.Zero)
            {
                position = moveDest;
                if (destinations.Count() != 0)
                {
                    moveDest = destinations.Dequeue();
                    elapsed = toMove;
                }

                else
                {
                    elapsed = TimeSpan.Zero;
                    moving = false;
                    moveOffset = Vector2.Zero;
                }
            }

            TimeSpan temp = toMove - elapsed;
            float progress = (float)temp.Ticks / toMove.Ticks;

            Vector2 offset = new Vector2(moveDest.X * layer.TileWidth, moveDest.Y * layer.TileHeight) - new Vector2(position.X * layer.TileWidth, position.Y * layer.TileHeight);//Utility.ToVector2(moveDest) - Utility.ToVector2(position);
            offset *= progress;
            moveOffset = offset;
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 loc = new Vector2(position.X * layer.TileWidth, position.Y * layer.TileHeight) + Utility.ToVector2(ScreenHelper.Viewport.TitleSafeArea.Location);

            if (moving)
                loc += moveOffset;

            spriteBatch.Draw(texture, loc, animation.CurrentRect, Color.White);

            Vector2 iconLoc = loc + new Vector2(layer.TileWidth - 12, layer.TileHeight - 12);

            spriteBatch.Draw(iconTexture, iconLoc, iconSource(phase), Color.White);

            Vector2 healthbarLoc = loc + new Vector2(2, 2);
            Rectangle backSource = new Rectangle(0, 0, 26, 4);
            Rectangle frontSource = new Rectangle(26, 0, 26, 4);

            frontSource.Width = (int)(frontSource.Width * Health.Fraction);

            spriteBatch.Draw(healthbarTexture, healthbarLoc, backSource, Color.White);
            spriteBatch.Draw(healthbarTexture, healthbarLoc, frontSource, Color.White);
        }

        private Rectangle iconSource(Phase phase)
        {
            switch (phase)
            {
                case Phase.Move:
                    return new Rectangle(0, 0, 8, 8);

                case Phase.Attack:
                    return new Rectangle(8, 0, 8, 8);

                case Phase.Finished:
                    return new Rectangle(16, 0, 8, 8);

                default:
                    return Rectangle.Empty;
            }
        }

        #endregion
    }
}
