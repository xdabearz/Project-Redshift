using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gala
{
    [Flags] public enum ComponentFlag
    {
        None = 0,
        GraphicComponent = 1,
        TransformComponent = 2,
        InputComponent = 4,
        BoxCollider = 8
    }

    public struct GraphicComponent
    {
        public Vector2 offset;
        public Texture2D texture;
    }

    public struct TransformComponent
    {
        public Vector2 position;
    }

    // This just flags an entity as reading from the global input
    public struct InputComponent
    {
        public bool enableInput;
    }

    // May want to have different collider shapes
    // Anything else for the colliders? Probably need a mask for what it can collide with
    public struct BoxCollider
    {
        public Rectangle collider;
    }
}
