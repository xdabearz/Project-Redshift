using Gala.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gala.Components
{
    public class Graphics : IComponent
    {
        public int Key {  get; private set; }
        public Vector2 Offset { get; set; }
        public Texture2D Texture { get; set; }

        public Graphics(int key, Texture2D texture)
        {
            Key = key;
            Offset = Vector2.Zero;
            Texture = texture;
        }

        // This should be moved up to a graphics system that accepts a transform component as well as this component
        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(Texture, position + Offset, Color.White);
        }
    }
}
