using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentMyCPU.Backend.Data;

namespace RentMyCPU.Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public StatisticsController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        [Route("")]
        public async Task<IActionResult> Get()
        {
            var executedTasks = await _applicationDbContext.WorkerTasks
                .Where(y => y.IsExecuted && y.IsSuccessful.Value)
                .Select(y => y.CreationDate)
                .GroupBy(y => y.Date)
                .Select(y => new
                {
                    Count = y.Count(),
                    y.First().Date,
                })
                .ToListAsync();

            var executedTasksOverTime = executedTasks.Select(x => new List<object> {x.Date.ToString("d"), x.Count})
                .ToList();

            var numberOfSuccessfulTasks = await _applicationDbContext.WorkerTasks
                .Where(y => y.IsExecuted && y.IsSuccessful.Value)
                .CountAsync();
            
            var percentageOfSuccessfulTasks = await _applicationDbContext.WorkerTasks
                .Where(y => y.IsExecuted).AnyAsync()
                ? 100 * numberOfSuccessfulTasks
                  / await _applicationDbContext.WorkerTasks
                      .Where(y => y.IsExecuted)
                      .CountAsync()
                : 0;
            
            var runningTasksNumber = await _applicationDbContext.WorkerTasks
                .Where(y => !y.IsExecuted)
                .CountAsync();


            var numberOfWorkersConnected = await _applicationDbContext.Workers.CountAsync(x => x.ConnexionId != null);

            return Ok(new{executedTasksOverTime, percentageOfSuccessfulTasks, numberOfWorkersConnected, numberOfSuccessfulTasks, runningTasksNumber});
        }
    }
}