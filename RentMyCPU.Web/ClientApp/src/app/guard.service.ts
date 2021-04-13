import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot, CanActivate } from '@angular/router';

import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { StorageService } from './storage.service';


// Guard for the index route of this extension

@Injectable()
export class Guard implements CanActivate {

  constructor(private router: Router, private storageService: StorageService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot)
    : Observable<boolean> | boolean {
    var token = this.storageService.get('token')
    if (token) {
      return true;
    }
    this.router.navigate(['/login']);
    return false; 
  }
}