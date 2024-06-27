using Gala.Commands;
using Microsoft.Xna.Framework;

namespace Gala.Behaviors
{
    internal enum BehaviorType
    {
        Limited,
        Repeated
    }

    internal struct BehaviorProperties
    {
        public int EntityId;
        public BehaviorType Type;
        public float Delay;
        public int Priority;
    }

    internal abstract class Behavior
    {
        public abstract int EntityId { get; }
        public abstract BehaviorType Type { get; }
        public abstract float Delay { get; }
        public abstract int Priority { get; }

        // This may need to be a List of commands
        public abstract Command Execute(World world, GameTime gameTime);
    }
}
