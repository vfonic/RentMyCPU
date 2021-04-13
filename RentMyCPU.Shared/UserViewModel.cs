using System.Collections.Generic;

namespace RentMyCPU.Shared
{
    public class UserViewModel
    {
        public List<TaskViewModel> WorkerTasks { get; set; }
        public List<TaskViewModel> RequestorTasks { get; set; }
        public decimal Credits { get; set; }
    }
}
