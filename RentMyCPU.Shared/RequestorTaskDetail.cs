using System;
using System.Collections.Generic;

namespace RentMyCPU.Shared
{
    public class RequestorTaskDetail
    {
        public Guid Id { get; set; }
        public IEnumerable<WorkerTaskDetail> WorkerTasks { get; set; }
    }

    public class WorkerTaskDetail
    {
        public int Result { get; set; }
        public int Parameter { get; set; }
        public string Output { get; set; }
        public bool IsExecuted { get; set; }
        public bool? IsSuccessful { get; set; }
        public long? ElapsedMilliseconds { get; set; }
        public Guid Id { get; set; }
    }
}
