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
        private Texture2D bulletTexture;

        public WeaponSystem(World world, EntityManager entityManager)
        {
            this.world = world;
            this.entityManager = entityManager;
            bulletBehaviors = new();
        }

        public void LoadBulletTexture(Texture2D bullet)
        {
            this.bulletTexture = bullet;
        }

        // This could probably be set up to accept some sort of prefab weapon
        public void AddWeapon(Entity entity, WeaponDetails details)
        {
            // This should be changed to allow for multiple weapons using WeaponsList later,
            // but just using a single weapon for now, so it will always be the active one

            entityManager.AddComponent<WeaponDetails>(entity, details);
        }

        public void FireWeapon(Entity entity, GameTime currentTime)
        {
            var details = entityManager.GetComponent<WeaponDetails>(entity);

            // If the weapon is on cooldown, we exit the function
            if (details.LastFired > 0 && details.LastFired + details.Cooldown > (float)currentTime.TotalGameTime.TotalSeconds)
            {
                return;
            }

            Vector2 origin = entityManager.GetComponent<TransformComponent>(entity).Position;

            Vector2[] bulletPath =
            {
                 origin + new Vector2(0, -1500) // Move up 1500 pixels and stop
            };

            // Now create the bullet entity
            Entity bullet = entityManager.CreateEntity();

            entityManager.AddComponent<TransformComponent>(bullet, new TransformComponent
            {
                Position = origin
            });

            entityManager.AddComponent<GraphicComponent>(bullet, new GraphicComponent
            {
                Texture = bulletTexture
            });

            entityManager.AddComponent<Collider>(bullet, new Collider
            {
                Bounds = new Rectangle(origin.ToPoint(), bulletTexture.Bounds.Size),
                Layer = CollisionLayer.Projectile,
                CollidesWith = CollisionLayer.Enemy | CollisionLayer.Environment
            });

            BehaviorProperties behaviorProperties = new BehaviorProperties
            {
                Entity = bullet,
                Type = BehaviorType.Limited,
                Delay = 0,
                Priority = 1,
                Limit = 1
            };

            PathBehavior bulletBehavior = new PathBehavior(behaviorProperties, bulletPath, details.ProjectileSpeed);
            
            world.BehaviorSystem.AddBehavior(bullet, bulletBehavior);

            details.LastFired = (float)currentTime.TotalGameTime.TotalSeconds;
        }
    }
}
