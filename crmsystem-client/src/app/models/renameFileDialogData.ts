import { FileModel } from './fileModel';

export class RenameFileDialogData {
    oldName: string;
    oldPath: string;
    companyId: number;
    files: Array<FileModel>;
}