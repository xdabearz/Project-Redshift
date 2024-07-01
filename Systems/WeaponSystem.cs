using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Redshift.Behaviors;
using Microsoft.Xna.Framework;
using Redshift.Commands;
using System;

namespace Redshift.Systems
{
    // This should probably be moved somewhere more logical
    public enum WeaponType
    {
        BasicWeapon
    }

    internal class WeaponSystem
    {
        private World world;
        private EntityManager entityManager;

        // This should be saved elsewhere; systems shouldn't hold state
        private List<Behavior> bulletBehaviors;

        // This should be moved out to a texture handling system later
        private Texture2D bullet;

        public WeaponSystem(World world, EntityManager entityManager)
        {
            this.world = world;
            this.entityManager = entityManager;
            bulletBehaviors = new();
        }

        public void LoadBulletTexture(Texture2D bullet)
        {
            this.bullet = bullet;
        }

        // This could probably be set up to accept some sort of prefab weapon
        public void AddWeapon(Entity entity, WeaponDetails details)
        {
            // This should be changed to allow for multiple weapons using WeaponsList later,
            // but just using a single weapon for now, so it will always be the active one

            entityManager.AddComponent(entity.Id, ComponentFlag.WeaponDetails, details);
        }

        public void FireWeapon(Entity entity, GameTime currentTime)
        {
            ref WeaponDetails details = ref entityManager.GetComponent<WeaponDetails>(entity.Id);

            // If the weapon is on cooldown, we exit the function
            if (details.LastFired > 0 && details.LastFired + details.Cooldown > (float)currentTime.TotalGameTime.TotalSeconds)
            {
                return;
            }

            Vector2 origin = entityManager.GetComponent<TransformComponent>(entity.Id).position;

            Vector2[] bulletPath =
            {
                 origin,
                 origin + new Vector2(0, -1000) // Move up 1000 pixels and stop
            };

            // Now create the bullet entity
            int bulletId = entityManager.CreateEntity();

            entityManager.AddComponent(bulletId, ComponentFlag.TransformComponent, new TransformComponent
            {
                position = origin
            });

            entityManager.AddComponent(bulletId, ComponentFlag.GraphicComponent, new GraphicComponent
            {
                texture = bullet
            });

            BehaviorProperties behaviorProperties = new BehaviorProperties
            {
                EntityId = bulletId,
                Type = BehaviorType.Limited,
                Delay = 0,
                Priority = 1
            };

            PathBehavior bulletBehavior = new PathBehavior(behaviorProperties, bulletPath, details.ProjectileSpeed);
            bulletBehaviors.Add(bulletBehavior);

            details.LastFired = (float)currentTime.TotalGameTime.TotalSeconds;
        }

        public List<(Command, Entity)> UpdateProjectiles(GameTime gameTime)
        {
            List<(Command, Entity)> commands = new();
            List<PathBehavior> deleteBehaviors = new();

            foreach (PathBehavior bulletBehavior in bulletBehaviors)
            {
                var command = bulletBehavior.Execute(world, gameTime);
                if (command != null)
                {
                    commands.Add((command, world.EntityManager.GetEntityById(bulletBehavior.EntityId)));
                }
                else
                {
                    // Bullet reached the end of its path, flag for deletion after this loop
                    deleteBehaviors.Add(bulletBehavior);
                }
            }

            // Now delete them all
            foreach (PathBehavior bulletBehavior in deleteBehaviors)
            {
                bulletBehaviors.Remove(bulletBehavior);
            }

            return commands;
        }
    }
}
