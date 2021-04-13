using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RentMyCPU.Backend.Data;
using RentMyCPU.Backend.Data.Entities;
using RentMyCPU.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RentMyCPU.Backend.Hubs
{
    [Authorize]
    public class TaskHub : Hub
    {
        private static readonly ConnectionMapping<string> _connections = new ConnectionMapping<string>();
        private readonly IConfiguration _config;

        public TaskHub(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendNewStatistics()
        {
            var id = Context.User.FindFirstValue(ClaimTypes.NameIdentifier).ToUpper();
            await SendNewStatisicsInternal(id);
        }

        public async Task FinishedTask(string message)
        {
            var task = JsonConvert.DeserializeObject<TaskResponse>(message);

            var id = Context.User.FindFirstValue(ClaimTypes.NameIdentifier).ToUpper();
            using (var applicationDbContext = ApplicationDbContext.FromConnectionString(_config))
            {
                var user = await applicationDbContext.Users
                    .Where(x => x.NormalizedEmail == id)
                    .FirstOrDefaultAsync();
                var dbTask = await applicationDbContext.WorkerTasks
                    .Include(x => x.RequestorTask)
                    .Where(x => x.Id == task.TaskId)
                    .FirstOrDefaultAsync();
                var requestor = await applicationDbContext.Users
                    .Where(x => x.Id == dbTask.RequestorTask.RequestorId)
                    .FirstOrDefaultAsync();

                decimal credits = task.Elapsed / 3600000m;

                if (requestor.Credits >= credits)
                {
                    requestor.Credits -= credits;
                    user.Credits += credits;
                    dbTask.ElapsedMilliseconds = task.Elapsed;
                    dbTask.Output = new string(task.Context.WritedChars.Select(x => (char)x).ToArray());
                    dbTask.IsExecuted = true;
                    dbTask.IsSuccessful = true;
                    dbTask.Result = task.Result;
                }
                else
                {
                    dbTask.IsExecuted = true;
                    dbTask.IsSuccessful = false;
                    dbTask.Error = "Insufficient credits.";
                }

                await applicationDbContext.SaveChangesAsync();

                var requestorConnexions = _connections.GetConnections(requestor.NormalizedUserName).ToList();
                if (requestorConnexions.Any())
                {
                    await Clients.Clients(requestorConnexions).SendAsync("UpdateWorkerTask", JsonConvert.SerializeObject(new WorkerTaskDetail
                    {
                        Id = dbTask.Id,
                        Result = dbTask.Result,
                        Parameter = dbTask.Parameter,
                        Output = dbTask.Output,
                        IsSuccessful = dbTask.IsSuccessful,
                        IsExecuted = dbTask.IsExecuted,
                        ElapsedMilliseconds = dbTask.ElapsedMilliseconds
                    }));
                }
                await SendNewStatisicsInternal(id, true);
            }
        }
        public async Task FailedTask(string message)
        {
            var task = JsonConvert.DeserializeObject<TaskErrorResponse>(message);

            using (var applicationDbContext = ApplicationDbContext.FromConnectionString(_config))
            {
                var dbTask = await applicationDbContext.WorkerTasks
                    .Include(x => x.RequestorTask)
                        .ThenInclude(x => x.Requestor)
                    .Where(x => x.Id == task.TaskId)
                    .FirstOrDefaultAsync();
                dbTask.IsExecuted = true;
                dbTask.IsSuccessful = false;
                dbTask.Error = task.ErrorMessage;
                await applicationDbContext.SaveChangesAsync();


                var requestorConnexions = _connections.GetConnections(dbTask.RequestorTask.Requestor.NormalizedUserName).ToList();
                if (requestorConnexions.Any())
                {
                    await Clients.Clients(requestorConnexions).SendAsync("UpdateWorkerTask", JsonConvert.SerializeObject(new WorkerTaskDetail
                    {
                        Id = dbTask.Id,
                        Result = dbTask.Result,
                        Parameter = dbTask.Parameter,
                        Output = dbTask.Output,
                        IsSuccessful = dbTask.IsSuccessful,
                        IsExecuted = dbTask.IsExecuted,
                        ElapsedMilliseconds = dbTask.ElapsedMilliseconds
                    }));
                }
            }
        }
        public async Task CreatedTask(string message)
        {
            var createdTask = JsonConvert.DeserializeObject<TaskCreation>(message);

            using (var applicationDbContext = ApplicationDbContext.FromConnectionString(_config))
            {
                var username = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await applicationDbContext.Users.Where(x => x.NormalizedEmail == username.ToUpper())
                    .Select(x => new { x.Id, x.Credits })
                    .FirstOrDefaultAsync();

                if (user.Credits <= 0)
                {
                    var error = new CannotCreateTask
                    {
                        Reason = "Insufficient credits."
                    };
                    await Clients.Caller.SendAsync("CannotCreateTask", JsonConvert.SerializeObject(error));
                    return;
                }

                var userId = user.Id;

                var dbRequestorTask = new RequestorTask
                {
                    RequestorId = userId,
                    WasmB64 = createdTask.WasmB64,
                    WorkerTasks = new List<WorkerTask>()
                };
#if DEBUG
                var allConnections = await applicationDbContext.Workers
                    .Where(x => x.ConnexionId != null)
                    .OrderBy(x => Guid.NewGuid())
                    .ToDictionaryAsync(x => x.ConnexionId, x => new { WorkerId = x.Id, x.UserId });
#else
                var allConnections = await applicationDbContext.Workers
                    .Where(x => x.ConnexionId != null && x.UserId != userId)
                    .OrderBy(x => Guid.NewGuid())
                    .ToDictionaryAsync(x => x.ConnexionId, x => new { WorkerId = x.Id, x.UserId });
#endif 
                var i = 0;
                Dictionary<string, string> tasksToSend = new Dictionary<string, string>();

                foreach (var conn in allConnections)
                {
                    var workerTask = new WorkerTask
                    {
                        Parameter = i,
                        ProviderId = conn.Value.UserId,
                        WorkerId = conn.Value.WorkerId
                    };
                    dbRequestorTask.WorkerTasks.Add(workerTask);
                    var response = new ReceivedTask
                    {
                        Parameter = i,
                        WasmB64 = createdTask.WasmB64,
                        TaskId = workerTask.Id,
                        SubTaskTimeout = 3600000
                    };
                    tasksToSend.Add(conn.Key, JsonConvert.SerializeObject(response));
                    i++;
                    if (i == createdTask.NumberOfNodes)
                    {
                        break;
                    }
                }

                if (tasksToSend.Count < createdTask.NumberOfNodes)
                {
                    var error = new CannotCreateTask
                    {
                        Reason = "Not enough nodes."
                    };
                    await Clients.Caller.SendAsync("CannotCreateTask", JsonConvert.SerializeObject(error));
                    return;
                }

                applicationDbContext.RequestorTasks.Add(dbRequestorTask);
                await applicationDbContext.SaveChangesAsync();

                foreach (var task in tasksToSend)
                {
                    await Clients.Client(task.Key).SendAsync("NewTask", task.Value);
                }

                await SendNewStatisicsInternal(username.ToUpper(), true);
            }
        }
        public async Task RetryTask(string message)
        {
            var taskToRetry = JsonConvert.DeserializeObject<RetryTask>(message);

            using (var applicationDbContext = ApplicationDbContext.FromConnectionString(_config))
            {
                var username = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await applicationDbContext.Users.Where(x => x.NormalizedEmail == username.ToUpper())
                    .Select(x => new { x.Id, x.Credits })
                    .FirstOrDefaultAsync();

                if (user.Credits <= 0)
                {
                    var error = new CannotCreateTask
                    {
                        Reason = "Insufficient credits."
                    };
                    await Clients.Caller.SendAsync("CannotCreateTask", JsonConvert.SerializeObject(error));
                    return;
                }

                var userId = user.Id;


#if DEBUG
                var workerUser = await applicationDbContext.Workers
                    .Where(x => x.ConnexionId != null)
                    .OrderBy(x => Guid.NewGuid())
                    .Select(x => new { x.ConnexionId, WorkerId = x.Id, x.UserId })
                    .FirstOrDefaultAsync();
#else
                var workerUser = await applicationDbContext.Workers
                    .Where(x => x.ConnexionId != null && x.UserId != userId)
                    .OrderBy(x => Guid.NewGuid())
                    .Select(x => new { x.ConnexionId, WorkerId = x.Id, x.UserId })
                    .FirstOrDefaultAsync();
#endif 

                if (workerUser == null)
                {
                    var error = new CannotCreateTask
                    {
                        Reason = "Not enough nodes."
                    };
                    await Clients.Caller.SendAsync("CannotCreateTask", JsonConvert.SerializeObject(error));
                    return;
                }

                var workerTask = await applicationDbContext.WorkerTasks
                    .Include(x => x.RequestorTask)
                    .Where(x => x.Id == taskToRetry.Id)
                    .FirstOrDefaultAsync();

                workerTask.IsExecuted = false;
                workerTask.IsSuccessful = null;
                workerTask.ProviderId = workerUser.UserId;
                workerTask.WorkerId = workerUser.WorkerId;

                var response = new ReceivedTask
                {
                    Parameter = workerTask.Parameter,
                    WasmB64 = workerTask.RequestorTask.WasmB64,
                    TaskId = workerTask.Id,
                    SubTaskTimeout = 3600000
                };

                await applicationDbContext.SaveChangesAsync();

                await Clients.Client(workerUser.ConnexionId).SendAsync("NewTask", JsonConvert.SerializeObject(response));

                var requestorConnexions = _connections.GetConnections(username.ToUpper()).ToList();
                if (requestorConnexions.Any())
                {
                    await Clients.Clients(requestorConnexions).SendAsync("UpdateWorkerTask", JsonConvert.SerializeObject(new WorkerTaskDetail
                    {
                        Id = workerTask.Id,
                        Result = workerTask.Result,
                        Parameter = workerTask.Parameter,
                        Output = workerTask.Output,
                        IsSuccessful = workerTask.IsSuccessful,
                        IsExecuted = workerTask.IsExecuted,
                        ElapsedMilliseconds = workerTask.ElapsedMilliseconds
                    }));
                }
            }
        }

        public override async Task OnConnectedAsync()
        {
            var id = Context.User.FindFirstValue(ClaimTypes.NameIdentifier).ToUpper();
            _connections.Add(id, Context.ConnectionId);
            var deviceIdParsed = Guid.TryParse(Context.GetHttpContext().Request.Query["device_id"].ToString(), out Guid deviceId);
            if (deviceIdParsed)
            {
                using (var applicationDbContext = ApplicationDbContext.FromConnectionString(_config))
                {
                    var device = await applicationDbContext.Workers
                        .Where(x => x.User.NormalizedUserName == id && x.DeviceId == deviceId)
                        .FirstOrDefaultAsync();

                    if (device != null)
                    {
                        if (device.ConnexionId != null)
                        {
                            Context.Abort();
                        }
                        else
                        {
                            device.ConnexionId = Context.ConnectionId;
                            await applicationDbContext.SaveChangesAsync();
                        }
                    }
                }
            }
            await base.OnConnectedAsync();
        }

        private async Task SendNewStatisicsInternal(string id, bool allConnectedDevices = false)
        {
            using (var applicationDbContext = ApplicationDbContext.FromConnectionString(_config))
            {
                var user = await applicationDbContext.Users
                    .Where(x => x.NormalizedUserName == id)
                    .Select(x => new UserViewModel
                    {
                        Credits = x.Credits,
                        WorkerTasks = x.WorkerTasks.Where(y => y.IsExecuted && y.IsSuccessful.Value)
                            .OrderByDescending(y => y.CreationDate)
                            .Select(y => new TaskViewModel
                            {
                                ElapsedMilliseconds = y.ElapsedMilliseconds.Value,
                                Id = y.Id,
                                CreationDate = y.CreationDate
                            }).ToList(),
                        RequestorTasks = x.RequestorTasks
                            .OrderByDescending(y => y.CreationDate)
                            .Select(y => new TaskViewModel
                            {
                                Id = y.Id,
                                CreationDate = y.CreationDate
                            }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (!allConnectedDevices)
                {
                    await Clients.Caller.SendAsync("UpdatedStats", JsonConvert.SerializeObject(user));
                }
                else
                {
                    await Clients.Clients(_connections.GetConnections(id).ToArray()).SendAsync("UpdatedStats", JsonConvert.SerializeObject(user));
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var id = Context.User.FindFirstValue(ClaimTypes.NameIdentifier).ToUpper();
            _connections.Remove(id, Context.ConnectionId);
            var deviceIdParsed = Guid.TryParse(Context.GetHttpContext().Request.Query["device_id"].ToString(), out Guid deviceId);
            if (deviceIdParsed)
            {
                using (var applicationDbContext = ApplicationDbContext.FromConnectionString(_config))
                {
                    var worker = await applicationDbContext.Workers
                        .Where(x => x.User.NormalizedUserName == id && x.DeviceId == deviceId)
                        .FirstOrDefaultAsync();

                    if (worker != null && worker.ConnexionId != null && worker.ConnexionId == Context.ConnectionId)
                    {
                        worker.ConnexionId = null;
                        await applicationDbContext.SaveChangesAsync();

                        var dbTasks = await applicationDbContext.WorkerTasks
                            .Include(x => x.RequestorTask)
                                .ThenInclude(x => x.Requestor)
                            .Where(x => x.IsExecuted == false && x.WorkerId == worker.Id)
                            .ToListAsync();

                        foreach (var dbTask in dbTasks)
                        {
                            dbTask.IsExecuted = true;
                            dbTask.IsSuccessful = false;
                            dbTask.Error = "App closed";
                        }
                        await applicationDbContext.SaveChangesAsync();
                        foreach (var dbTask in dbTasks)
                        {
                            var requestorConnexions = _connections.GetConnections(dbTask.RequestorTask.Requestor.NormalizedUserName).ToList();
                            if (requestorConnexions.Any())
                            {
                                await Clients.Clients(requestorConnexions).SendAsync("UpdateWorkerTask", JsonConvert.SerializeObject(new WorkerTaskDetail
                                {
                                    Id = dbTask.Id,
                                    Result = dbTask.Result,
                                    Parameter = dbTask.Parameter,
                                    Output = dbTask.Output,
                                    IsSuccessful = dbTask.IsSuccessful,
                                    IsExecuted = dbTask.IsExecuted,
                                    ElapsedMilliseconds = dbTask.ElapsedMilliseconds
                                }));
                            }
                        }
                    }
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

    }


}
