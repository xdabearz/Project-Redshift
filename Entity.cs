﻿namespace Redshift
{
    internal class Entity
    {
        public bool Enabled { get; set; }
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
