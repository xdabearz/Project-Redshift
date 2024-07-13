﻿using Redshift.Behaviors;
using Redshift.Commands;
using Redshift.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Redshift
{
    internal class World
    {
        // Systems & Managers
        public EntityManager EntityManager { get; private set; }
        
        private readonly List<ECSSystem> LogicSystems;
        private List<ECSSystem> GraphicSystems;

        public EnemySystem EnemySystem { get; private set; }
        public WeaponSystem WeaponSystem { get; private set; }
        public BehaviorSystem BehaviorSystem { get; private set; }
        public FollowCamera Camera { get; private set; }
        public GameTime GameTime { get; private set; }

        private Entity playerEntity;

        private readonly float moveSpeed = 300; // pixels per second
        private bool enemySpawned;
        private PathBehavior enemyBehavior;
        private Entity enemy;

        public World()
        {
            EntityManager = new EntityManager();

            // These systems are updated in order inside the Update loop
            LogicSystems = new()
            {
                new InputCaptureSystem(this),
                new CollisionMovementSystem(this),
                new MovementSystem(this),
                new CollisionSystem(this),
            };

            GraphicSystems = new List<ECSSystem>();

            EnemySystem = new EnemySystem(EntityManager);
            WeaponSystem = new WeaponSystem(this, EntityManager);
            BehaviorSystem = new BehaviorSystem(this, EntityManager);

            GameTime = new GameTime();
        }

        public void CreatePlayer(ContentManager content)
        {
            // Creating the player entity
            playerEntity = EntityManager.CreateEntity();
            EntityManager.AddComponent<InputComponent>(playerEntity, new InputComponent());
            EntityManager.AddComponent<GraphicComponent>(playerEntity, new GraphicComponent
            {
                Offset = new Vector2(-64, -64),
                Texture = content.Load<Texture2D>("ship")
            });
            EntityManager.AddComponent<TransformComponent>(playerEntity, new TransformComponent
            {
                Position = new Vector2(400, 400)
            });
            EntityManager.AddComponent<Collider>(playerEntity, new Collider
            {
                Bounds = new Rectangle(400, 400, 128, 128),
                Layer = CollisionLayer.Player,
                CollidesWith = CollisionLayer.Enemy | CollisionLayer.Environment
            });

            WeaponSystem.AddWeapon(playerEntity, new WeaponDetails
            {
                Cooldown = 0.5f,
                LastFired = -1,
                Damage = 20,
                ProjectileSpeed = 1500
            });
        }

        public void SpawnEnemy(ContentManager content) 
        {
            enemy = EnemySystem.SpawnEnemy(new Vector2(400, 100), content.Load<Texture2D>("enemy"));
            enemySpawned = true;

            // Attach a PatrolBehavior to the enemy, but it should likely be done elsewhere (in the enemySystem?)
            BehaviorProperties props = new BehaviorProperties
            {
                Entity = enemy,
                Type = BehaviorType.Repeated,
                Delay = 0,
                Priority = 1
            };

            EntityManager.AddComponent<Collider>(enemy, new Collider
            {
                Bounds = new Rectangle(400, 100, 128, 128),
                Layer = CollisionLayer.Enemy,
                CollidesWith = CollisionLayer.Player | CollisionLayer.Projectile
            });

            // Simple move to the left and right
            Vector2[] path = new Vector2[2];
            path[0] = new Vector2(300, 100);
            path[1] = new Vector2(500, 100);

            BehaviorSystem.AddBehavior(enemy, new PathBehavior(props, path, moveSpeed / 3));
        }

        public void Update(GameTime gameTime)
        {
            GameTime = gameTime;

            foreach (ECSSystem system in LogicSystems)
            {
                system.Update(gameTime);
            }

            BehaviorSystem.RunBehaviors(gameTime);

            EntityManager.CleanupEntities();
            Camera.Update();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Maybe move this outside the update call?
            List<Entity> drawEntites = EntityManager.GetEntitiesByFlag(ComponentFlag.GraphicComponent | ComponentFlag.TransformComponent);

            foreach (Entity entity in drawEntites)
            {
                var transform = EntityManager.GetComponent<TransformComponent>(entity);
                var graphic = EntityManager.GetComponent<GraphicComponent>(entity);

                spriteBatch.Draw(graphic.Texture, transform.Position, Color.White);
            }
        }

        public void AddCamera(Viewport viewport)
        {
            Camera = new FollowCamera(viewport, playerEntity, EntityManager);
        }
    }
}
