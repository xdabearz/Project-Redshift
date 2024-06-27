using Microsoft.Xna.Framework;

namespace Redshift.Systems
{
    public class MoveSystem
    {
        private EntityManager entityManager;

        public MoveSystem(EntityManager entityManager) 
        {
            this.entityManager = entityManager;
        }

        public void ApplyMovement(int entityId, Vector2 movement)
        {
            ref TransformComponent transform = ref entityManager.GetTransformComponent(entityId);
            
            // An entity may not have a collider, so need to have this handled better
            ref BoxCollider collider = ref entityManager.GetBoxCollider(entityId);

            transform.position += movement;

            // Moves the collider position to match the transform movement
            collider.collider.Offset(movement);
        }
    }
}
