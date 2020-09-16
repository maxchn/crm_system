import { GenderModel } from './genderModel';
import { DepartmentModel } from './departmentModel';
import { PositionModel } from './positionModel';
import { Phone } from './phone';

export class ProfileModel {
    id: string;
    firstName: string;
    lastName: string;
    patronymic: string;
    dayOfBirth: string;
    gender: GenderModel;
    department: DepartmentModel;
    position: PositionModel;
    email: string;
    phones: Array<Phone>;
}