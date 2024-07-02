using System;
using System.Collections.Generic;

namespace Redshift
{
    public class EntityManager
    {
        // Entities are an identifier, a bitmask of components, and a position
        // for each of those components within their respective arrays
        private Entity[] entities;
        private int activeEntities;

        private Component[][] typeArrays;
        int[] activeCounts;

        public readonly int ComponentTypeCount;

        public EntityManager(int maxEntityCount=10000) 
        {
            entities = new Entity[maxEntityCount];
            activeEntities = 0;

            // Subtract one to ignore the Component.None default value
            ComponentTypeCount = Enum.GetNames(typeof(ComponentFlag)).Length - 1;

            // Initialize the arrays
            typeArrays = new Component[ComponentTypeCount][];
            activeCounts = new int[ComponentTypeCount];

            for (int i = 0; i < ComponentTypeCount; i++)
            {
                typeArrays[i] = new Component[maxEntityCount];
                activeCounts[i] = 0;
            }
        }

        public Entity CreateEntity()
        {
            int id = activeEntities++;
            entities[id] = new Entity(id);
            return entities[id];
        }

        public void AddComponent<T>(Entity entity, ComponentFlag flag, T component)
            where T : Component
        {
            // Need to check for the component already existing for this entity
            // ComponentFlag can also likely be simplified here

            int typeIndex = 0;
            if (flag != ComponentFlag.None)
            {
                // The enum counts by powers of 2, so this converts back to a linear count
                typeIndex = (int)Math.Log((int)flag, 2);
            }

            // Add the component to the EntityManager array
            int componentId = activeCounts[typeIndex]++;
            typeArrays[typeIndex][componentId] = component;

            // Set the flag in the Entity
            entities[entity.Id].ActiveComponents |= flag;

            // Set the created component id in the Entity
            entities[entity.Id].ComponentIds[typeIndex] = componentId;
        }

        public List<Entity> GetEntitiesByFlag(ComponentFlag flags)
        {
            List<Entity> matches = new();
            for (int i = 0; i < activeEntities; i++)
            {
                if (entities[i].ActiveComponents.HasFlag(flags))
                {
                    matches.Add(entities[i]);
                }
            }

            return matches;
        }

        public Entity GetEntityById(int entityId)
        {
            return entities[entityId];
        }

        public T GetComponent<T>(Entity entity)
            where T : Component
        {
            ComponentFlag flag = getComponentFlag<T>();

            // The component type is not active for this entity
            if (!entities[entity.Id].ActiveComponents.HasFlag(flag))
                return null;

            int typeIndex = (int)Math.Log((int)flag, 2);
            int componentId = entities[entity.Id].ComponentIds[typeIndex];

            if (typeArrays[typeIndex][componentId] is T component)
            {
                return component;
            }

            return null;
        }

        private ComponentFlag getComponentFlag<T>() 
            where T : Component
        {
            if (typeof(T) == typeof(TransformComponent)) return ComponentFlag.TransformComponent;
            if (typeof(T) == typeof(InputComponent)) return ComponentFlag.InputComponent;
            if (typeof(T) == typeof(GraphicComponent)) return ComponentFlag.GraphicComponent;
            if (typeof(T) == typeof(BoxCollider)) return ComponentFlag.BoxCollider;
            if (typeof(T) == typeof(EntityAttributes)) return ComponentFlag.EntityAttributes;
            if (typeof(T) == typeof(WeaponsList)) return ComponentFlag.WeaponsList;
            if (typeof(T) == typeof(WeaponDetails)) return ComponentFlag.WeaponDetails;

            throw new InvalidOperationException("Unsupported component type");
        }
    }
}
