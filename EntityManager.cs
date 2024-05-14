using System;
using System.Collections.Generic;

namespace Gala
{
    public class EntityManager
    {
        // Entities are an identifier, a bitmask of components, and a position
        // for each of those components within their respective arrays
        private Entity[] entities;
        private int activeEntities;

        private GraphicComponent[] graphicComponents;
        private int graphicComponentCount;

        private TransformComponent[] transformComponents;
        private int transformComponentCount;

        private InputComponent[] inputComponents;
        private int inputComponentCount;

        public EntityManager() 
        {
            entities = new Entity[200];
            activeEntities = 0;

            graphicComponents = new GraphicComponent[200];
            graphicComponentCount = 0;

            transformComponents = new TransformComponent[200];
            transformComponentCount = 0;

            inputComponents = new InputComponent[5];
            inputComponentCount = 0;
        }

        public int CreateEntity()
        {
            int id = activeEntities++;
            entities[id] = new Entity(id);
            return id;
        }

        public void AddComponent(int entityId, ComponentFlag flag, GraphicComponent graphicComponent)
        {
            // Need to check for the component already existing for this entity

            int componentType = 0;
            if (flag != ComponentFlag.None)
            {
                // The enum counts by powers of 2, so this converts back to a linear count
                componentType = (int)Math.Log((int)flag, 2);
            }

            // Add the component to the EntityManager array
            int componentId = graphicComponentCount++;
            graphicComponents[componentId] = graphicComponent;

            // Set the flag in the Entity
            entities[entityId].ActiveComponents |= flag;

            // Set the created component id in the Entity
            entities[entityId].ComponentIds[componentType] = componentId;
        }

        public void AddComponent(int entityId, ComponentFlag newFlag, TransformComponent transformComponent)
        {
            // Need to check for the component already existing for this entity

            int componentType = 0;
            if (newFlag != ComponentFlag.None)
            {
                // The enum counts by powers of 2, so this converts back to a linear count
                componentType = (int)Math.Log((int)newFlag, 2);
            }

            // Add the component to the EntityManager array
            int componentId = transformComponentCount++;
            transformComponents[componentId] = transformComponent;

            // Set the flag in the Entity
            entities[entityId].ActiveComponents |= newFlag;

            // Set the created component id in the Entity
            entities[entityId].ComponentIds[componentType] = componentId;
        }

        public void AddComponent(int entityId, ComponentFlag newFlag, InputComponent inputComponent)
        {
            // Need to check for the component already existing for this entity

            int componentType = 0;
            if (newFlag != ComponentFlag.None)
            {
                // The enum counts by powers of 2, so this converts back to a linear count
                componentType = (int)Math.Log((int)newFlag, 2);
            }

            // Add the component to the EntityManager array
            int componentId = inputComponentCount++;
            inputComponents[componentId] = inputComponent;

            // Set the flag in the Entity
            entities[entityId].ActiveComponents |= newFlag;

            // Set the created component id in the Entity
            entities[entityId].ComponentIds[componentType] = componentId;
        }

        public List<Entity> getMoveEntities()
        {
            List<Entity> moveEntites = new List<Entity>();
            ComponentFlag moveFlags = ComponentFlag.InputComponent | ComponentFlag.TransformComponent;
            for (int i = 0; i < activeEntities; i++)
            {
                if (entities[i].ActiveComponents.HasFlag(moveFlags))
                {
                    moveEntites.Add(entities[i]);
                }
            }

            return moveEntites;
        }

        public List<Entity> getDrawEntities()
        {
            List<Entity> drawEntites = new List<Entity>();
            ComponentFlag drawFlags = ComponentFlag.GraphicComponent | ComponentFlag.TransformComponent;
            for (int i = 0; i < activeEntities; i++)
            {
                if (entities[i].ActiveComponents.HasFlag(drawFlags))
                {
                    drawEntites.Add(entities[i]);
                }
            }

            return drawEntites;
        }

        public ref TransformComponent GetTransformComponent(int entityId)
        {
            int componentType = (int)Math.Log((int)ComponentFlag.TransformComponent, 2);
            return ref transformComponents[entities[entityId].ComponentIds[componentType]];
        }

        public ref InputComponent GetInputComponent(int entityId)
        {
            int componentType = (int)Math.Log((int)ComponentFlag.InputComponent, 2);
            return ref inputComponents[entities[entityId].ComponentIds[componentType]];
        }

        public ref GraphicComponent GetGraphicComponent(int entityId)
        {
            int componentType = (int)Math.Log((int)ComponentFlag.TransformComponent, 2);
            return ref graphicComponents[entities[entityId].ComponentIds[componentType]];
        }
    }
}
