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
        public FollowCamera Camera { get; private set; }

        private int playerEntityId;

        private readonly float moveSpeed = 300; // pixels per second
        private bool enemySpawned;
        private PatrolBehavior enemyBehavior;
        private int enemyId;
        private Queue<Command> commands;

        public World()
        {
            EntityManager = new EntityManager();
            MoveSystem = new MoveSystem(EntityManager);
            EnemySystem = new EnemySystem(EntityManager);
            CollisionSystem = new CollisionSystem(EntityManager);
            commands = new();
        }

        public void CreatePlayer(ContentManager content)
        {
            // Creating the player entity
            playerEntityId = EntityManager.CreateEntity();
            EntityManager.AddComponent(playerEntityId, ComponentFlag.InputComponent, new InputComponent());
            EntityManager.AddComponent(playerEntityId, ComponentFlag.GraphicComponent, new GraphicComponent
            {
                offset = new Vector2(-64, -64),
                texture = content.Load<Texture2D>("ship")
            });
            EntityManager.AddComponent(playerEntityId, ComponentFlag.TransformComponent, new TransformComponent
            {
                position = new Vector2(400, 400)
            });
            EntityManager.AddComponent(playerEntityId, ComponentFlag.BoxCollider, new BoxCollider
            {
                collider = new Rectangle(400, 400, 128, 128)
            });
        }

        public void QueueInputCommand(Vector2 moveDirection, GameTime gameTime)
        {
            // Commands are tightly coupled with world right now, so this probably needs to be revisited
            commands.Enqueue(new MoveCommand(this, moveDirection * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds));
        }

        public void SpawnEnemy(ContentManager content) 
        {
            enemyId = EnemySystem.SpawnEnemy(new Vector2(400, 100), content.Load<Texture2D>("enemy"));
            enemySpawned = true;

            // Attach a PatrolBehavior to the enemy, but it should likely be done elsewhere (in the enemySystem)
            BehaviorProperties props = new BehaviorProperties { 
                EntityId = enemyId,
                Type = BehaviorType.Repeated,
                Delay = 0,
                Priority = 1 
            };

            EntityManager.AddComponent(enemyId, ComponentFlag.BoxCollider, new BoxCollider
            {
                collider = new Rectangle(400, 100, 128, 128)
            });

            // Simple move to the left and right
            Vector2[] path = new Vector2[2];
            path[0] = new Vector2(300, 100);
            path[1] = new Vector2(500, 100);

            enemyBehavior = new PatrolBehavior(props, path, moveSpeed / 3);
        }

        public void Update(GameTime gameTime)
        {
            // Maybe move this outside the update call?
            List<Entity> inputEntites = EntityManager.getInputEntities();

            // The input commands get propagated to all input entities
            while (commands.Count > 0)
            {
                Command inputCommand = commands.Dequeue();
                foreach (Entity entity in inputEntites)
                {
                    inputCommand.Execute(entity.Id);
                }
            }

            // This command bypasses the command list because there's no way to
            // distinguish the input commands from enemy movement commands yet
            Command enemyMovement = enemyBehavior.Execute(this, gameTime);
            enemyMovement.Execute(enemyId);

            // Collision checking after movement
            List<(Entity, Entity)> collisions = CollisionSystem.CheckCollisions();
            if (collisions.Count > 0)
            {
                foreach(var collision in collisions)
                {
                    Console.WriteLine("Entity {0} collided with entity {1}", collision.Item1.Id, collision.Item2.Id);
                }
            }

            Camera.Update();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Maybe move this outside the update call?
            List<Entity> drawEntites = EntityManager.getDrawEntities();

            foreach (Entity entity in drawEntites)
            {
                var transform = EntityManager.GetTransformComponent(entity.Id);
                var graphic = EntityManager.GetGraphicComponent(entity.Id);

                spriteBatch.Draw(graphic.texture, transform.position, Color.White);
            }
        }

        public void AddCamera(Viewport viewport)
        {
            Camera = new FollowCamera(viewport, playerEntityId, EntityManager);
        }
    }
}
