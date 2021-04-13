using System;
using System.Collections.Generic;

namespace RentMyCPU.Shared
{
    public class TaskResponse
    {
        public Guid TaskId { get; set; }
        public long Elapsed { get; set; }
        public int Result { get; set; }
        public TaskContext Context { get; set; }
        public int Parameter { get; set; }
    }
    public class TaskErrorResponse
    {
        public Guid TaskId { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class TaskContext
    {
        public TaskContext()
        {
            WritedChars = new List<int>();
        }
        public List<int> WritedChars { get; set; } 
    }
}
