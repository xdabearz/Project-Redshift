using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Redshift
{
    internal class EntityManager
    {
        // Entities
        private Dictionary<int, Entity> entities;
        private List<int> recycledEntityIds;
        private int activeEntities;

        // Components
        private Dictionary<int, Component>[] components;
        private List<int>[] recycledComponentIds;
        private int[] componentCounts;

        private List<Entity> entitiesToDelete;

        public readonly int ComponentTypeCount;

        public EntityManager(int maxEntityCount=10000) 
        {
            entities = new();
            recycledEntityIds = new();
            activeEntities = 0;

            // Subtract one to ignore the Component.None default value
            ComponentTypeCount = Enum.GetNames(typeof(ComponentFlag)).Length - 1;

            // Initialize the arrays
            components = new Dictionary<int, Component>[ComponentTypeCount];
            recycledComponentIds = new List<int>[ComponentTypeCount];
            componentCounts = new int[ComponentTypeCount];

            for (int i = 0; i < ComponentTypeCount; i++)
            {
                components[i] = new Dictionary<int, Component>();
                recycledComponentIds[i] = new List<int>();
                componentCounts[i] = 0;
            }

            entitiesToDelete = new();
        }

        public Entity CreateEntity()
        {
            int id;
            if (recycledEntityIds.Count > 0)
            {
                id = recycledEntityIds[0];
                recycledEntityIds.RemoveAt(0);
            }
            else
            {
                id = activeEntities++;
            }
            entities.Add(id, new Entity(id));
            return entities[id];
        }

        public void AddComponent<T>(Entity entity, T component)
            where T : Component
        {
            // Need to check for the component already existing for this entity
            ComponentFlag flag = getComponentFlag<T>();

            int typeIndex = 0;
            if (flag != ComponentFlag.None)
            {
                // The enum counts by powers of 2, so this converts back to a linear count
                typeIndex = (int)Math.Log((int)flag, 2);
            }

            // Add the component to the EntityManager array
            int componentId;
            if (recycledComponentIds[typeIndex].Count > 0)
            {
                componentId = recycledComponentIds[typeIndex][0];
                recycledComponentIds[typeIndex].RemoveAt(0);
            }
            else
            {
                componentId = componentCounts[typeIndex]++;
            }
            components[typeIndex][componentId] = component;

            // Set the flag in the Entity
            entities[entity.Id].ActiveComponents |= flag;

            // Set the created component id in the Entity
            entities[entity.Id].ComponentIds[typeIndex] = componentId;
        }

        public void DeleteEntity(Entity entity)
        {
            // This can do more such as disabling an entity to prevent it from
            // being used in further logic for this game frame

            entitiesToDelete.Add(entity);
        }

        public void CleanupEntities()
        {
            if (entitiesToDelete.Count == 0)
                return;

            foreach (Entity entity in entitiesToDelete)
            {
                // Delete the active components first
                for (int i = 0; i < ComponentTypeCount; i++)
                {
                    ComponentFlag flag = (ComponentFlag)Math.Pow(2, i);
                    int componentId = entity.ComponentIds[i];
                    if (entity.ActiveComponents.HasFlag(flag))
                    {
                        components[i].Remove(componentId);
                        recycledComponentIds[i].Add(componentId);
                    }
                }

                // Delete the entity
                recycledEntityIds.Add(entity.Id);
                entities.Remove(entity.Id);
            }

            entitiesToDelete = new();
        }

        public List<Entity> GetEntitiesByFlag(ComponentFlag flags)
        {
            List<Entity> matches = new();
            foreach(Entity entity in entities.Values)
            {
                if (entity.ActiveComponents.HasFlag(flags))
                {
                    matches.Add(entity);
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

            // The entity is no longer active
            if (!entities.ContainsKey(entity.Id))
                return null;

            // The component type is not active for this entity
            if (!entities[entity.Id].ActiveComponents.HasFlag(flag))
                return null;

            int typeIndex = (int)Math.Log((int)flag, 2);
            int componentId = entities[entity.Id].ComponentIds[typeIndex];

            if (components[typeIndex][componentId] is T component)
            {
                return component;
            }

            return null;
        }

        private ComponentFlag getComponentFlag<T>() 
            where T : Component
        {
            if (typeof(T) == typeof(TransformComponent)) return ComponentFlag.TransformComponent;
            else if (typeof(T) == typeof(InputComponent)) return ComponentFlag.InputComponent;
            else if (typeof(T) == typeof(GraphicComponent)) return ComponentFlag.GraphicComponent;
            else if (typeof(T) == typeof(Collider)) return ComponentFlag.Collider;
            else if (typeof(T) == typeof(EntityAttributes)) return ComponentFlag.EntityAttributes;
            else if (typeof(T) == typeof(WeaponsList)) return ComponentFlag.WeaponsList;
            else if (typeof(T) == typeof(WeaponDetails)) return ComponentFlag.WeaponDetails;
            else if (typeof(T) == typeof(AIBehaviorComponent)) return ComponentFlag.AIBehaviorComponent;
            else throw new InvalidOperationException("Unsupported component type");
        }
    }
}
