import { Component, OnInit } from '@angular/core';
import { ApiService } from '../api.service';
import { StorageService } from '../storage.service';
import { Router } from '@angular/router';
import {environment} from "../../environments/environment";
import {UwpBridgeService} from "../uwpbridge.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  isLoading: boolean = false;
  userName: string = "";
  password: string = "";
  constructor(private apiService: ApiService, private storageService: StorageService, private router: Router,
              private uwpBridgeService: UwpBridgeService) {
  }

  ngOnInit() {
    if(environment.production && !this.uwpBridgeService.isRunningOnUwp()){
      this.router.navigate(['/download']);
    }
  }

  submitLogin(event: Event) {
    event.preventDefault();
    if (this.userName == "" || this.password == "") {
      return;
    }
    this.isLoading = true;
    this.apiService.login(this.userName, this.password).subscribe(
      (res) => {
        this.storageService.set({ token: res.accessToken })
        this.router.navigate(['/']);
      },
      () => this.isLoading = false
    )
  }
}
