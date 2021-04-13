import {HttpClient, HttpHeaders} from '@angular/common/http';
import {environment} from '../environments/environment'
import {TokenResult} from './shared/token';
import {Injectable} from '@angular/core';
import {RequestorTaskDetail} from './shared/requestortaskdetail';
import {StorageService} from './storage.service';
import {UwpBridgeService} from './uwpbridge.service';

@Injectable()
export class ApiService {
  public static ApiAddress = "http://localhost:5000";

  constructor(private httpClient: HttpClient,
              private storageService: StorageService,
              private uwpBridgeService: UwpBridgeService) {

  }

  login(username: string, password: string) {
    let obj = {
      email: username,
      password: password
    };
    var easClientDeviceInformation = this.uwpBridgeService.getDeviceInfos();
    var headers = new HttpHeaders({
      "x-device-id": easClientDeviceInformation.Id,
      "x-device-os": easClientDeviceInformation.OperatingSystem,
      "x-device-name": easClientDeviceInformation.FriendlyName
    });
    return this.httpClient.post<TokenResult>(environment.apiAddress + "api/token/login", obj, {headers: headers});
  }

  register(username: string, password: string, confirmPassword: string) {
    let obj = {
      userName: username,
      password: password,
      confirmPassword: confirmPassword
    };
    return this.httpClient.post(environment.apiAddress + "api/token/register", obj);
  }

  getRequestorTasks(id: string) {
    var token = this.storageService.get('token');
    var headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + token
    });
    return this.httpClient.get<RequestorTaskDetail>(environment.apiAddress + "api/requestortask/" + id, {headers: headers});
  }

  getStatistics() {
    var token = this.storageService.get('token');
    var headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + token
    });
    return this.httpClient.get<any>(environment.apiAddress + "api/statistics", {headers: headers});
  }


}
