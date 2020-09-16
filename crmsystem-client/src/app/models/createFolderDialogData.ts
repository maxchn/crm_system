import { FileModel } from './fileModel';

export class CreateFolderDialogData {
    currentPath: string;
    companyId: number;
    files: Array<FileModel>;
}