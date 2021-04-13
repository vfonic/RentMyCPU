import {Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {ApiService} from "../api.service";
import * as d3 from "d3";
import {Observable, Subscription} from "rxjs";

@Component({
  selector: 'app-stats',
  templateUrl: './stats.component.html',
  styleUrls: ['./stats.component.css']
})
export class StatsComponent implements OnInit, OnDestroy {
  showMenu: boolean= true;
  chartData: Array<Array<any>>;
  percentageOfSuccessfulTasks: number;
  numberOfWorkersConnected: number;
  numberOfSuccessfulTasks: number;
  runningTasksNumber: number;
  subscription: Subscription;

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.subscription = this.apiService.getStatistics().subscribe(x=>{
      this.chartData = x.executedTasksOverTime;
      this.percentageOfSuccessfulTasks = x.percentageOfSuccessfulTasks;
      this.numberOfWorkersConnected = x.numberOfWorkersConnected;
      this.numberOfSuccessfulTasks  = x.numberOfSuccessfulTasks;
      this.runningTasksNumber = x.runningTasksNumber;
    });
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

}
