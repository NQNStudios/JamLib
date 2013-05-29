using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShmupLib
{
    public class EntityManager
    {
        #region Fields

        Dictionary<string, List<Entity>> entities = new Dictionary<string, List<Entity>>();

        Texture2D backBarTexture;
        Texture2D frontBarTexture;

        #endregion

        #region Properties

        public Dictionary<string, List<Entity>> Entities
        {
            get { return entities; }
        }

        public Texture2D BackBarTexture
        {
            get { return backBarTexture; }
        }

        public Texture2D FrontBarTexture
        {
            get { return frontBarTexture; }
        }

        #endregion

        #region Initialization

        public EntityManager(Texture2D _backBar, Texture2D _frontBar)
        {
            backBarTexture = _backBar;
            frontBarTexture = _frontBar;
        }

        #endregion

        #region Collection

        public void Add(Entity e)
        {
            if (entities.ContainsKey(e.Group))
            {
                entities[e.Group].Add(e);
            }
            else
            {
                entities.Add(e.Group, new List<Entity>());
                Add(e);
            }
        }

        public void Remove(Entity e)
        {
            if (e.OnRemove != null)
                e.OnRemove();

            if (entities.ContainsKey(e.Group))
            {
                entities[e.Group].Remove(e);
            }
        }

        public Entity Get(string tag)
        {
            foreach (List<Entity> group in entities.Values)
            {
                foreach (Entity e in group)
                {
                    if (e.Tag == tag)
                        return e;
                }
            }

            return null;
        }

        public Entity Get(string tag, string group)
        {
            if (!entities.ContainsKey(group))
                return null;

            foreach (Entity e in entities[group])
            {
                if (e.Tag == tag)
                    return e;
            }

            return null;
        }

        public List<Entity> GetGroup(string group)
        {
            if (entities.ContainsKey(group))
                return entities[group];

            return null;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            for (int i = entities.Keys.Count() - 1; i >= 0; i--)//(List<Entity> group in entities.Values)
            {
                List<Entity> group = entities[entities.Keys.ElementAt(i)];
                for (int j = group.Count() - 1; j >= 0; j--)
                {
                    Vector2 loc = group[j].Sprite.Location;
                    if (loc.X < 0 - group[j].Sprite.BoundingBoxRect.Width || loc.X > ScreenHelper.Viewport.Width || loc.Y < 0 - group[j].Sprite.BoundingBoxRect.Height || loc.Y > ScreenHelper.Viewport.Height)
                    {
                        Remove(group[j]);
                        continue;
                    }

                    group[j].Update(gameTime, this);
                }
            }
        }

        #endregion

        #region Draw

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (List<Entity> group in entities.Values)
            {
                foreach (Entity e in group)
                {
                    e.Draw(spriteBatch, this);
                }
            }
        }

        #endregion
    } 
}