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
        InputComponent = 4
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
    }
}
