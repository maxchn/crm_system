import { Employee } from './employeeModel';
import { CompanyModel } from './companyModel';

export class ShortLink {
    shortLinkId: number;
    short: string;
    full: string;
    owner: Employee;
    ownerId: string;
    company: CompanyModel;
    companyId: number;
}