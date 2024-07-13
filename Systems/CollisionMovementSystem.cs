using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Redshift.Systems
{
    internal class CollisionMovementSystem : ECSSystem
    {
        public CollisionMovementSystem(World world) : base(world) { }

        public override void Update(GameTime gameTime)
        {
            List<Entity> entities = World.EntityManager.GetEntitiesByFlag(ComponentFlag.TransformComponent | ComponentFlag.Collider | ComponentFlag.MovementComponent);

            foreach (Entity entity in entities)
            {
                var collider = World.EntityManager.GetComponent<Collider>(entity);
                var movement = World.EntityManager.GetComponent<MovementComponent>(entity);
                
                collider.Bounds.Offset(movement.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }
    }
}
