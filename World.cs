using Redshift.Behaviors;
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

        public MoveSystem MoveSystem { get; private set; }
        public EnemySystem EnemySystem { get; private set; }
        public CollisionSystem CollisionSystem { get; private set; }
        public WeaponSystem WeaponSystem { get; private set; }
        public FollowCamera Camera { get; private set; }
        public GameTime GameTime { get; private set; }

        private Entity playerEntity;

        private readonly float moveSpeed = 300; // pixels per second
        private bool enemySpawned;
        private PathBehavior enemyBehavior;
        private Entity enemy;
        private Queue<Command> commands;

        public World()
        {
            EntityManager = new EntityManager();
            MoveSystem = new MoveSystem(EntityManager);
            EnemySystem = new EnemySystem(EntityManager);
            CollisionSystem = new CollisionSystem(EntityManager);
            WeaponSystem = new WeaponSystem(this, EntityManager);
            GameTime = new GameTime();
            commands = new();
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
            EntityManager.AddComponent<BoxCollider>(playerEntity, new BoxCollider
            {
                Bounds = new Rectangle(400, 400, 128, 128)
            });

            WeaponSystem.AddWeapon(playerEntity, new WeaponDetails
            {
                Cooldown = 0.5f,
                LastFired = -1,
                Damage = 20,
                ProjectileSpeed = 1500
            });
        }

        public void QueueInputCommand(Vector2 moveDirection, string inputType, GameTime gameTime)
        {
            // Commands are tightly coupled with world right now, so this probably needs to be revisited
            if (inputType == "Move")
            {
                commands.Enqueue(new MoveCommand(this, moveDirection * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds));
            }
            else if (inputType == "Shoot")
            {
                commands.Enqueue(new ShootCommand(this, gameTime));
            }
        }

        public void SpawnEnemy(ContentManager content) 
        {
            enemy = EnemySystem.SpawnEnemy(new Vector2(400, 100), content.Load<Texture2D>("enemy"));
            enemySpawned = true;

            // Attach a PatrolBehavior to the enemy, but it should likely be done elsewhere (in the enemySystem?)
            BehaviorProperties props = new BehaviorProperties { 
                Entity = enemy,
                Type = BehaviorType.Repeated,
                Delay = 0,
                Priority = 1 
            };

            EntityManager.AddComponent<BoxCollider>(enemy, new BoxCollider
            {
                Bounds = new Rectangle(400, 100, 128, 128)
            });

            // Simple move to the left and right
            Vector2[] path = new Vector2[2];
            path[0] = new Vector2(300, 100);
            path[1] = new Vector2(500, 100);

            enemyBehavior = new PathBehavior(props, path, moveSpeed / 3);
        }

        public void Update(GameTime gameTime)
        {
            GameTime = gameTime;

            // Maybe move this outside the update call?
            List<Entity> inputEntites = EntityManager.GetEntitiesByFlag(ComponentFlag.InputComponent);

            // The input commands get propagated to all input entities
            while (commands.Count > 0)
            {
                Command inputCommand = commands.Dequeue();
                foreach (Entity entity in inputEntites)
                {
                    inputCommand.Execute(entity);
                }
            }

            // This command bypasses the command list because there's no way to
            // distinguish the input commands from enemy movement commands yet
            Command enemyMovement = enemyBehavior.Execute(this, gameTime);
            enemyMovement.Execute(enemy);

            var projectileBehaviors = WeaponSystem.UpdateProjectiles(gameTime);

            foreach (var behavior in projectileBehaviors)
            {
                behavior.Item1.Execute(behavior.Item2);
            }

            // Collision checking after movement
            List<(Entity, Entity)> collisions = CollisionSystem.CheckCollisions();
            if (collisions.Count > 0)
            {
                foreach(var collision in collisions)
                {
                    if (collision.Item1.Id == 1)
                        EntityManager.DeleteEntity(collision.Item2);

                    Console.WriteLine("Entity {0} collided with entity {1}", collision.Item1.Id, collision.Item2.Id);
                }
            }

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
