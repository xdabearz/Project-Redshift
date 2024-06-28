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
            // Ref types must be used here since we want to update the original, not a copy
            ref TransformComponent transform = ref entityManager.GetComponent<TransformComponent>(entityId);
            
            // An entity may not have a collider, so need to have this handled better
            ref BoxCollider collider = ref entityManager.GetComponent<BoxCollider>(entityId);

            transform.position += movement;

            // Moves the collider position to match the transform movement
            collider.collider.Offset(movement);
        }
    }
}
