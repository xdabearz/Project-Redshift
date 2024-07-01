using Microsoft.Xna.Framework;

namespace Redshift.Commands
{
    internal class ShootCommand : Command
    {
        private World world;
        private GameTime gameTime;

        public ShootCommand(World world, GameTime gameTime) 
        {
            this.world = world;
            this.gameTime = gameTime;
        }

        public override void Execute(int entityId)
        {
            world.WeaponSystem.FireWeapon(world.EntityManager.GetEntityById(entityId), gameTime);
        }
    }
}
