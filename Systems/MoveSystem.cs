using Microsoft.Xna.Framework;

namespace Gala.Systems
{
    public class MoveSystem
    {
        public MoveSystem() { }

        // Make this accept some kind of Action?
        public void ApplyMovement(ref TransformComponent transform, Vector2 movement)
        {
            transform.position += movement;
        }
    }
}
