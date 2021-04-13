import { Component, OnInit, OnDestroy } from '@angular/core';
import { UwpBridgeService } from '../uwpbridge.service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-buy-credits',
  templateUrl: './buy-credits.component.html',
  styleUrls: ['./buy-credits.component.css']
})
export class BuyCreditsComponent implements OnInit, OnDestroy{

  private buyResultRef: Subscription = null;
  isLoading:boolean = false;
  showMenu: boolean = true;
  constructor(private uwpBridgeService: UwpBridgeService, private router: Router) { }

  ngOnInit(){
    this.buyResultRef = this.uwpBridgeService.buyResult.subscribe(res=>{
      this.isLoading = false;
      if(res){
        this.uwpBridgeService.showNotification("Credits added");
        this.router.navigate(["/"]);
      }
    })
  }

  buyCredits() {
    this.isLoading = true;
    this.uwpBridgeService.buyCredits();
  }

  ngOnDestroy(){
    this.buyResultRef.unsubscribe();
  }
}
