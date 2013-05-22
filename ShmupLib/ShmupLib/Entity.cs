using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShmupLib
{
    public delegate void Action();
    public delegate void Action1(Entity e);
    public delegate void Action2(Entity e1, Entity e2);

    public class Entity
    {
        #region Fields

        public string Tag = "";
        public string Group = "";
        
        protected StatBar health;
        Sprite sprite;

        bool collides;
        uint collisionDamage;
        List<string> collisionGroups = new List<string>();

        public Action1 OnCollision;
        public Action OnDamage;
        public Action OnDeath;

        float invTime = 0.4f;
        float elapsedInv = 0;

        #endregion

        #region Properties

        public Sprite Sprite
        {
            get { return sprite; }
        }

        #endregion

        #region Initialization

        public Entity(string tag, string group, int _health, Sprite _sprite, bool _collides, uint _collisionDamage, params string[] _collisionGroups)
        {
            Tag = tag;
            Group = group;
            health = new StatBar(_health);
            sprite = _sprite;
            collides = _collides;
            collisionDamage = _collisionDamage;

            foreach (string s in _collisionGroups)
            {
                collisionGroups.Add(s);
            }
        }

        #endregion

        #region Health

        public void Damage(uint amount)
        {
            health.Damage(amount);
        }

        public void Heal(uint amount)
        {
            health.Heal(amount);
        }

        #endregion

        #region Update

        public virtual void Update(GameTime gameTime, EntityManager manager)
        {
            sprite.Update(gameTime);

            if (health.IsEmpty)
            {
                if (OnDeath != null)
                {
                    OnDeath();
                }

                manager.Remove(this);
                return;
            }

            if (elapsedInv > 0)
            {
                elapsedInv -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                return;
            }

            foreach (string group in manager.Entities.Keys)
            {
                if (collisionGroups.Contains(group))
                {
                    foreach (Entity e in manager.Entities[group])
                    {
                        if (e.collides && sprite.IsPixelColliding(e.sprite))
                        {
                            if (OnCollision != null)
                            {
                                OnCollision(e);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Draw

        public virtual void Draw(SpriteBatch spriteBatch, EntityManager manager)
        {
            sprite.Draw(spriteBatch);

            if (health.MaxValue > 1)
                DrawBar(spriteBatch, health, Color.Red, manager);
        }

        protected virtual void DrawBar(SpriteBatch spriteBatch, StatBar b, Color color, EntityManager manager)
        {
            Texture2D backTexture = manager.BackBarTexture;
            Texture2D frontTexture = manager.FrontBarTexture;

            int x = (int)sprite.Center.X - backTexture.Width / 2;
            int y = sprite.BoundingBoxRect.Y - backTexture.Height;

            Rectangle backRect = new Rectangle(x, y, (int)(backTexture.Width * health.Fraction), backTexture.Height);
            Rectangle frontRect = new Rectangle(x, y, frontTexture.Width, frontTexture.Height);

            spriteBatch.Draw(manager.BackBarTexture, backRect, color);
            spriteBatch.Draw(manager.FrontBarTexture, frontRect, Color.White);
        }

        #endregion

        #region Generic Events

        public void damageEffect()
        {
            sprite.SetTint(Color.Red, 0.1f);
            elapsedInv = invTime;
        }

        public void collideWith(Entity e)
        {
            Damage(e.collisionDamage);

            if (OnDamage != null)
                OnDamage();
        }

        #endregion
    }
}
