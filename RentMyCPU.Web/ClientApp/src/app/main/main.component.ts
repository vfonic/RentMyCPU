import { Component, OnInit, OnDestroy } from '@angular/core';
import { Task } from '../shared/task';
import { NotificationService } from '../notification.service';
import { ReceivedTask } from '../shared/receivedTask';
import { ExecutorService } from '../executor.service';
import { TaskErrorResponse } from '../shared/taskErrorResponse';
import { UwpBridgeService } from '../uwpbridge.service';
import { CannotCreateTask } from '../shared/cannotCreateTask';
import { StorageService } from '../storage.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.css']
})
export class MainComponent implements OnInit, OnDestroy {
  isIncomingTasks: boolean = true;
  incomingTasks: Task[] = [];
  outcomingTasks: Task[] = [];
  credits: number = 0;
  isLoading: boolean = false;
  showMenu: boolean= true;

  constructor(private notificationService: NotificationService,
    private executorService: ExecutorService,
    private uwpBridgeService: UwpBridgeService,
    private storageService: StorageService,
    private router: Router) { }

  ngOnInit() {
    this.isLoading = true;
    this.notificationService.start().then(() => {
      this.notificationService.addHandler("UpdatedStats", (res) => this.updateStats(res));
      this.notificationService.invoke("SendNewStatistics");
    })
    .catch(()=>{
      this.storageService.remove('token');
      this.router.navigate(["/login"]);
    });
  }

  updateStats(message) {
    var stats = JSON.parse(message);
    this.incomingTasks = stats.WorkerTasks as Task[];
    this.outcomingTasks = stats.RequestorTasks as Task[];
    this.credits = stats.Credits;
    this.isLoading = false;
  }

  ngOnDestroy() {
    this.notificationService.removeHandler("UpdatedStats");
  }
}
