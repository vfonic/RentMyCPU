import {Component, HostListener, Input, OnInit} from '@angular/core';
import { StorageService } from '../storage.service';
import {ActivatedRoute, Router} from '@angular/router';
import { NotificationService } from '../notification.service';
import {UwpBridgeService} from "../uwpbridge.service";

class MenuEntry {
  public title: string;
  public isNew: boolean;
  public show: boolean;
  public link: string;
  public icon: string;
}

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {
  menuEntries: MenuEntry[];

  @HostListener('window:resize', ['$event'])
  onResize(event){
    if(event.target.innerWidth >= 768 && this.router.url === '/menu'){
      this.router.navigate(['/']);
    }
  }

  constructor(private storageService: StorageService, private router: Router, private notificationService: NotificationService,
              private uwpBridgeService: UwpBridgeService) { }

  ngOnInit() {
    this.menuEntries = [
      { title: 'Overview', link: '/', isNew: false, show: true, icon: 'fa-home' },
      { title: 'Create a task', link: '/create-task', isNew: false, show: true, icon: 'fa-file-code'  },
      { title: 'Buy credits', link: '/buy-credits', isNew: false, show: this.uwpBridgeService.isRunningOnUwp(), icon: 'fa-money-bill-wave' },
      { title: 'Statistics', link: '/stats', isNew: true, show: true , icon: 'fa-chart-pie' },
      { title: 'Help', link: '/help', isNew: false, show: true, icon: 'fa-lightbulb'  }
    ]
  }

  logOut() {
    this.notificationService.stop().then(res=>{
      this.storageService.remove('token');
      this.router.navigate(["/login"]);
    })
  }
}
