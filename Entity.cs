namespace Redshift
{
    public class Entity
    {
        public int Id { get; }
        public ComponentFlag ActiveComponents { get; set; }
        public int[] ComponentIds { get; }

        public Entity(int id)
        {
            Id = id;
            ActiveComponents = ComponentFlag.None;
            ComponentIds = new int[64];
        }
    }
}
