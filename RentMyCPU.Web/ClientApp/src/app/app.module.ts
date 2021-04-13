import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoadingComponent } from './loading/loading.component';
import { MenuComponent } from './menu/menu.component';
import { MainComponent } from './main/main.component';
import { Guard } from './guard.service';
import { CreateTaskComponent } from './create-task/create-task.component';
import { BuyCreditsComponent } from './buy-credits/buy-credits.component';
import { HelpComponent } from './help/help.component';
import { StorageService } from './storage.service';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ApiService } from './api.service';
import { NotificationService } from './notification.service';
import { TaskDetailComponent } from './task-detail/task-detail.component';
import { ExecutorService } from './executor.service';
import { UwpBridgeService } from './uwpbridge.service';
import { WebworkerService } from './webworker.service';
import { StatsComponent } from './stats/stats.component';
import { LayoutComponent } from './layout/layout.component';
import { BarChartComponent } from './stats/bar-chart/bar-chart.component';
import { DownloadComponent } from './download/download.component';

@NgModule({
  declarations: [
    AppComponent,
    LoadingComponent,
    MenuComponent,
    MainComponent,
    CreateTaskComponent,
    BuyCreditsComponent,
    HelpComponent,
    LoginComponent,
    RegisterComponent,
    TaskDetailComponent,
    StatsComponent,
    LayoutComponent,
    BarChartComponent,
    DownloadComponent
  ],
  imports: [
    HttpClientModule,
    FormsModule,
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    AppRoutingModule
  ],
  providers: [
    Guard,
    StorageService,
    ApiService,
    UwpBridgeService,
    NotificationService,
    ExecutorService,
    WebworkerService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
