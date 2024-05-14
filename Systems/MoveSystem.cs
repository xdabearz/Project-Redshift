using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Gala.Systems
{
    public class MoveSystem
    {
        public MoveSystem() { }

        public void ApplyMovement(ref TransformComponent transform, Vector2 movement)
        {
            transform.position += movement;
        }
    }
}
