using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Redshift.Systems
{
    internal class MovementSystem : ECSSystem
    {
        public MovementSystem(World world) : base(world) { }

        public override void Update(GameTime gameTime)
        {
            List<Entity> entities = World.EntityManager.GetEntitiesByFlag(ComponentFlag.TransformComponent | ComponentFlag.MovementComponent);

            foreach (var entity in entities)
            {
                var transform = World.EntityManager.GetComponent<TransformComponent>(entity);
                var movement = World.EntityManager.GetComponent<MovementComponent>(entity);

                transform.Position += movement.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
