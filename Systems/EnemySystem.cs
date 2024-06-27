using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Redshift.Systems
{
    internal class EnemySystem
    {
        private EntityManager entityManager;

        public EnemySystem(EntityManager entityManager) 
        { 
            this.entityManager = entityManager;
        }

        public int SpawnEnemy(Vector2 spawnLocation, Texture2D texture)
        {
            // Will enemies have different type IDs? That likely needs to be included
            // Add an entity with some commands from the entity manager
            int id = entityManager.CreateEntity();

            entityManager.AddComponent(id, ComponentFlag.TransformComponent, new TransformComponent
            {
                position = spawnLocation
            });
            
            entityManager.AddComponent(id, ComponentFlag.GraphicComponent, new GraphicComponent
            {
                offset = new Vector2(0, 0),
                texture = texture
            });

            return id;

            // How does input work for an enemy? Is it some generated action?
            // Should there be some sort of factory class to create action objects?
            // There likely needs to be some sort of AI Component or Clustered AI Component
            // Create a simple patrol AI action for moving back and forth
            // Actions should be created by input and AI
        }

        // How are enemies controlled? Through some update function?
        // How can the enemy's next movement be inferred from its current state?
        // Are there any persisting things that need to be carried over between game loops?
        // Like pathfinding, the path shouldn't be calculated each tick, just when there is no path
        // When does an enemy decided to shoot? On some time cycle? When it has no actions it's performing?
        // Do actions have a cooldown? Are actions an enum? Do they hold state?
    }
}
