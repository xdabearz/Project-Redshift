using Microsoft.Xna.Framework;

namespace Redshift.Commands
{
    internal class MoveCommand : Command
    {
        private readonly World world;
        private readonly Vector2 movement;

        public MoveCommand(World world, Vector2 movement) 
        {
            this.movement = movement;
            this.world = world;
        }

        public override void Execute(Entity entity)
        {
            // With the entity ID, execute the movement
            world.MoveSystem.ApplyMovement(entity, movement);
        }
    }
}
