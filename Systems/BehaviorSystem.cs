using Microsoft.Xna.Framework;
using Redshift.Behaviors;
using System.Collections.Generic;

namespace Redshift.Systems
{
    internal class BehaviorSystem
    {
        private World world;
        private EntityManager entityManager;

        public BehaviorSystem(World world, EntityManager entityManager) 
        {
            this.world = world;
            this.entityManager = entityManager;
        }

        public void AddBehavior(Entity entity, Behavior behavior)
        {
            if (!entity.ActiveComponents.HasFlag(ComponentFlag.AIBehaviorComponent))
            {
                var component = new AIBehaviorComponent();
                entityManager.AddComponent(entity, component);

                component.Behaviors.Add(behavior);
            }
            else
            {
                var component = entityManager.GetComponent<AIBehaviorComponent>(entity);
                component.Behaviors.Add(behavior);
            }
        }

        public void RunBehaviors(GameTime gameTime)
        {
            List<Entity> entities = entityManager.GetEntitiesByFlag(ComponentFlag.AIBehaviorComponent);
            List<(Entity entity, Command command)> commands = new();

            foreach (var entity in entities)
            {
                var behaviorComponet = entityManager.GetComponent<AIBehaviorComponent>(entity);

                // This can have more complex selection logic based on priority, etc
                foreach (var behavior in behaviorComponet.Behaviors)
                {
                    var command = behavior.Execute(world, gameTime);

                    if (command == null)
                        continue;
                    
                    if (behavior.Type == BehaviorType.Repeated)
                    {
                        commands.Add((behavior.Entity, command));
                    }
                    else if (behavior.Type == BehaviorType.Limited && behavior.Completions < behavior.Limit)
                    {
                        commands.Add((behavior.Entity, command));
                    }
                }    
            }

            foreach (var command in commands)
            {
                command.command.Execute(command.entity);
            }
        }
    }
}
