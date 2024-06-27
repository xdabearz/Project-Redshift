
namespace Gala.Commands
{
    internal abstract class Command
    {
        // The entity to act upon
        public abstract void Execute(int entityId);
    }
}
