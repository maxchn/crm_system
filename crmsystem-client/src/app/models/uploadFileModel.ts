export class UploadFileModel {
    file: any;
    progress: number = 0;
    isShowActions: boolean = true;

    constructor(file: any) {
        this.file = file;
    }
}