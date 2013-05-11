using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileGameLib
{
    public class EntityManager
    {
        #region Fields

        List<Entity> entities = new List<Entity>();
        List<string> groups = new List<string>();
        int currentGroup;
        TileLayer layer;

        public Texture2D BloodTexture;
        public Texture2D HealingTexture;

        Dictionary<Point, FrameAnimation> animations = new Dictionary<Point, FrameAnimation>();
        double CastleHealAmount = 5.0;

        #endregion

        #region Properties

        public List<Entity> EntityList
        {
            get { return entities; }
        }

        public string CurrentGroup
        {
            get { return groups[currentGroup]; }
        }

        #endregion

        public EntityManager(TileLayer layer)
        {
            this.layer = layer;
            groups.Add("Player");
        }

        #region Methods

        public Entity EntityAt(Point loc)
        {
            foreach (Entity e in entities)
            {
                if (e.Position == loc)
                    return e;
            }

            return null;
        }

        public List<Entity> Group(string group)
        {
            List<Entity> entities = new List<Entity>();

            foreach (Entity e in this.entities)
            {
                if (e.Group == group)
                    entities.Add(e);
            }

            return entities;
        }

        public bool GroupFinished(string group)
        {
            foreach (Entity e in Group(group))
            {
                if (e.Phase != Phase.Finished || e.Moving || e.Attacking)
                    return false;
            }

            return true;
        }

        public bool AnimationsFinished()
        {
            foreach (FrameAnimation animation in animations.Values)
            {
                return false;
            }

            return true;
        }

        public Entity Tag(string tag)
        {
            foreach (Entity e in entities)
            {
                if (e.Tag == tag)
                    return e;
            }

            return null;
        }

        public void Add(Entity e)
        {
            entities.Add(e);
            e.EntityManager = this;

            if (!string.IsNullOrEmpty(e.Group))
            {
                if (!groups.Contains(e.Group))
                    groups.Add(e.Group);
            }
        }

        #endregion

        #region Update

        int currentEntity = 0;
        public void Update(GameTime gameTime)
        {
            List<Point> frameRemove = new List<Point>();
            foreach (Point p in animations.Keys)
            {
                animations[p].Update(gameTime);
                if (animations[p].Finished)
                    frameRemove.Add(p);
            }

            foreach (Point p in frameRemove)
            {
                animations.Remove(p);
            }

            List<Entity> toRemove = new List<Entity>();
            foreach (Entity e in entities)
            {
                e.Update(gameTime, PlayerIndex.One);

                if (e.Health.IsEmpty)
                {
                    animations.Remove(e.Position);
                    FrameAnimation animation = new FrameAnimation(5, 32, 32, 0, 0, true);
                    animation.FramesPerSecond = 10;
                    animations.Add(e.Position, animation);
                    toRemove.Add(e);
                }
            }

            foreach (Entity e in toRemove)
            {
                string group = e.Group;
                entities.Remove(e);
                if (Group(group).Count() == 0)
                {
                    if (group != "Player")
                        groups.Remove(group);
                }
            }

            if (GroupFinished(CurrentGroup) && AnimationsFinished())
            {
                currentEntity = 0;
                foreach (Entity e in Group(CurrentGroup))
                {
                    e.EndTurn();
                    if (layer.IsCastleTile(e.Position))
                    {
                        e.Health.Heal(CastleHealAmount);
                        FrameAnimation animation = new FrameAnimation(5, 32, 32, 0, 0, true);
                        animation.FramesPerSecond = 12;
                        animations.Add(e.Position, animation);
                    }
                }
                currentGroup = ++currentGroup % groups.Count;
            }
            else
            {
                if (CurrentGroup == "Player")
                    return;

                List<Entity> cur = Group(CurrentGroup);
                if (currentEntity < cur.Count())
                {
                    cur[currentEntity].Act();
                    if (cur[currentEntity].Phase == Phase.Finished)
                        currentEntity++;
                }
            }
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Entity e in entities)
            {
                if (!e.InvMode)
                    e.Draw(spriteBatch);
            }

            foreach (Entity e in entities)
            {
                if (e.InvMode)
                    e.Draw(spriteBatch);
            }

            foreach (Point p in animations.Keys)
            {
                Vector2 loc = Utility.ToVector2(p) * new Vector2(layer.TileWidth, layer.TileHeight);
                if (animations[p].FramesPerSecond == 10)
                    spriteBatch.Draw(BloodTexture, loc, animations[p].CurrentRect, Color.White);
                else
                    spriteBatch.Draw(HealingTexture, loc, animations[p].CurrentRect, Color.White);
            }

            string text = CurrentGroup + " Turn";
            Color color = colorFor(CurrentGroup);
            spriteBatch.DrawString(ScreenHelper.Font, text,
                ScreenHelper.Camera.Position + new Vector2(ScreenHelper.Viewport.Width / 2 - ScreenHelper.Font.MeasureString(text).X / 2, 0), color);
        }

        private Color colorFor(string group)
        {
            switch (group)
            {
                case "Player":
                    return Color.Blue;
                    
                case "Enemy":
                    return Color.Red;

                default:
                    return Color.Green;
            }
        }

        #endregion
    }
}
