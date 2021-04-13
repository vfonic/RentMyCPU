import { Component } from '@angular/core';
import { NotificationService } from './notification.service';
import { ApiService } from './api.service';
import { ReceivedTask } from './shared/receivedTask';
import { ExecutorService } from './executor.service';
import { TaskErrorResponse } from './shared/taskErrorResponse';
import { CannotCreateTask } from './shared/cannotCreateTask';
import { UwpBridgeService } from './uwpbridge.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(private notificationService: NotificationService,
    private executorService: ExecutorService,
    private uwpBridgeService: UwpBridgeService) {
    window.onbeforeunload = () => {
      this.notificationService.stop();
    }
    this.notificationService.isConnected.subscribe(connected => {
      if (connected) {
        this.notificationService.addHandler("NewTask", (res) => { this.newTask(res); });
        this.notificationService.addHandler("CannotCreateTask", (res) => { this.cannotCreateTask(res); });
      }
      else {
        this.notificationService.removeHandler("NewTask");
        this.notificationService.removeHandler("CannotCreateTask");
      }
    })
  }

  newTask(res) {
    var task = JSON.parse(res) as ReceivedTask;
    this.executorService.execute(task).then(result => {
      this.notificationService.invokeWithMessage("FinishedTask", JSON.stringify(result));
    }).catch(error => {
      var response = new TaskErrorResponse();
      response.ErrorMessage = error;
      response.TaskId = task.TaskId;
      this.notificationService.invokeWithMessage("FailedTask", JSON.stringify(response));
    })
    setTimeout(() => {
      if (this.executorService.currentTaskExecutedId == task.TaskId) {
        this.executorService.stop();
        var response = new TaskErrorResponse();
        response.ErrorMessage = "Task cancelled";
        response.TaskId = task.TaskId;
        this.notificationService.invokeWithMessage("FailedTask", JSON.stringify(response));
      }
    }, task.SubTaskTimeout);
  }

  cannotCreateTask(res) {
    var data = JSON.parse(res) as CannotCreateTask;
    this.uwpBridgeService.showNotification("Unable to create task: " + data.Reason);
  }
}
