using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentMyCPU.Backend.Data.Entities
{
    public class Purchase : Entity
    {
        public Guid TransactionId { get; set; }
        public string Receipt { get; set; } 
        public Guid UserId { get; set; }

        public virtual User User { get; set; }
    }
}
