using Gala.Interfaces;
using Microsoft.Xna.Framework;

namespace Gala.Components
{
    public class Transform : IComponent
    {
        public int Key { get; private set; }
        public Vector2 Position { get; set; }

        public Transform(int key)
        {
            Key = key;
            Position = Vector2.Zero;
        }

        public Transform(int key, Vector2 position)
        {
            Key = key;
            Position = position;
        }
    }
}
