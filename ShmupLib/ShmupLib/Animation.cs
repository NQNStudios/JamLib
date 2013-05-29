using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShmupLib
{
    public class Animation : Entity
    {
        public Animation(Sprite sprite)
            : base("", "Effects", 1, "", sprite, false, 0)
        {
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, EntityManager manager)
        {
            base.Update(gameTime, manager);

            if (Sprite.Done)
            {
                Damage(1);
            }
        }
    }
}
