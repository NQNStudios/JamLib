using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        public string TargetGroup = "";

        public StatBar Health;
        int skill;

        int speed;

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

        #region Inventory

        public bool InvMode = false;
        int selectedItem = 0;
        Inventory inventory;
        Weapon weapon;

        public Inventory Items
        {
            get { return inventory; }
        }

        public Weapon EquippedWeapon
        {
            get { return weapon; }
            set { weapon = value; }
        }

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

        public bool Attacking
        {
            get { return attacking; }
        }

        #endregion

        #region Initialization

        public Entity(
            string tag, string group, string targetGroup,
            double health,
            int speed, int skill,
            TileLayer layer, Point position, 
            Texture2D texture, Texture2D iconTexture, Texture2D healthbarTexture)
        {
            Tag = tag;
            Group = group;
            TargetGroup = targetGroup;

            Health = new StatBar(health);

            this.speed = speed;
            this.skill = skill;

            this.inventory = new Inventory(5, this);

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
        TimeSpan toAttack = TimeSpan.FromSeconds(0.3);
        TimeSpan elapsed = TimeSpan.Zero;
        Point moveDest;
        Vector2 moveOffset;
        bool moving;
        bool attacking;

        Queue<Point> destinations;

        public void MoveTo(Point destination)
        {
            if (destination == Position || destination.X < 0 || destination.Y < 0 || destination.X >= layer.Width() || destination.Y >= layer.Height() || !layer.IsPassable(destination) || Moving)
                return;

            destinations = new Queue<Point>();

            List<Point> points = layer.Pathfind.FindPath(position, destination);

            for (int i = 0; i < points.Count(); ++i)
            {
                if (layer.Pathfind.MoveDistance(position, points[i]) <= speed)
                    destinations.Enqueue(points[i]);
                else
                    break;
            }

            moveDest = destinations.Dequeue();
            moving = true;
            elapsed = toMove;
        }

        public void MoveTo(Entity target)
        {
            Point destination = target.Position;

            if (destination.X < 0 || destination.Y < 0 || destination.X >= layer.Width() || destination.Y >= layer.Height() || !layer.IsPassable(destination) || Moving || CanAttack(destination))
                return;

            Point closest = new Point(-1, -1);
            int dist = int.MaxValue;
            foreach (Point p in MovePoints())
            {
                if (CanAttackFrom(p, destination) && layer.Pathfind.MoveDistance(position, p) < dist)
                {
                    closest = p;
                    dist = layer.Pathfind.MoveDistance(position, closest);
                }
            }

            if (closest != new Point(-1, -1))
            {
                MoveTo(closest);
                return;
            }
            MoveTo(destination);
        }

        public bool CanMoveTo(Point p)
        {
            if (Position == p) return true;
            if (!layer.IsPassable(p)) return false;
            if (layer.Pathfind.MoveDistance(position, p) > speed) return false;
            if (EntityManager != null && EntityManager.EntityAt(p) != null) return false;
            return true;
        }

        public bool CanAttackFrom(Point me, Point enemy)
        {
            if (weapon == null)
                return false;

            int dist = layer.Pathfind.AttackDistance(me, enemy);

            return (layer.IsPassable(enemy) && dist >= weapon.MinRange && dist <= weapon.MaxRange);
        }

        public bool CanAttack(Point p)
        {
            if (weapon == null)
                return false;

            int dist = layer.Pathfind.AttackDistance(position, p);

            return (layer.IsPassable(p) && dist >= weapon.MinRange && dist <= weapon.MaxRange);
        }

        #endregion

        #region Combat

        public void Attack(Entity e)
        {
            if (weapon != null && e != null && weapon.CanTarget(e))
            {
                //Animation
                attacking = true;
                targetPos = e.Position;
                elapsed = toAttack;

                //Mechanics
                if (layer.IsCoverTile(e.Position) && !weapon.Healing)
                {
                    weapon.Use(e, skill / 2);
                }
                else
                {
                    weapon.Use(e, skill);
                }
            }
        }

        Point targetPos;

        #endregion

        #region AI

        public Entity ClosestTarget(List<Entity> entities)
        {
            int lowestDistance = int.MaxValue;
            Entity closest = null;

            foreach (Entity e in entities)
            {
                if (e.Group == TargetGroup && DistanceTo(e) <= lowestDistance)
                {
                    closest = e;
                    lowestDistance = DistanceTo(e);
                }
            }

            return closest;
        }

        public int DistanceTo(Entity e)
        {
                return layer.Pathfind.MoveDistance(Position, e.Position);
        }

        #endregion

        #region Phase Shift

        public void EndPhase()
        {
            phase = (Phase)(MathHelper.Clamp((int)phase + 1, 0, 2));

            if (EquippedWeapon == null && phase == Phase.Attack)
                phase = Phase.Finished;
        }

        public void EndTurn()
        {
            phase = Phase.Move;
        }

        #endregion

        #region Update

        GamePadState lastPad;
        KeyboardState lastKey;
        public void Update(GameTime gameTime, PlayerIndex index)
        {
            KeyboardState keyState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(index);

            if (moving)
                updateMove(gameTime);

            if (attacking)
                updateAttack(gameTime);

            #region Inv Management

            if (InvMode)
            {
                if (padState.IsButtonDown(Buttons.B) && lastPad.IsButtonUp(Buttons.B))
                {
                    InvMode = false;
                    selectedItem = 0;
                }

                if (padState.IsButtonDown(Buttons.A) && lastPad.IsButtonUp(Buttons.A))
                {
                    Item i = Items.Items[selectedItem];

                    if (i is Weapon)
                        weapon = i as Weapon;

                    else if (i is Consumable)
                        i.UseOn(this);
                }

                if (padState.ThumbSticks.Left.Y > 0 && lastPad.ThumbSticks.Left.Y <= 0)
                    selectedItem++;
                else if (padState.ThumbSticks.Left.Y < 0 && lastPad.ThumbSticks.Left.Y >= 0)
                    selectedItem--;
                
                if (keyState.IsKeyDown(Keys.Escape) && lastKey.IsKeyUp(Keys.Escape))
                {
                    InvMode = false;
                    selectedItem = 0;
                }

                else if (keyState.IsKeyDown(Keys.Space) && lastKey.IsKeyUp(Keys.Space))
                {
                    Item i = Items.Items[selectedItem];
                    
                    if (i is Weapon)
                        weapon = i as Weapon;                        

                    else if (i is Consumable)
                        i.UseOn(this);
                }

                if (keyState.IsKeyDown(Keys.Down) && lastKey.IsKeyUp(Keys.Down))
                    selectedItem++;
                else if (keyState.IsKeyDown(Keys.Up) && lastKey.IsKeyUp(Keys.Up))
                    selectedItem--;

                

                if (selectedItem < 0)
                    selectedItem = Items.Items.Count() - 1;
                else if (selectedItem > Items.Items.Count() - 1)
                    selectedItem = 0;
            }

            #endregion

            animation.Update(gameTime);

            lastKey = keyState;
            lastPad = padState;
        }

        #endregion

        #region Act

        public void Act()
        {
            if (Behavior != null)
                Behavior(this);
        }

        #endregion

        #region Move Helper

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

        #region Points

        public List<Point> MovePoints()
        {
            List<Point> points = new List<Point>();

            for (int x = Math.Max(0, Position.X - speed); x <= Math.Min(layer.TileWidth - 1, Position.X + speed); ++x)
            {
                for (int y = Math.Max(0, Position.Y - speed); y <= Math.Min(layer.TileHeight - 1, Position.Y + speed); ++y)
                {
                    if (CanMoveTo(new Point(x, y)))
                        points.Add(new Point(x, y));
                }
            }

            return points;
        }

        public List<Point> AttackPoints()
        {
            List<Point> points = new List<Point>();

            if (weapon == null)
                return points;

            for (int x = Math.Max(0, Position.X - weapon.MaxRange); x <= Math.Min(layer.TileWidth - 1, Position.X + weapon.MaxRange); ++x)
            {
                for (int y = Math.Max(0, Position.Y - weapon.MaxRange); y <= Math.Min(layer.TileHeight - 1, Position.Y + weapon.MaxRange); ++y)
                {
                    if (CanAttack(new Point(x, y)))
                        points.Add(new Point(x, y));
                }
            }

            return points;
        }

        #endregion

        #region Attack Helper

        private void updateAttack(GameTime gameTime)
        {
            elapsed -= gameTime.ElapsedGameTime;

            if (elapsed <= TimeSpan.Zero)
            {
                elapsed = TimeSpan.Zero;
                attacking = false;
            }

            float frac = 0;
            if (elapsed.Ticks >= toAttack.Ticks / 2)
                frac = (float)(toAttack.Ticks - elapsed.Ticks) / toAttack.Ticks;
            else
                frac = (float)elapsed.Ticks / toAttack.Ticks;

            moveOffset = new Vector2(layer.TileWidth * (targetPos.X - Position.X), layer.TileHeight * (targetPos.Y - Position.Y)) * frac;
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 loc = new Vector2(position.X * layer.TileWidth, position.Y * layer.TileHeight) + Utility.ToVector2(ScreenHelper.Viewport.TitleSafeArea.Location);

            if (moving || attacking)
                loc += moveOffset;

            spriteBatch.Draw(texture, loc, animation.CurrentRect, Color.White);

            if (layer.Entities.CurrentGroup == Group)
            {
                Vector2 iconLoc = loc + new Vector2(layer.TileWidth - 12, layer.TileHeight - 12);

                spriteBatch.Draw(iconTexture, iconLoc, iconSource(phase), Color.White);
            }

            Vector2 healthbarLoc = loc + new Vector2(2, 2);
            Rectangle backSource = new Rectangle(0, 0, 26, 4);
            Rectangle frontSource = new Rectangle(26, 0, 26, 4);

            frontSource.Width = (int)(frontSource.Width * Health.Fraction);

            spriteBatch.Draw(healthbarTexture, healthbarLoc, backSource, Color.White);
            spriteBatch.Draw(healthbarTexture, healthbarLoc, frontSource, Color.White);

            if (InvMode)
            {
                Vector2 itemLoc = new Vector2((position.X * layer.TileWidth) + (layer.TileWidth / 2), (position.Y * layer.TileHeight) + (layer.TileHeight / 2));
                foreach (Item i in Items.Items)
                {
                    Color color = (selectedItem == Items.Items.IndexOf(i)) ? Color.Yellow : Color.White;
                    string text = i.ToString();
                    if (i == weapon)
                        text += ": Equipped";


                    spriteBatch.DrawString(ScreenHelper.Font, text, itemLoc, color);
                    itemLoc.Y += ScreenHelper.Font.LineSpacing;
                }
            }
        }

        private Rectangle iconSource(Phase phase)
        {
            switch (phase)
            {
                case Phase.Move:
                    return new Rectangle(0, 0, 8, 8);

                case Phase.Attack:
                    if (EquippedWeapon.Healing)
                        return new Rectangle(24, 0, 8, 8);
                    else
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
