using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentMyCPU.Backend.Data.Entities
{
    public class User : IdentityUser<Guid>, IEntity
    { 
        public User()
        {
            CreationDate = DateTimeOffset.Now;
            ModificationDate = DateTimeOffset.Now; 
        }
        public decimal Credits { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public DateTimeOffset ModificationDate { get; set; }

        public virtual ICollection<RequestorTask> RequestorTasks { get; set; }
        public virtual ICollection<WorkerTask> WorkerTasks { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
        public virtual ICollection<Worker> Workers { get; set; }
    }
}
