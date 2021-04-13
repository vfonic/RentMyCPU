using System;

namespace RentMyCPU.Backend.Data.Entities
{
    internal interface IEntity
    {
        Guid Id { get; set; }
        DateTimeOffset CreationDate { get; set; }
        DateTimeOffset ModificationDate { get; set; }
    }
}
