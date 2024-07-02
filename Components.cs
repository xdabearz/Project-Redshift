using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Redshift
{
    [Flags] public enum ComponentFlag
    {
        None = 0,
        GraphicComponent = 1,
        TransformComponent = 2,
        InputComponent = 4,
        BoxCollider = 8,
        EntityAttributes = 16,
        WeaponsList = 32,
        WeaponDetails = 64,
    }

    public abstract class Component 
    { 
        // If there is any necessary functionality for all components, it can be added here
    }

    public class GraphicComponent : Component
    {
        public Vector2 Offset;
        public Texture2D Texture;
    }

    public class TransformComponent : Component
    {
        public Vector2 Position;
    }

    // This just flags an entity as reading from the global input
    public class InputComponent : Component
    {
        public bool EnableInput;
    }

    // May want to have different collider shapes
    // Anything else for the colliders? Probably need a mask for what it can collide with
    public class BoxCollider : Component
    {
        public Rectangle Bounds;
    }

    public class EntityAttributes : Component
    {
        public float MovementSpeed;
        public int Hitpoints;
    }

    // This struct can likely get very big. Maybe not the best way to handle weapons, but
    // there needs to be some sort of parent-child association for entities. Possibly just
    // having a single "parent" for any number of children would be faster, but it would 
    // only be a one-way relationship and would be slow to search for all children from 
    // the parent's perspective
    public class WeaponsList : Component
    {
        public int[] Active;
        public int[] EntityIds;
    }

    public class WeaponDetails : Component
    {
        public float Cooldown; // Time between attacks in seconds
        public float LastFired;
        public float Damage;
        public float ProjectileSpeed;
    }
}
