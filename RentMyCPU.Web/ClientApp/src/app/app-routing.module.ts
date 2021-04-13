import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MenuComponent } from './menu/menu.component';
import { LoadingComponent } from './loading/loading.component';
import { MainComponent } from './main/main.component';
import { Guard } from './guard.service';
import { CreateTaskComponent } from './create-task/create-task.component';
import { BuyCreditsComponent } from './buy-credits/buy-credits.component';
import { HelpComponent } from './help/help.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { TaskDetailComponent } from './task-detail/task-detail.component';
import {StatsComponent} from "./stats/stats.component";
import {DownloadComponent} from "./download/download.component";

const routes: Routes = [
  { path: 'menu', component: MenuComponent, canActivate: [Guard] },
  { path: 'loading', component: LoadingComponent },
  { path: 'create-task', component: CreateTaskComponent, canActivate: [Guard] },
  { path: 'buy-credits', component: BuyCreditsComponent, canActivate: [Guard] },
  { path: 'help', component: HelpComponent, canActivate: [Guard] },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'task/:id', component: TaskDetailComponent },
  { path: 'stats', component: StatsComponent, canActivate: [Guard] },
  { path: 'download', component: DownloadComponent },
  { path: '**', component: MainComponent, canActivate: [Guard] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
