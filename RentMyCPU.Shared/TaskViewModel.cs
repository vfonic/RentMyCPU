using System;

namespace RentMyCPU.Shared
{
    public class TaskViewModel
    {
        public long ElapsedMilliseconds { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public Guid Id { get; set; }
    }
}
