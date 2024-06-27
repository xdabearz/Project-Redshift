using Microsoft.Xna.Framework;

namespace Gala.Commands
{
    internal class MoveCommand : Command
    {
        private readonly World world;
        private readonly Vector2 movement;

        public MoveCommand(World world, Vector2 movement) 
        {
            this.world = world;
            this.movement = movement;
        }

        public override void Execute(int entityId)
        {
            // With the entity ID, execute the movement
            world.MoveSystem.ApplyMovement(ref world.EntityManager.GetTransformComponent(entityId), movement);
        }
    }
}
