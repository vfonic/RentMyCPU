import { environment } from '../environments/environment'
import { HubConnectionBuilder, HubConnection, IHttpConnectionOptions } from '@aspnet/signalr'
import { StorageService } from './storage.service';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { UwpBridgeService } from './uwpbridge.service';

@Injectable({
    providedIn: 'root',
})
export class NotificationService {
    private connexion: HubConnection;
    private dictHandlers: object;

    private isConnectedSource = new BehaviorSubject<boolean>(false);
    public isConnected = this.isConnectedSource.asObservable();

    constructor(private storageService: StorageService,
        private uwpBridgeService: UwpBridgeService) {
        this.dictHandlers = {};
    }

    addHandler(action: string, handler: (res: string) => void) {
        if (!this.dictHandlers[action]) {
            this.connexion.on(action, handler);
            this.dictHandlers[action] = true;
        }
    }

    removeHandler(action: string) {
        if (this.dictHandlers[action]) {
            this.connexion.off(action);
            delete this.dictHandlers[action];
        }
    }

    start(): Promise<void> {
        if (!this.connexion) {
            this.connexion = new HubConnectionBuilder()
                .withUrl(environment.apiAddress + "taskhub" + this.getDeviceIdQueryString(), {
                    accessTokenFactory: () => this.storageService.get("token")
                    //transport: HttpTransportType.LongPolling
                })
                .build(); 
            this.connexion.onclose((error) => {
                this.isConnectedSource.next(false);
            });
        }
        if (!this.isConnectedSource.value) {
            var promise = this.connexion.start();
            promise.then(() => {
                this.isConnectedSource.next(true);
            })
            return promise;
        }
        else {
            return new Promise((resolve) => {
                resolve();
            })
        }
    }
    getDeviceIdQueryString() {
        var deviceInfos = this.uwpBridgeService.getDeviceInfos();
        if(deviceInfos){
            return "?device_id=" + deviceInfos.Id;
        }
        else return "";
    }

    invoke(action: string): Promise<any> {
        return this.connexion.invoke(action);
    }

    invokeWithMessage(action: string, message: string): Promise<any> {
        return this.connexion.invoke(action, message);
    }

    stop(): Promise<void> {
        if (this.isConnectedSource.value) {
            this.isConnectedSource.next(false);
            return new Promise<void>((resolve, reject) => {
                this.connexion.stop().then(() => {
                    this.connexion = null;
                    resolve();
                }).catch(() => {
                    reject();
                })
            })
        }
    }
}