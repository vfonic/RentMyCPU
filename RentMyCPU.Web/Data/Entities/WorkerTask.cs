using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentMyCPU.Backend.Data.Entities
{
    public class RequestorTask : Entity
    {
        public Guid RequestorId { get; set; }
        public User Requestor { get; set; }
        public string WasmB64 { get; set; }

        public virtual ICollection<WorkerTask> WorkerTasks { get; set; }
    }
    public class WorkerTask : Entity
    {
        public Guid RequestorTaskId { get; set; }
        public Guid ProviderId { get; set; }
        public int Result { get; set; }
        public string Output { get; set; }
        public int Parameter { get; set; }
        public string Error { get; set; }
        public bool? IsSuccessful { get; set; }
        public bool IsExecuted { get; set; }
        public long? ElapsedMilliseconds { get; set; }
        public Guid WorkerId { get; set; }

        public virtual Worker Worker { get; set; }
        public virtual User Provider { get; set; }
        public virtual RequestorTask RequestorTask { get; set; }
    } 
}
