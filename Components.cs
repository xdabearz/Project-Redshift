using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redshift.Behaviors;

namespace Redshift
{
    [Flags] internal enum ComponentFlag
    {
        None = 0,
        GraphicComponent = 1 << 0,
        TransformComponent = 1 << 1,
        InputComponent = 1 << 2,
        Collider = 1 << 3,
        EntityAttributes = 1 << 4,
        WeaponsList = 1 << 5,
        WeaponDetails = 1 << 6,
        AIBehaviorComponent = 1 << 7,
    }

    internal abstract class Component 
    { 
        // If there is any necessary functionality for all components, it can be added here
    }

    internal class GraphicComponent : Component
    {
        public Vector2 Offset;
        public Texture2D Texture;
    }

    internal class TransformComponent : Component
    {
        public Vector2 Position;
    }

    // This just flags an entity as reading from the global input
    internal class InputComponent : Component
    {
        public bool EnableInput;
    }

    [Flags] internal enum CollisionLayer
    {
        None = 0,
        Player = 1 << 0,
        Enemy = 1 << 1,
        Projectile = 1 << 2,
        Environment = 1 << 3,
    }

    // May want to have different collider shapes
    internal class Collider : Component
    {
        public Rectangle Bounds;
        public CollisionLayer Layer;
        public CollisionLayer CollidesWith;
    }

    internal class EntityAttributes : Component
    {
        public float MovementSpeed;
        public int Hitpoints;
    }

    // This struct can likely get very big. Maybe not the best way to handle weapons, but
    // there needs to be some sort of parent-child association for entities. Possibly just
    // having a single "parent" for any number of children would be faster, but it would 
    // only be a one-way relationship and would be slow to search for all children from 
    // the parent's perspective
    internal class WeaponsList : Component
    {
        public int[] Active;
        public int[] EntityIds;
    }

    internal class WeaponDetails : Component
    {
        public float Cooldown; // Time between attacks in seconds
        public float LastFired;
        public float Damage;
        public float ProjectileSpeed;
    }

    internal class AIBehaviorComponent : Component
    {
        public List<Behavior> Behaviors;

        public AIBehaviorComponent()
        {
            Behaviors = new();
        }
    }
}
