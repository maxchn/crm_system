import { Employee } from './employeeModel';
import { CompanyModel } from './companyModel';

export class Chat {
    chatId: number;
    name: string;
    owner: Employee;
    company: CompanyModel;
    isSelected: boolean = false;
}