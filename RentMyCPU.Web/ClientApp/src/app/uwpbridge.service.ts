import {StorageService} from "./storage.service";
import {Injectable} from "@angular/core";
import {Router} from "@angular/router";
import {BehaviorSubject} from "rxjs";
import {environment} from "../environments/environment";

@Injectable({providedIn: 'root'})
export class UwpBridgeService {

  private buyResultSource = new BehaviorSubject<boolean>(false);
  public buyResult = this.buyResultSource.asObservable();

  constructor(private storageService: StorageService, private router: Router) {
    (window as any).buyResult = () => {
      this.buyResultSource.next(true);
    };
    (window as any).buyFailure = () => {
      this.buyResultSource.next(false);
    }
  }

  isRunningOnUwp() {
    return !!(window["uwpbridge"] || window["originalPostMessage"]);
  }

  showNotification(text: string) {
    window["uwpbridge"] && window["uwpbridge"].showNotification(text);
  }

  getDeviceInfos() {
    var easClientDeviceInformation = window["uwpbridge"] && JSON.parse(window["uwpbridge"].getDeviceInfos());

    if (!easClientDeviceInformation && !environment.production) {
      easClientDeviceInformation = {
        Id: "1b671a64-40d5-491e-99b0-da01ff1f3341",
        OperatingSystem: "Windows",
        FriendlyName: "Debug device"
      };
    }
    return easClientDeviceInformation;
  }

  buyCredits() {
    window["uwpbridge"] && window["uwpbridge"].buyCredits(this.storageService.get("token"));
  }
}
