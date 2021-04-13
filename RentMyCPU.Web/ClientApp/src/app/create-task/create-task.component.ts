import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NotificationService } from '../notification.service';
import { TaskCreation } from '../shared/taskCreation';
import { ApiService } from '../api.service';
import { ExecutorService } from '../executor.service';

@Component({
  selector: 'app-create-task',
  templateUrl: './create-task.component.html',
  styleUrls: ['./create-task.component.css']
})
export class CreateTaskComponent implements OnInit {
  numberOfNodes: number = 1;
  fileContent: string = null;
  error: string = null;
  showMenu: boolean= true;
  constructor(private router: Router, private notificationService: NotificationService, private executorService: ExecutorService) { }

  ngOnInit() {
  }

  handleInputChange(e) {
    var file = e.dataTransfer ? e.dataTransfer.files[0] : e.target.files[0];
    var reader = new FileReader();
    reader.onload = this._handleReaderLoaded.bind(this);
    reader.readAsDataURL(file);
  }
  _handleReaderLoaded(e) {
    let reader = e.target;
    this.fileContent = reader.result.replace("data:application/wasm;base64,", "").replace("data:;base64,", "");
  }

  confirmTask() {
    this.executorService.isValid(this.fileContent).then(res => {
      if (!this.fileContent || !this.fileContent.length || !res) {
        this.error = "Cannot create task: please choose a valid file."
      }
      else {
        var creation = new TaskCreation();
        creation.WasmB64 = this.fileContent;
        creation.NumberOfNodes = this.numberOfNodes;

        this.notificationService.start().then(res => {
          this.router.navigate(['/']).then(() => {
            this.notificationService.invokeWithMessage("CreatedTask", JSON.stringify(creation));
          });
        });
      }
    })
  }
}
