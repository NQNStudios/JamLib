using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShmupLib
{
    public class Particle : Entity
    {
        float fadeTime;
        float elapsedTime;

        public Particle(Sprite sprite, float fadeTime)
            : base("", "Effects", 1, sprite, false, 0)
        {
            this.fadeTime = fadeTime;
        }

        public override void Update(GameTime gameTime, EntityManager manager)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Sprite.SetTint(Color.White * (1 - elapsedTime / fadeTime), 5f);

            if (elapsedTime >= fadeTime)
            {
                Damage(1);
            }

            base.Update(gameTime, manager);
        }

        public override void Draw(SpriteBatch spriteBatch, EntityManager manager)
        {
            
            base.Draw(spriteBatch, manager);
        }
    }
}
