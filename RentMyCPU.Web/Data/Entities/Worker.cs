using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentMyCPU.Backend.Data.Entities
{
    public class Worker : Entity
    {
        public Guid DeviceId { get; set; }
        public string Name { get; set; }
        public string OS { get; set; }
        public Guid UserId { get; set; }
        public string ConnexionId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<WorkerTask> Tasks { get; set; }
    }
}
