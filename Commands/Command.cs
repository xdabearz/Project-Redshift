
namespace Redshift.Commands
{
    internal abstract class Command
    {
        // The entity to act upon
        public abstract void Execute(Entity entity);
    }
}
