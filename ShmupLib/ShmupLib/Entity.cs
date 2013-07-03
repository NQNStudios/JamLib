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

        string damageSound;

        protected StatBar health;
        Sprite sprite;

        bool collides;
        uint collisionDamage;
        List<string> collisionGroups = new List<string>();

        public Action1 OnCollision;
        public Action OnDamage;
        public Action OnRemove;
        public Action1 OnDeath;

        protected float invTime = 0.1f;
        float elapsedInv = 0;

        #endregion

        #region Properties

        public StatBar Health
        {
            get { return health; }
            set { health = value; }
        }

        public Sprite Sprite
        {
            get { return sprite; }
        }

        #endregion

        #region Initialization

        public Entity(string tag, string group, int _health, string damageSound, 
            Sprite _sprite, bool _collides, uint _collisionDamage, params string[] _collisionGroups)
        {
            Tag = tag;
            Group = group;
            health = new StatBar(_health);
            this.damageSound = damageSound;
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
            SoundManager.Play(damageSound);
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
                die(manager);
                return;
            }

            if (elapsedInv > 0)
            {
                elapsedInv -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                return;
            }

            if (collides)
            {
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
        }

        protected void die(EntityManager manager)
        {
            health.CurrentValue = 0;
            if (OnDeath != null)
            {
                OnDeath(this);
            }

            manager.Remove(this);
            return;
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

            Rectangle backRect = new Rectangle(x, y, (int)(backTexture.Width * b.Fraction), backTexture.Height);
            Rectangle frontRect = new Rectangle(x, y, frontTexture.Width, frontTexture.Height);

            spriteBatch.Draw(manager.BackBarTexture, backRect, null, color, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(manager.FrontBarTexture, frontRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
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
