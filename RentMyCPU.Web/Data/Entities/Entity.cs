using System;

namespace RentMyCPU.Backend.Data.Entities
{
    public class Entity : IEntity
    {
        public Entity()
        {
            CreationDate = DateTimeOffset.Now;
            ModificationDate = DateTimeOffset.Now;
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset ModificationDate { get; set; }
    }
}
