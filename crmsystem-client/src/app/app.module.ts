import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './core/app-routing.module';
import { MaterialDesignModule } from './core/material.module';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ProfileComponent } from './profile/profile.component';
import { ChangePersonalDataComponent } from './change-personal-data/change-personal-data.component';
import { ChangePasswordComponent } from './change-password/change-password.component';

import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ChangeAvatarComponent } from './change-avatar/change-avatar.component';
import { CompanyDetailsComponent } from './company-details/company-details.component';
import { EditCompanyDataDialogComponent } from './edit-company-data-dialog/edit-company-data-dialog.component';
import { EmployeesComponent } from './employees/employees.component';
import { EmployeeInvitationDialogComponent } from './employee-invitation-dialog/employee-invitation-dialog.component';
import { TasksComponent } from './tasks/tasks.component';
import { TaskCreateComponent } from './task-create/task-create.component';
import { EmployeeDetailsComponent } from './employee-details/employee-details.component';
import { TaskDetailsComponent } from './task-details/task-details.component';
import { CloudComponent } from './cloud/cloud.component';
import { CreateFolderDialogComponent } from './create-folder-dialog/create-folder-dialog.component';
import { RenameFileDialogComponent } from './rename-file-dialog/rename-file-dialog.component';
import { UploadFilesDialogComponent } from './upload-files-dialog/upload-files-dialog.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ChatComponent } from './chat/chat.component';
import { CreateChatDialogComponent } from './create-chat-dialog/create-chat-dialog.component';
import { ChatSettingsDialogComponent } from './chat-settings-dialog/chat-settings-dialog.component';
import { CookieService } from 'ngx-cookie-service';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CalendarComponent } from './calendar/calendar.component';
import { DataService } from './services/data.service';
import { AuthGuard } from './core/auth.guard';
import { FullCalendarModule } from '@fullcalendar/angular';
import { EventCreateDialogComponent } from './event-create-dialog/event-create-dialog.component';
import { EventEditDialogComponent } from './event-edit-dialog/event-edit-dialog.component';
import { LinkShortenerComponent } from './link-shortener/link-shortener.component';
import { ShortLinkCreateDialogComponent } from './short-link-create-dialog/short-link-create-dialog.component';
import { NgMaterialMultilevelMenuModule } from 'ng-material-multilevel-menu';
import { ExtendedRegistrationComponent } from './extended-registration/extended-registration.component';
import { LoaderComponent } from './loader/loader.component';
import { LoaderService } from './services/loader.service';
import { LoaderInterceptor } from './interceptors/loader.interceptor';
import { ForgotPasswordDialogComponent } from './forgot-password-dialog/forgot-password-dialog.component';

import { HttpService } from './services/http.service';
import { ProfileGuard } from './core/profile.guard';
import { FileDropZoneDirective } from './directives/file-drop-zone.directive';
import { CKEditorModule } from '@ckeditor/ckeditor5-angular';
import { PrivateNotificationDialogComponent } from './private-notification-dialog/private-notification-dialog.component';
import { EditEmployeeDialogComponent } from './edit-employee-dialog/edit-employee-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    ProfileComponent,
    ChangePersonalDataComponent,
    ChangePasswordComponent,
    ChangeAvatarComponent,
    CompanyDetailsComponent,
    EditCompanyDataDialogComponent,
    EmployeesComponent,
    EmployeeInvitationDialogComponent,
    TasksComponent,
    TaskCreateComponent,
    EmployeeDetailsComponent,
    TaskDetailsComponent,    
    CloudComponent,
    CreateFolderDialogComponent,
    RenameFileDialogComponent,
    UploadFilesDialogComponent,
    ChatComponent,
    CreateChatDialogComponent,
    ChatSettingsDialogComponent,
    DashboardComponent,
    CalendarComponent,
    EventCreateDialogComponent,
    EventEditDialogComponent,
    LinkShortenerComponent,
    ShortLinkCreateDialogComponent,
    ExtendedRegistrationComponent,
    LoaderComponent,
    ForgotPasswordDialogComponent,
    FileDropZoneDirective,
    PrivateNotificationDialogComponent,
    EditEmployeeDialogComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule, ReactiveFormsModule,
    AppRoutingModule,
    MaterialDesignModule,
    HttpClientModule,
    DragDropModule,
    FullCalendarModule,
    NgMaterialMultilevelMenuModule,    
    CKEditorModule
  ],
  providers: [
    CookieService,
    HttpService,
    DataService,
    AuthGuard,
    ProfileGuard,
    LoaderService,
    { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true }
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  entryComponents: [
    EditCompanyDataDialogComponent,
    EmployeeInvitationDialogComponent,
    CreateFolderDialogComponent,
    RenameFileDialogComponent,
    UploadFilesDialogComponent,
    CreateChatDialogComponent,
    ChatSettingsDialogComponent,
    EventCreateDialogComponent,
    EventEditDialogComponent,
    ShortLinkCreateDialogComponent,
    ForgotPasswordDialogComponent,
    PrivateNotificationDialogComponent,
    EditEmployeeDialogComponent
  ]
})

export class AppModule { }