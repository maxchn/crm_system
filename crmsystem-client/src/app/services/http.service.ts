import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpEventType, HttpParams } from '@angular/common/http';
import { Observable, Observer } from 'rxjs';
import { RegistrationModel } from '../register/register.component';
import { LoginModel } from '../login/login.component';
import { Utils } from '../core/utils';
import { ProfileModel } from '../models/profileModel';
import { map } from 'rxjs/operators';
import { Task } from '../models/taskModel';
import { FileModel } from '../models/fileModel';
import { DataService } from './data.service';
import { resolve } from 'path';

@Injectable()
export class HttpService {

    constructor(private http: HttpClient,
        private dataService: DataService) { }

    signUp(model: RegistrationModel, token: string): Observable<any> {
        const body = {
            email: model.login,
            password: model.password,
            confirmPassword: model.confirmPassword
        };

        return this.http.post(`${Utils.API_BASE_URL}/Account/Register${token != null ? `?token=${token}` : ``}`, body);
    }

    extendedSignUp(model: any): Observable<any> {
        return this.http.post(`${Utils.API_BASE_URL}/Account/ExtendedRegistration`, model);
    }

    sendInvitation(email: string, companyId: number): Observable<any> {
        let headers = new HttpHeaders().set('Content-Type', 'application/json');
        headers = headers.set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.API_BASE_URL}/Account/SendInvitation?email=${email}&companyId=${companyId}`, null, { headers: headers });
    }

    signIn(model: LoginModel): Observable<any> {
        const body = {
            login: model.login,
            password: model.password
        };

        return this.http.post(`${Utils.API_BASE_URL}/Account/Login`, body);
    }

    logout(): Observable<any> {
        return this.http.post(`${Utils.API_BASE_URL}/Account/Logout`, null);
    }

    forgotPassword(email: string): Observable<any> {

        let params = new HttpParams()
            .set('email', email)
            .set('callbackUrl', Utils.CLIENT_URL + "/reset_password");

        return this.http.post(`${Utils.API_BASE_URL}/Account/ForgotPassword`, null,
            { params: params });
    }

    loadUserInfo(): Observable<any> {
        let headers = new HttpHeaders().set('Content-Type', 'application/json');
        headers = headers.set('authorization', this.dataService.getFullToken());

        return this.http.get(`${Utils.API_BASE_URL}/Account/GetDetailsInfo`, { headers: headers });
    }

    async getIsFullProfile(id: string) {
        let query = `query GetUserProfileIsFull($id: String!) {
            userProfileIsFull(id: $id)
          }`;

        let variables = {
            "id": id
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        let isFull = false;
        await this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers })
            .toPromise()
            .then(res => {
                isFull = res["data"].userProfileIsFull;
            })
            .catch(err => {
                return Promise.reject(err.error || 'Error');
            });

        return isFull;
    }

    loadUserRoles(): Observable<any> {
        return this.http.get(`${Utils.API_BASE_URL}/Account/GetRoles`)
    }

    loadEmployeeCompany(id: string): Observable<any> {
        let query = `query GetEmployeeCompany($id: String!) {
            employeeCompany(employeeId: $id) {
              companyId,
              name,
              urlName
            }
          }`;

        let variables = {
            "id": id
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    updateProfile(model: ProfileModel, companyId: number): Observable<any> {
        let query = `mutation($user: ApplicationUserInput!, $companyId: Int!) {
            updateUser(user: $user, companyId: $companyId) {
              status,
              message
            }
          }`;

        let variables = {
            "user": {
                "id": model.id,
                "lastName": model.lastName,
                "firstName": model.firstName,
                "patronymic": model.patronymic,
                "dateOfBirth": model.dayOfBirth,
                "gender": {
                    "genderId": model.gender.id
                },
                "department": {
                    "name": model.department.name
                },
                "position": {
                    "name": model.position.name
                },
                "phones": model.phones
            },
            "companyId": companyId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    changePassword(oldPassword: string, newPassword: string, confirmPassword: string): Observable<any> {
        const body = {
            oldPassword: oldPassword,
            newPassword: newPassword,
            confirmPassword: confirmPassword
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.API_BASE_URL}/Account/ChangePassword`, body, { headers: headers });
    }

    loadCompanyInfo(id: number): Observable<any> {
        let query = `query($id: Int!) {
            company(id: $id) {
              companyId,
              name,
              owner {
                lastName,
                firstName,
                patronymic
              },
              urlName
            }
          }`;

        let variables = {
            "id": id
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    loadCompanyPermission(id: number): Observable<any> {
        let query = `query($id: Int!) {
            getPermissionOnUpdatingCompanyData(id: $id)
          }`;

        let variables = {
            "id": id
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    updateCompany(id: number, name: string, url: string): Observable<any> {
        let query = `mutation($company: CompanyInput!) {
            updateCompany(company: $company) {
              status,
              message
            }
          }`;

        let variables = {
            "company": {
                "companyId": id,
                "name": name,
                "urlName": url
            }
        };

        let headers = new HttpHeaders().set('Content-Type', 'application/json');
        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    loadEmployees(companyId: number): Observable<any> {
        let query = `query ($id: Int!) {
            employees(id: $id) {
              id,
              lastName,
              firstName,
              patronymic,
              email,
              department {
                name
              },
              position {
                name
              }
            }
          }`;

        let variables = {
            "id": companyId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    uploadAvatar(file: any): Observable<any> {
        const uploadData = new FormData();
        uploadData.append('file', file);

        let headers = new HttpHeaders()
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.API_BASE_URL}/Account/UploadAvatar`, uploadData, { headers: headers, reportProgress: true, observe: 'events' })
            .pipe(map((event) => {
                switch (event.type) {
                    case HttpEventType.UploadProgress:
                        const progress = Math.round(100 * event.loaded / event.total);
                        
                        return { status: 'progress', message: progress };
                    case HttpEventType.Response:
                        return event.body;
                    default:
                        return `Unhandled event: ${event.type}`;
                }
            }));
    }

    downloadAvatar(id: string): Observable<any> {
        let headers = new HttpHeaders()
            .set('authorization', this.dataService.getFullToken())
            .set('Accept', 'image/*');

        return this.http.get(`${Utils.API_BASE_URL}/Account/DownloadAvatar?id=${id}`, { headers: headers, responseType: 'blob' });
    }

    loadEmployeeInfo(id: string): Observable<any> {
        let query = `query ($id: String!) {
            user(id: $id) {
              id,
              lastName,
              firstName,
              dateOfBirth,
              patronymic,
              email,
              department {
                name
              },
              position {
                name
              },
              gender {
                genderId,
                name
              },
              phones {
                phoneNumber
              }
            }
          }`;

        let variables = {
            "id": id
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    registerNewEmployee(model: ProfileModel, companyId: number, isSendLoginPasswordOnEmail: Boolean): Observable<any> {
        
        let query = `mutation($employee: EmployeeInput!, $companyId: Int!, $isSendLoginPasswordOnEmail: Boolean!) {
            createEmployee(employee: $employee, companyId: $companyId, isSendLoginPasswordOnEmail: $isSendLoginPasswordOnEmail) {
              status,
              employee {
                id,
                lastName,
                firstName,
                patronymic,
                email,
                department {
                  name
                },
                position {
                  name
                }
              }
            }
          }`;

        let variables = {
            "companyId": companyId,
            "isSendLoginPasswordOnEmail": isSendLoginPasswordOnEmail,
            "employee": {
                "email": model.email,
                "lastName": model.lastName,
                "firstName": model.firstName,
                "patronymic": model.patronymic,
                "department": {
                    "name": model.department
                },
                "position": {
                    "name": model.position
                },
            }
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    createNewTask(task: Task, companyId: number): Observable<any> {
        let query = `mutation($task: TaskInput!, $companyId: Int!) {
            createTask(task: $task, companyId: $companyId) {
              status,
              message,
              value
          }
        }`;

        let variables = {
            "task": {
                "name": task.name,
                "body": task.body,
                "deadline": task.deadline,
                "author": {
                    "id": task.author.id
                },
                "isImportant": task.isImportant,
                "company": {
                    "companyId": task.company.id
                },
                "coExecutors": task.coExecutors,
                "observers": task.observers,
                "responsiblesForExecution": task.responsibleForExecution,
                "taskTags": task.tags,
                "attachedFiles": task.attachedFiles
            },
            "companyId": companyId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    updateTask(task: Task): Observable<any> {
        let query = `mutation($task: TaskInput!) {
            updateTask(task: $task) {
                status,
                message,
                value
          }
        }`;

        let variables = {
            "task": {
                "taskId": task.taskId,
                "name": task.name,
                "body": task.body,
                "deadline": task.deadline,
                "author": {
                    "id": task.author.id
                },
                "isImportant": task.isImportant,
                "company": {
                    "companyId": task.company.id
                },
                "coExecutors": task.coExecutors,
                "observers": task.observers,
                "responsiblesForExecution": task.responsibleForExecution,
                "taskTags": task.tags,
                "attachedFiles": task.attachedFiles
            }
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    getTaskDetailsInfoById(taskId: number): Observable<any> {
        let query = `query ($id: Int!) {
            task(id: $id) {
                taskId,
                name,
                body,
                createdDate,
                deadline,
                author {
                    id,
                    lastName,
                    firstName,
                    patronymic
                },
                isExecution,
                startExecution,
                endExecution,
                finalPerformerId,
                finalPerformer {
                    id,
                    lastName,
                    firstName,
                    patronymic
                },
                totalTime,
                isImportant,
                coExecutors {
                    user {
                        id,
                        lastName,
                        firstName,
                        patronymic
                    }
                },
                observers {
                    user {
                        id,
                        lastName,
                        firstName,
                        patronymic
                    }
                },
                responsiblesForExecution {
                    user {
                        id,
                        lastName,
                        firstName,
                        patronymic
                    }
                },
                taskTags {
                    tag {
                        name
                    }
                }
            },
            taskAttachedFiles(taskId: $id) {
                taskAttachedFileId,
                attachedFileId,
                taskId,
                attachedFile {
                  attachedFileId,
                  name,
                  path
                }
              }
          }`;

        let variables = {
            "id": taskId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    removeTask(taskId: number): Observable<any> {
        let query = `mutation($id: Int!) {
          removeTask(taskId: $id) {
            status,
            message
          }
        }`;

        let variables = {
            "id": taskId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    loadAllTasksAsResponsible(userId: string, companyId: Number, taskType: string): Observable<any> {
        let query = `query GetAllTask($userId: String!, $companyId: Int!, $taskType: TaskType!) {
            tasks(taskType: $taskType, userId: $userId, companyId: $companyId) {
              taskId,
              name,
              createdDate,
              deadline,
              author {
                lastName,
                firstName,
                patronymic
              },
              isExecution
            }
          }`;

        let variables = {
            "userId": userId,
            "companyId": companyId,
            "taskType": taskType
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    checkExecution(taskId: number, status: boolean): Observable<any> {
        let query = `mutation ($taskId: Int!, $status: Boolean!) {
            checkExecution(taskId: $taskId, status: $status) {
              isExecution,
              status,
              totalTime
            }
          }`;

        let variables = {
            "taskId": taskId,
            "status": status
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    markTaskCompletion(taskId: Number, userId: string): Observable<any> {
        let query = `mutation ($taskId: Int!, $userId: String!) {
            markTaskCompletion(taskId: $taskId, userId: $userId) 
          }`;

        let variables = {
            "taskId": taskId,
            "userId": userId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    getFilesList(path: string, companyId: Number, type: number): Observable<any> {
        let headers = new HttpHeaders().set('Content-Type', 'application/json');
        headers = headers.set('authorization', this.dataService.getFullToken());

        let params = new HttpParams().set('path', path)
            .set('companyId', companyId.toString())
            .set('type', type.toString());

        return this.http.get(`${Utils.API_BASE_URL}/v1/Cloud/GetFilesList`,
            { headers: headers, params: params });
    }

    getFilesListPromise(path: string, companyId: Number, type: number): Promise<any> {
        let headers = new HttpHeaders().set('Content-Type', 'application/json');
        headers = headers.set('authorization', this.dataService.getFullToken());

        let params = new HttpParams().set('path', path)
            .set('companyId', companyId.toString())
            .set('type', type.toString());

        return this.http.get(`${Utils.API_BASE_URL}/v1/Cloud/GetFilesList`,
            { headers: headers, params: params }).toPromise();
    }

    addTaskAttachedFile(file: any, taskId: number, companyId: number): Promise<any> {

        const uploadData = new FormData();
        uploadData.append('file', file);

        let headers = new HttpHeaders()
            .set('authorization', this.dataService.getFullToken())
            .set('Access-Control-Allow-Origin', 'http://localhost:4200');

        let params = new HttpParams()
            .set('taskId', taskId.toString())
            .set('companyId', companyId.toString());

        return this.http.post<boolean>(`${Utils.API_BASE_URL}/v1/Cloud/UpdloadTaskAttachedFiles`, uploadData,
            { headers: headers, params: params }).toPromise();
    }

    createNewFolder(folderName: string, currentPath: string, companyId: Number): Observable<any> {
        let headers = new HttpHeaders().set('Content-Type', 'application/json');
        headers = headers.set('authorization', this.dataService.getFullToken());

        let params = new HttpParams().set('companyId', companyId.toString())
            .set('path', encodeURI(currentPath))
            .set('nameOfNewFolder', encodeURI(folderName));

        return this.http.post(`${Utils.API_BASE_URL}/v1/Cloud/CreateFolder`,
            null, { headers: headers, params: params });
    }

    deleteFiles(files: Array<string>, companyId: Number): Observable<any> {
        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('Accept', 'application/json');
        headers = headers.set('authorization', this.dataService.getFullToken());

        let data = {
            "paths": files,
            "companyId": companyId
        };

        return this.http.post(`${Utils.API_BASE_URL}/v1/Cloud/RemoveFiles`, data,
            { headers: headers });
    }

    renameFile(oldPath: string, newName: string, companyId: number): Observable<any> {
        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        let params = new HttpParams()
            .set('oldPath', encodeURI(oldPath))
            .set('newName', encodeURI(newName))
            .set('companyId', companyId.toString());

        return this.http.post(`${Utils.API_BASE_URL}/v1/Cloud/RenameFile`, null,
            { headers: headers, params: params });
    }

    uploadFile(file: any, path: string, companyId: number): Observable<any> {
        const uploadData = new FormData();
        uploadData.append('file', file);

        let params = new HttpParams()
            .set('path', encodeURI(path))
            .set('companyId', companyId.toString());

        let headers = new HttpHeaders()
            .set('authorization', this.dataService.getFullToken())
            .set('Access-Control-Allow-Origin', 'http://localhost:4200');

        return this.http.post(`${Utils.API_BASE_URL}/v1/Cloud/UploadFile`, uploadData,
            { headers: headers, params: params, reportProgress: true, observe: 'events' });
    }

    moveFile(companyId: Number, from: string, to: string): Observable<any> {

        let params = new HttpParams()
            .set('companyId', companyId.toString())
            .set('oldPath', encodeURI(from))
            .set('newPath', encodeURI(to));

        let headers = new HttpHeaders()
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.API_BASE_URL}/v1/Cloud/Move`, null,
            { headers: headers, params: params });
    }

    createNewChat(chat: any, companyId: number, userId: string): Observable<any> {
        let query = `mutation CreateNewChat($chat: ChatInput!, $companyId: Int!, $userId: String!) {
            createChat(chat: $chat, companyId: $companyId, userId: $userId) {
                chatId,
                name
              }
          }`;

        let variables = {
            "chat": chat,
            "companyId": companyId,
            "userId": userId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    loadAllChats(companyId: number, userId: string): Observable<any> {
        let query = `query GetAllChats($companyId: Int!, $userId: String!) {
            chats(companyId: $companyId, userId: $userId) {
              chatId,
              name,
              owner {
                id,
                lastName,
                firstName,
                patronymic
              }
            }
          }`;

        let variables = {
            "companyId": companyId,
            "userId": userId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    addTaskComment(text: string, taskId: number, userId: string): Observable<any> {
        let query = `mutation ($text: String!, $userId: String!, $taskId: Int!, $date: DateTime!) {
            createTaskComment(text: $text, userId: $userId, taskId: $taskId, date: $date) {
              taskCommentId,
                text,
                date,
                author {
                    id,
                    lastName,
                    firstName,
                    patronymic
                },
                isAccessOnDeleting
          }
        }`;

        let variables = {
            "text": text,
            "taskId": taskId,
            "userId": userId,
            "date": new Date()
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    loadAllTaskComments(taskId: number): Observable<any> {
        let query = `query GetAllTaskComment ($taskId: Int!) {
            comments (taskId: $taskId) {
                taskCommentId,
                text,
                date,
                author {
                    id,
                    lastName,
                    firstName,
                    patronymic
                },
                isAccessOnDeleting
              }
          }`;

        let variables = {
            "taskId": taskId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    deleteComment(commentId: number): Observable<any> {
        let query = `mutation DeleteComment($id: Int!) {
            deleteTaskComment(id: $id) {
                status,
                message,
                value
            }
          }`;

        let variables = {
            "id": commentId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    downloadFile(path: string, companyId: number): Observable<any> {
        let headers = new HttpHeaders()
            .set('authorization', this.dataService.getFullToken())
            .set('Accept', '*/*');

        let params = new HttpParams()
            .set('filePath', path)
            .set('companyId', companyId.toString());

        return this.http.get(`${Utils.API_BASE_URL}/v1/Cloud/DownloadFile`,
            { headers: headers, params: params, responseType: 'blob' });
    }

    downloadFiles(files: FileModel[], companyId: number): Observable<any> {
        let headers = new HttpHeaders()
            .set('authorization', this.dataService.getFullToken())
            .set('Accept', '*/*');

        let paths = [];
        files.forEach((value) => {
            paths.push(encodeURI(value.path));
        });

        let data = {
            "filesPaths": paths,
            "companyId": companyId
        };

        return this.http.post(`${Utils.API_BASE_URL}/v1/Cloud/DownloadFiles`, data,
            { headers: headers, responseType: 'blob' });
    }

    createFilePublickLink(filePath: string, companyId: number): Observable<any> {
        let params = new HttpParams()
            .set('companyId', companyId.toString())
            .set('filePath', encodeURI(filePath));

        let headers = new HttpHeaders()
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.API_BASE_URL}/v1/Cloud/CreatePublicLink`, null,
            { headers: headers, params: params });
    }

    removeFilePublicLink(link: string): Observable<any> {
        let params = new HttpParams()
            .set('link', link);

        let headers = new HttpHeaders()
            .set('authorization', this.dataService.getFullToken());

        return this.http.delete(`${Utils.API_BASE_URL}/v1/Cloud/RemoveLink`, { headers: headers, params: params });
    }

    loadAllCalendarEvent(userId: string) {
        let query = `query GetAllCalendarEvents($userId: String!) {
            calendarEvents(userId: $userId) {
              calendarEventId,
              text,
              start,
              end
            }
          }`;

        let variables = {
            "userId": userId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    addCalendarEvent(title: string, startDate: string, endDate: string, userId: string): Observable<any> {
        let query = `mutation CreateCalendarEvent($calendarEvent: CalendarEventInput!) {
            createCalendarEvent(calendarEvent: $calendarEvent) {
              calendarEventId,
              text,
              start,
              end
            }
          }`;

        let variables = {
            "calendarEvent": {
                "text": title,
                "authorId": userId,
                "start": startDate,
                "end": endDate
            }
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    updateCalendarEvent(title: string, startDate: string, endDate: string, id: number): Observable<any> {
        let query = `mutation UpdateCalendarEvent($calendarEvent: CalendarEventInput!) {
            updateCalendarEvent(calendarEvent: $calendarEvent) {
              calendarEventId,
              text,
              start,
              end
            }
          }`;

        let variables = {
            "calendarEvent": {
                "calendarEventId": id,
                "text": title,
                "start": startDate,
                "end": endDate
            }
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    removeCalendarEvent(id: number) {
        let query = `mutation DeleteCalendarEvent($calendarEventId: Int!) {
            deleteCalendarEvent(calendarEventId: $calendarEventId)
          }`;

        let variables = {
            "calendarEventId": id
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    loadAllUserShortLinks(userId: string): Observable<any> {
        let query = `query GetAllUserShortLinks($userId: String!) {
            userShortLinks(userId: $userId) {
                shortLinkId,
                full,
                short
            }
        }`;

        let variables = {
            "userId": userId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    loadAllCompanyShortLinks(companyId: number): Observable<any> {
        let query = `query GetAllCompanyShortLinks($companyId: Int!) {
            companyShortLinks(companyId: $companyId) {
                shortLinkId,
                full,
                short
            }
          }`;

        let variables = {
            "companyId": companyId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    createShortLink(variables: any): Observable<any> {
        let query = `mutation CreateShortLink($shortLink: ShortLinkInput!) {
            createShortLink(shortLink: $shortLink) {
              shortLinkId,
              full,
              short,
              ownerId,
              companyId
            }
          }`;

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    removeShortLink(id: number): Observable<any> {
        let query = `mutation DeleteShortLink($shortLinkId: Int!) {
            deleteShortLink(shortLinkId: $shortLinkId) {
                status,
                message,
                value
            }
          }`;

        let variables = {
            "shortLinkId": id
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    downloaAttachedFile(id: number): Observable<any> {
        let headers = new HttpHeaders()
            .set('authorization', this.dataService.getFullToken())
            .set('Accept', '*/*');

        let params = new HttpParams()
            .set('id', id.toString());

        return this.http.get(`${Utils.BASE_URL}/attach`,
            { headers: headers, params: params, responseType: 'blob' });
    }

    getCloudSize(companyId: number): Observable<any> {
        let headers = new HttpHeaders()
            .set('authorization', this.dataService.getFullToken());

        let params = new HttpParams()
            .set('companyId', companyId.toString());

        return this.http.get(`${Utils.API_BASE_URL}/v1/Cloud/GetCloudSizeInfo`, { headers: headers, params: params });
    }

    getAllNotifications(companyId: number) {
        let query = `query GetNotification($companyId: Int!) {
            birthDayNotifications(companyId: $companyId) {
              id,
              lastName,
              firstName,
              patronymic
            },
            taskNotifications(companyId: $companyId) {
              author {
                id,
                lastName,
                firstName,
                patronymic
              },
              dateTime,
              task {
                taskId,
                name
              },
              type
            },
            companyNotifications (companyId: $companyId) {
              author {
                id,
                lastName,
                firstName,
                patronymic
              },
              body,
              dateTime,
              type,
              newEmployee {
                id,
                lastName,
                firstName,
                patronymic
              }
            }, 
            privateNotifications (companyId: $companyId) {
              privateNotificationId,
              body,
              dateTime,
              author {
                id,
                lastName,
                firstName,
                patronymic
              }
            }
          }`;

        let variables = {
            "companyId": companyId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    getAccessOnReopenTask(taskId: number): Observable<any> {
        let query = `query GetAccessOnReopenTask($taskId: Int!) {
            getAccessOnReopenTask(taskId: $taskId)
          }`;

        let variables = {
            "taskId": taskId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    reopenTask(taskId: number): Observable<any> {
        let query = `mutation ReopenTask($taskId: Int!) {
            reopenTask(taskId: $taskId)
          }`;

        let variables = {
            "taskId": taskId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    changeCalendarEventDataTime(calendarEventId: number, start: any, end: any): Promise<any> {
        let query = `mutation ChangeDateTimeCalendarEvent($calendarEventId: Int!, $startDateTime: DateTime!, $endDateTime: DateTime!) {
            changeDateTimeCalendarEvent(calendarEventId: $calendarEventId, startDateTime: $startDateTime, endDateTime: $endDateTime) 
          }`;

        let variables = {
            "calendarEventId": calendarEventId,
            "startDateTime": start,
            "endDateTime": end
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers }).toPromise();
    }

    createPrivateNotification(privateNotifacation: any, notificationEmployees: any): Observable<any> {
        let query = `mutation CreatePrivateNotification($privateNotification: PrivateNotificationInput!, $notificationEmployees: [PrivateNotificationEmployeeInput]!) {
            createPrivateNotification(privateNotification: $privateNotification, notificationEmployees: $notificationEmployees) {
              privateNotificationId,
              body,
              dateTime,
              author {
                id,
                lastName,
                firstName,
                patronymic
              }
            }
          }`;

        let variables = {
            "privateNotification": privateNotifacation,
            "notificationEmployees": notificationEmployees
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    updatePrivateNotification(privateNotifacation: any, notificationEmployees: any): Observable<any> {
        let query = `mutation UpdatePrivateNotification($privateNotification: PrivateNotificationInput!, $notificationEmployees: [PrivateNotificationEmployeeInput]!) {
            updatePrivateNotification(privateNotification: $privateNotification, notificationEmployees: $notificationEmployees) {
              privateNotificationId,
              body,
              dateTime,
              author {
                id,
                lastName,
                firstName,
                patronymic
              }
            }
          }`;

        let variables = {
            "privateNotification": privateNotifacation,
            "notificationEmployees": notificationEmployees
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    getPermissionOnPrivateNotification(notificationId: number): Promise<any> {
        let query = `query GetPermissionOnPrivateNotification($notificationId: Int!) {
            getPermissionOnPrivateNotification(notificationId: $notificationId)
          }`;

        let variables = {
            "notificationId": notificationId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers }).toPromise();
    }

    removePrivateNotification(id): Observable<any> {
        let query = `mutation DeletePrivateNotification($id: Int!) {
            deletePrivateNotification(id: $id)
          }`;

        let variables = {
            "id": id
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    loadNotificationEmployees(notificationId: number, companyId: number): Observable<any> {
        let query = `query GetPrivateNotificationEmployees($notificationId: Int!, $companyId: Int!) {
            privateNotificationEmployees(notificationId: $notificationId, companyId: $companyId) {
              employeeId
          }
        }`;

        let variables = {
            "notificationId": notificationId,
            "companyId": companyId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    loadPermissionAsOwner(companyId: number): Observable<any> {
        let query = `query GetPermissionAsOwner($companyId: Int!) {
            getPermissionAsOwner(companyId: $companyId)
        }`;

        let variables = {
            "companyId": companyId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }

    getStatistics(companyId: number): Observable<any> {
        let query = `query GetTaskStatistics($companyId: Int!) {
            getTaskStatistics(companyId: $companyId) {
                countTasksAsAuthor,
                countTasksAsCoExecutor,
                countTasksAsObserver,
                countTasksAsResponsible,
                countCompletedTasks,
                countOverdueTasks,
            
                countIssuedTasksPerMonth,
                countTasksCompletedPerMonth,
                countAllTasksCompletedPerMonth,
                countTasksOutstandingPerMonth
          }
        }`;

        let variables = {
            "companyId": companyId
        };

        let headers = new HttpHeaders()
            .set('Content-Type', 'application/json')
            .set('authorization', this.dataService.getFullToken());

        return this.http.post(`${Utils.BASE_URL}/graphql`, JSON.stringify({ query, variables: variables }), { headers: headers });
    }
}