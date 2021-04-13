import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';

@Injectable()
export class StorageService {
  constructor() {

  }

  get(key: string):string {
    return localStorage.getItem(key)
  }

  set(keys: any) {
      for (var key in keys) {
        localStorage.setItem(key, keys[key]);
      }
  }

  remove(keys: string) {
      localStorage.removeItem(keys);
  }
}
