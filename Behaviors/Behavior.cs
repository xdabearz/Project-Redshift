using Redshift.Commands;
using Microsoft.Xna.Framework;
using System;

namespace Redshift.Behaviors
{
    internal enum BehaviorType
    {
        Limited,
        Repeated
    }

    internal struct BehaviorProperties
    {
        public Entity Entity;
        public BehaviorType Type;
        public float Delay;
        public int Priority;
        public int Limit;
        public int Completions;
        public Action Callback;

        public BehaviorProperties(Entity entity, BehaviorType type, float delay, int priority, Action callback = null, int limit=0)
        {
            Entity = entity;
            Type = type;
            Delay = delay;
            Priority = priority;
            Limit = limit;
            Completions = 0;
            Callback = callback;
        }
    }

    internal abstract class Behavior
    {
        public Entity Entity { get; }
        public BehaviorType Type { get; }
        public float Delay { get; }
        public int Priority { get; }
        public int Limit { get; }
        public int Completions { get; set; }
        public Action Callback { get; private set; }

        protected Behavior (BehaviorProperties properties)
        {
            Entity = properties.Entity;
            Type = properties.Type;
            Delay = properties.Delay;
            Priority = properties.Priority;
            Limit = properties.Limit;
            Completions = properties.Completions;
            Callback = properties.Callback;
        }

        // This may need to be a List of commands
        public abstract Command Execute(World world, GameTime gameTime);
    }
}
