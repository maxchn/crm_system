import { Employee } from './employeeModel';
import { CompanyModel } from './companyModel';
import { TaskEmployee } from './taskEmployeeModel';
import { TaskTag } from './taskTag';

export class Task {
    taskId: number;
    name: string;
    body: string;
    deadline: string;
    isImportant: boolean;
    responsibleForExecution: Array<TaskEmployee> = [];
    coExecutors: Array<TaskEmployee> = [];
    observers: Array<TaskEmployee> = [];
    tags: Array<TaskTag> = [];
    attachedFiles: Array<any> = [];
    author: Employee;
    company: CompanyModel;
    isExecution: boolean;
    finalPerformerId: string;
    createdDate: any;
    totalTime: any;
    responsiblesForExecution: any;
    taskTags: Array<any> = [];
}