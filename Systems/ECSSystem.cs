using Microsoft.Xna.Framework;

namespace Redshift.Systems
{
    internal abstract class ECSSystem
    {
        protected World World;

        public ECSSystem(World world)
        {
            World = world;
        }

        public abstract void Update(GameTime gameTime);
    }
}
