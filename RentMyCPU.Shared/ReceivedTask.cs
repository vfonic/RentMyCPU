using System;

namespace RentMyCPU.Shared
{
    public class ReceivedTask
    {
        public Guid TaskId { get; set; }
        public int Parameter { get; set; }
        public string WasmB64 { get; set; }
        public int SubTaskTimeout { get; set; }
    }
}
