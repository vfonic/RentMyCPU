using System;

namespace RentMyCPU.Shared
{
    public class TaskCreation
    {
        public string WasmB64 { get; set; }
        public int NumberOfNodes { get; set; }
    }

    public class RetryTask
    {
        public Guid Id { get; set; }
    }
}
