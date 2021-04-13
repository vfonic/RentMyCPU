using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentMyCPU.Backend.Data;
using RentMyCPU.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RentMyCPU.Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class RequestorTaskController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public RequestorTaskController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var requestorTask = await _applicationDbContext.RequestorTasks
                .Where(x => x.Id == id)
                .Select(x => new RequestorTaskDetail
                {
                    Id = x.Id,
                    WorkerTasks = x.WorkerTasks.Select(y => new WorkerTaskDetail
                    {
                        Id = y.Id,
                        Result = y.Result,
                        Parameter = y.Parameter,
                        Output = y.Output,
                        IsSuccessful = y.IsSuccessful,
                        IsExecuted = y.IsExecuted,
                        ElapsedMilliseconds = y.ElapsedMilliseconds
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return Ok(requestorTask);
        }
    }
}
