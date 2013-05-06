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
        List<Entity> entities = new List<Entity>();
        List<string> groups = new List<string>();
        int currentGroup;

        public string CurrentGroup
        {
            get { return groups[currentGroup]; }
        }

        public EntityManager()
        {
            groups.Add("Player");
        }

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
                if (e.Phase != Phase.Finished)
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

        public void Update(GameTime gameTime)
        {
            foreach (Entity e in entities)
            {
                e.Update(gameTime);
            }

            if (GroupFinished(CurrentGroup))
            {
                foreach (Entity e in Group(CurrentGroup))
                {
                    e.EndTurn();
                }
                currentGroup = ++currentGroup % groups.Count;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Entity e in entities)
            {
                e.Draw(spriteBatch);
            }
        }
    }
}
