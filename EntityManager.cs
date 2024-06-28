using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Redshift
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

        private BoxCollider[] boxColliders;
        private int boxColliderCount;

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

            boxColliders = new BoxCollider[50];
            boxColliderCount = 0;
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

        public void AddComponent(int entityId, ComponentFlag newFlag, BoxCollider collider)
        {
            // Need to check for the component already existing for this entity

            int componentType = 0;
            if (newFlag != ComponentFlag.None)
            {
                // The enum counts by powers of 2, so this converts back to a linear count
                componentType = (int)Math.Log((int)newFlag, 2);
            }

            // Add the component to the EntityManager array
            int componentId = boxColliderCount++;
            boxColliders[componentId] = collider;

            // Set the flag in the Entity
            entities[entityId].ActiveComponents |= newFlag;

            // Set the created component id in the Entity
            entities[entityId].ComponentIds[componentType] = componentId;
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

        // Ref return type must be used here since we are modifying components directly (i.e. mutable)
        // Alternatives are using classes instead of structs since they are reference types by default
        // or making the structs immutable and creating/overwriting the original struct each time it 
        // needs to be updated. Not sure what's best
        public ref T GetComponent<T>(int entityId) where T : struct
        {
            int componentType = (int)Math.Log((int)getComponentFlag<T>(), 2);
            int componentId = entities[entityId].ComponentIds[componentType];

            if (typeof(T) == typeof(TransformComponent))
            {
                return ref Unsafe.As<TransformComponent, T>(ref transformComponents[componentId]);
            }
            else if (typeof(T) == typeof(InputComponent))
            {
                return ref Unsafe.As<InputComponent, T>(ref inputComponents[componentId]);
            }
            else if (typeof(T) == typeof(GraphicComponent))
            {
                return ref Unsafe.As<GraphicComponent, T>(ref graphicComponents[componentId]);
            }
            else if (typeof(T) == typeof(BoxCollider))
            {
                return ref Unsafe.As<BoxCollider, T>(ref boxColliders[componentId]);
            }
            else
            {
                throw new InvalidOperationException("Unsupported component type");
            }
        }

        private ComponentFlag getComponentFlag<T>() where T : struct
        {
            if (typeof(T) == typeof(TransformComponent)) return ComponentFlag.TransformComponent;
            if (typeof(T) == typeof(InputComponent)) return ComponentFlag.InputComponent;
            if (typeof(T) == typeof(GraphicComponent)) return ComponentFlag.GraphicComponent;
            if (typeof(T) == typeof(BoxCollider)) return ComponentFlag.BoxCollider;

            throw new InvalidOperationException("Unsupported component type");
        }

        private ref T getComponentFromArray<T>(ref T[] componentArray, int entityId, int componentType) where T : struct
        {
            return ref componentArray[entities[entityId].ComponentIds[componentType]];
        }
    }
}
