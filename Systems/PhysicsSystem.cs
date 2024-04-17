using Gala.Components;
using Microsoft.Xna.Framework;

namespace Gala.Systems
{
    public class PhysicsSystem
    {
        private int count;
        private Transform[] transforms;

        public PhysicsSystem(int arraySize=512) 
        {
            count = 0;
            // These get initialized to null
            transforms = new Transform[arraySize];
        }

        public int Add()
        {
            transforms[count] = new Transform(count);
            return count++;
        }

        public int Add(Vector2 position)
        {
            transforms[count] = new Transform(count, position);
            return count++;
        }

        public int Count
        {
            get { return count; }
        }

        public void ApplyMovement(int key, Vector2 movement)
        {
            transforms[key].Position += movement;
        }

        public Transform GetTransform(int key)
        {
            return transforms[key];
        }
    }
}
