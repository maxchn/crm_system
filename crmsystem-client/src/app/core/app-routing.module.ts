import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { LoginComponent } from '../login/login.component';
import { RegisterComponent } from '../register/register.component';
import { ProfileComponent } from '../profile/profile.component';
import { CompanyDetailsComponent } from '../company-details/company-details.component';
import { EmployeesComponent } from '../employees/employees.component';
import { TasksComponent } from '../tasks/tasks.component';
import { TaskCreateComponent } from '../task-create/task-create.component';
import { EmployeeDetailsComponent } from '../employee-details/employee-details.component';
import { TaskDetailsComponent } from '../task-details/task-details.component';
import { CloudComponent } from '../cloud/cloud.component';
import { ChatComponent } from '../chat/chat.component';
import { CalendarComponent } from '../calendar/calendar.component';
import { DashboardComponent } from '../dashboard/dashboard.component';
import { AuthGuard } from './auth.guard';
import { LinkShortenerComponent } from '../link-shortener/link-shortener.component';
import { ExtendedRegistrationComponent } from '../extended-registration/extended-registration.component';
import { ProfileGuard } from './profile.guard';

const routes: Routes = [
  { path: '', component: LoginComponent, canActivate: [ProfileGuard], pathMatch: 'full' },
  { path: 'login', component: LoginComponent, canActivate: [ProfileGuard] },
  { path: 'join', component: RegisterComponent, canActivate: [ProfileGuard] },
  { path: 'extended_registration', component: ExtendedRegistrationComponent, canActivate: [AuthGuard] },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard, ProfileGuard] },
  { path: 'task/index', component: TasksComponent, canActivate: [AuthGuard, ProfileGuard] },
  { path: 'task/create', component: TaskCreateComponent, canActivate: [AuthGuard, ProfileGuard] },
  { path: 'task/edit/:id', component: TaskCreateComponent, canActivate: [AuthGuard, ProfileGuard] },
  { path: 'task/details/:id', component: TaskDetailsComponent, canActivate: [AuthGuard, ProfileGuard] },
  { path: 'employee/index', component: EmployeesComponent, canActivate: [AuthGuard, ProfileGuard] },
  { path: 'employee/details/:id', component: EmployeeDetailsComponent, canActivate: [AuthGuard, ProfileGuard] },
  { path: 'chat', component: ChatComponent, canActivate: [AuthGuard, ProfileGuard] },
  { path: 'cloud', component: CloudComponent, canActivate: [AuthGuard, ProfileGuard] },
  { path: 'calendar', component: CalendarComponent, canActivate: [AuthGuard, ProfileGuard] },
  { path: 'link_shortener', component: LinkShortenerComponent, canActivate: [AuthGuard, ProfileGuard] },
  { path: 'company/details', component: CompanyDetailsComponent, canActivate: [AuthGuard, ProfileGuard] },
  { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard, ProfileGuard] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }