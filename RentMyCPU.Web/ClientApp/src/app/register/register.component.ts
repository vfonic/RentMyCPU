import {Component, OnInit} from '@angular/core';
import {ApiService} from '../api.service';
import {Router} from '@angular/router';
import {StorageService} from '../storage.service';
import {environment} from "../../environments/environment";
import {UwpBridgeService} from "../uwpbridge.service";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  isLoading: boolean = false;
  userName: string = "";
  password: string = "";
  confirmPassword: string = "";

  constructor(private apiService: ApiService, private router: Router, private storageService: StorageService, private  uwpBridgeService: UwpBridgeService) {
  }

  ngOnInit() {
    if (environment.production && !this.uwpBridgeService.isRunningOnUwp()) {
      this.router.navigate(['/download']);
    }
  }

  submitRegister(event: Event) {
    event.preventDefault();
    if (this.userName == "" || this.password == "" || this.confirmPassword == "") {
      return;
    }
    this.isLoading = true;
    this.apiService.register(this.userName, this.password, this.confirmPassword).subscribe(
      () => {
        this.apiService.login(this.userName, this.password).subscribe(
          (res) => {
            this.storageService.set({token: res.accessToken});
            this.router.navigate(['/']);
          },
          () => this.isLoading = false
        )
      },
      () => this.isLoading = false
    )
  }
}
