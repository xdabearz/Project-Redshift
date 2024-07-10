using Microsoft.Xna.Framework;

namespace Redshift.Systems
{
    internal class MoveSystem
    {
        private readonly EntityManager entityManager;

        public MoveSystem(EntityManager entityManager) 
        {
            this.entityManager = entityManager;
        }

        public void ApplyMovement(Entity entity, Vector2 movement)
        {
            var transform = entityManager.GetComponent<TransformComponent>(entity);
            
            var collider = entityManager.GetComponent<Collider>(entity);

            transform.Position += movement;

            // Moves the collider (if there is one) to match the transform movement
            collider?.Bounds.Offset(movement);
        }
    }
}
