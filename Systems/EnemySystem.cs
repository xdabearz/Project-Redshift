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

        public Entity SpawnEnemy(Vector2 spawnLocation, Texture2D texture)
        {
            // Will enemies have different type IDs? That likely needs to be included
            // Add an entity with some commands from the entity manager
            Entity entity = entityManager.CreateEntity();

            entityManager.AddComponent<TransformComponent>(entity, new TransformComponent
            {
                Position = spawnLocation
            });
            
            entityManager.AddComponent<GraphicComponent>(entity, new GraphicComponent
            {
                Offset = new Vector2(0, 0),
                Texture = texture
            });

            return entity;
        }
    }
}
