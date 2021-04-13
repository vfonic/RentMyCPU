import { Component, OnInit, OnDestroy } from '@angular/core';
import { ApiService } from '../api.service';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { WorkerTaskDetail } from '../shared/workertaskdetail';
import { NotificationService } from '../notification.service';
import { ReceivedTask } from '../shared/receivedTask';
import { ExecutorService } from '../executor.service';
import { TaskErrorResponse } from '../shared/taskErrorResponse';

@Component({
  selector: 'app-task-detail',
  templateUrl: './task-detail.component.html',
  styleUrls: ['./task-detail.component.css']
})
export class TaskDetailComponent implements OnInit, OnDestroy {
  taskList: WorkerTaskDetail[] = [];
  id: string = "";

  constructor(private apiService: ApiService, private router: Router,
    private notificationService: NotificationService,
    private executorService: ExecutorService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    var obs = this.route.paramMap.pipe(
      switchMap((params: ParamMap) => {
        return this.apiService.getRequestorTasks(params.get('id'));
      })
    );
    obs.subscribe(res => {
      this.id = res.id;
      if (res.workerTasks) {
        this.taskList = res.workerTasks;
      }
    });
    this.notificationService.addHandler("UpdateWorkerTask", (res) => {
      var workerTaskUpdate = JSON.parse(res);
      if (this.taskList.some(x => x.id == workerTaskUpdate.Id)) {
        var workerTask = this.taskList.find(x => x.id == workerTaskUpdate.Id);
        workerTask.result = workerTaskUpdate.Result;
        workerTask.parameter = workerTaskUpdate.Parameter;
        workerTask.output = workerTaskUpdate.Output;
        workerTask.isSuccessful = workerTaskUpdate.IsSuccessful;
        workerTask.isExecuted = workerTaskUpdate.IsExecuted;
        workerTask.elapsedMilliseconds = workerTaskUpdate.ElapsedMilliseconds;
      }
    });
  }

  retry(task: WorkerTaskDetail) {
    this.notificationService.start().then(() => {
      this.notificationService.invokeWithMessage("RetryTask", JSON.stringify({
        id: task.id
      }));
    });
  }

  ngOnDestroy() {
    this.notificationService.removeHandler("UpdateWorkerTask"); 
  }
}
