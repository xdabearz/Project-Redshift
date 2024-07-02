using System.Collections.Generic;

namespace Redshift.Systems
{
    // This may be better merged into MoveSystem
    internal class CollisionSystem
    {
        private EntityManager entityManager;

        public CollisionSystem(EntityManager entityManager) 
        {
            this.entityManager = entityManager;
        }

        public List<(Entity, Entity)> CheckCollisions()
        {
            List<(Entity, Entity)> collisions = new();

            // This is a pretty slow check, runs in O(n^2) time. May want to speed up
            // later with something like a quadtree or uniform grid

            List<Entity> entities = entityManager.GetEntitiesByFlag(ComponentFlag.TransformComponent | ComponentFlag.BoxCollider);

            for (int i = 0; i < entities.Count; i++)
            {
                var colliderA = entityManager.GetComponent<BoxCollider>(entities[i]);
                for (int j = i + 1; j < entities.Count; j++)
                {
                    // Don't check for collisions with itself
                    if (entities[i].Id == entities[j].Id)
                        continue;

                    var colliderB = entityManager.GetComponent<BoxCollider>(entities[j]);

                    if (colliderA.Bounds.Intersects(colliderB.Bounds))
                        collisions.Add((entities[i], entities[j]));
                }
            }

            return collisions;
        }
    }
}
