using Redshift.Commands;
using Microsoft.Xna.Framework;
using System;

namespace Redshift.Behaviors
{
    internal class PathBehavior : Behavior
    {
        public override Entity Entity { get; }
        public override BehaviorType Type { get; }
        public override float Delay { get; }
        public override int Priority { get; }

        private readonly float movementSpeed;
        private readonly Vector2[] pathNodes;

        private int destinationNode;

        // movementSpeed should eventually be moved to a component accessible via the entityId/world
        public PathBehavior(BehaviorProperties properties, Vector2[] pathNodes, float movementSpeed)
        {
            Entity = properties.Entity;
            Type = properties.Type;
            Delay = properties.Delay;
            Priority = properties.Priority;

            this.movementSpeed = movementSpeed;

            // Sanity check that there are actually nodes in the path
            if (pathNodes == null || pathNodes.Length < 1)
                throw new ArgumentNullException(nameof(pathNodes));

            this.pathNodes = pathNodes;
            destinationNode = 0;
        }

        public override Command Execute(World world, GameTime gameTime)
        {
            var transform = world.EntityManager.GetComponent<TransformComponent>(Entity);

            if (transform == null)
                return null;

            // Move toward the destination node
            Vector2 pathToDestination = pathNodes[destinationNode] - transform.Position;
            float moveDistance = movementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (pathToDestination.Length() > moveDistance)
            {
                pathToDestination.Normalize();
                pathToDestination *= moveDistance;
            }

            var moveCommand = new MoveCommand(world, pathToDestination);

            // If destination reached (or really close to it), move to the next node or to the
            // initial node if the end has been reached
            Vector2 positionAfterMovement = transform.Position + pathToDestination;
            Vector2 pathRemaining = pathNodes[destinationNode] - positionAfterMovement;
            if (pathRemaining.Length() < 0.001f)
                destinationNode++;

            if (destinationNode >= pathNodes.Length && Type == BehaviorType.Repeated)
                destinationNode = 0;

            // If end of path reached and limited (for now, all limited = 1) then return null
            if (destinationNode >= pathNodes.Length && Type == BehaviorType.Limited)
                return null;

            // Delay and Priority are not yet implemented, but they can add more complex behavior in tandem
            // with other behaviors such as making an enemy stop and look around at the end of the path.

            return moveCommand;
        }
    }
}
