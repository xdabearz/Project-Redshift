using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Redshift.Systems
{
    internal class CollisionSystem : ECSSystem
    {
        public CollisionSystem(World world) : base(world) { }

        public override void Update(GameTime gameTime)
        {
            // This is a pretty slow check, runs in O(n^2) time. May want to speed up
            // later with something like a quadtree or uniform grid

            List<Entity> entities = World.EntityManager.GetEntitiesByFlag(ComponentFlag.TransformComponent | ComponentFlag.Collider);

            for (int i = 0; i < entities.Count; i++)
            {
                var colliderA = World.EntityManager.GetComponent<Collider>(entities[i]);

                if (colliderA.Layer == CollisionLayer.None)
                    continue;

                for (int j = i + 1; j < entities.Count; j++)
                {
                    // Don't check for collisions with itself
                    if (entities[i].Id == entities[j].Id)
                        continue;

                    var colliderB = World.EntityManager.GetComponent<Collider>(entities[j]);

                    // If the 2 objects don't collide, do nothing
                    if (!colliderB.CollidesWith.HasFlag(colliderA.Layer))
                        continue;

                    if (colliderA.Bounds.Intersects(colliderB.Bounds))
                    {
                        // TODO: Add some sort of collision resolve component to entity A
                        Console.WriteLine("Entity {0} collided with entity {1}.", entities[i].Id, entities[j].Id);
                    }
                }
            }
        }
    }
}
