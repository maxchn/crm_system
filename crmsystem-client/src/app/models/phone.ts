export class Phone {
    phoneId: number;
    phoneNumber: string;
    ownerId: string;

    constructor(phoneNumber: string, ownerId: string) {
        this.phoneNumber = phoneNumber;
        this.ownerId = ownerId;
    }
}