import { Component, Inject } from '@angular/core';
import { HttpService } from '../services/http.service';
import { Utils } from '../core/utils';
import { FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { UploadFileModel } from '../models/uploadFileModel';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpEventType } from '@angular/common/http';
import { DataService } from '../services/data.service';

@Component({
  selector: 'app-upload-files-dialog',
  templateUrl: './upload-files-dialog.component.html',
  styleUrls: ['./upload-files-dialog.component.css'],
  providers: [HttpService, DataService]
})
export class UploadFilesDialogComponent {
  selectedFiles: Array<UploadFileModel> = [];
  form: FormGroup;
  currentPath: string;
  uploadedFiles: Array<any> = [];
  cloudSizeInfo = null;

  constructor(public dialogRef: MatDialogRef<UploadFilesDialogComponent>,
    private _httpService: HttpService,
    private _dataService: DataService,
    private _snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: any) {

    this.currentPath = data.path;
    this.cloudSizeInfo = data.cloudSizeInfo;

    if (data.files != null) {
      for (let i = 0; i < data.files.length; i++) {
        this.selectedFiles.push({ file: data.files[i], progress: 0, isShowActions: true });
      }
    }
  }

  preview(event) {
    if (event.target.files.length === 0)
      return;

    for (let i = 0; i < event.target.files.length; i++) {
      this.selectedFiles.push({ file: event.target.files[i], progress: 0, isShowActions: true });
    }
  }

  uploadFile(file: UploadFileModel) {

    if (this.cloudSizeInfo.size + file.file.size > this.cloudSizeInfo.maxSize) {
      this._snackBar.open(`Файл не может быть загружен так как его размер превышает объем свободного места на облачном хранилище!!! Удалите менее важные документы и повторите попытку`, null, {
        duration: 7000,
      });
      return;
    }

    this._httpService.uploadFile(file.file, this.currentPath, this._dataService.getCompanyId())
      .subscribe(data => {

        if (data.type === HttpEventType.UploadProgress) {
          file.progress = Math.round(100 * data.loaded / data.total);
        }
        else if (data.type === HttpEventType.Response) {
          if (data.status == 200 && data.body.status == true) {
            this.cloudSizeInfo.size += file.file.size;
            file.isShowActions = false;
            this.uploadedFiles.push(data.body);
          }
          else {
            this._snackBar.open(`Файл ${file.file.name} не был загружен!!! Подробнее: ${data.body.message}`, null, {
              duration: 3000,
            });
          }
        }
      }, error => {
        Utils.printValueWithHeaderToConsole("Error", error);

        this._snackBar.open(`При загрузке файла ${file.file.name} произошла ошибка!!!`, null, {
          duration: 3000,
        });
      });
  }

  removeFile(file: UploadFileModel) {
    let index = this.selectedFiles.indexOf(file);

    if (index >= 0) {
      this.selectedFiles.splice(index, 1);
    }
  }

  onCloseClick(): void {
    this.dialogRef.close(this.uploadedFiles);
  }

  calculateFileSize(fileSize): string {

    let units = [
      "Байт",
      "КБ",
      "МБ",
      "ГБ",
      "ТБ",
      "ПБ",
      "ЭБ",
      "ЗБ",
      "ИБ",
    ];
    let index = 0;

    while (fileSize > 1024) {
      fileSize = fileSize / 1024;
      index++;
    }

    return `${fileSize.toFixed(2)} ${units[index]}`;
  }
}
