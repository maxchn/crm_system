import { Component, OnInit, Inject } from '@angular/core';
import { HttpService } from '../services/http.service';
import { Utils } from '../core/utils';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { CreateFolderDialogData } from '../models/createFolderDialogData';
import { FileModel } from '../models/fileModel';

@Component({
  selector: 'app-create-folder-dialog',
  templateUrl: './create-folder-dialog.component.html',
  styleUrls: ['./create-folder-dialog.component.css'],
  providers: [HttpService]
})

export class CreateFolderDialogComponent implements OnInit {
  currentPath: string;
  companyId: number;
  createFolderForm: FormGroup;
  folderNameControl: FormControl = new FormControl('',
    [
      Validators.required,
      Validators.maxLength(120),
      Validators.pattern('[^/\:\*?"<>\|]+')
    ]);
  files: Array<FileModel> = [];

  constructor(fb: FormBuilder,
    public dialogRef: MatDialogRef<CreateFolderDialogComponent>,
    private httpService: HttpService,
    @Inject(MAT_DIALOG_DATA) public data: CreateFolderDialogData) {

    this.currentPath = data.currentPath;
    this.companyId = data.companyId;
    this.files = data.files;

    this.createFolderForm = fb.group({
      hideRequired: false,
      floatLabel: 'auto',
    });
  }

  ngOnInit() {
    this.createFolderForm = new FormGroup({
      folderName: this.folderNameControl
    });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.createFolderForm.controls[controlName].hasError(errorName);
  }

  createFolder(): void {

    if (this.createFolderForm.valid) {
      let folderName = this.createFolderForm.value.folderName;

      // Проверка на дубликат
      let index = this.files.findIndex(f =>
        f.name.toLowerCase() === folderName.toLowerCase()
      );

      this.folderNameControl.setErrors({ dublicate: index >= 0 ? true : false });
      if (index >= 0)
        return;

      this.httpService.createNewFolder(folderName, this.currentPath, this.companyId).subscribe(data => {
        let result = {
          status: true,
          file: data
        };

        this.dialogRef.close(result);
      },
        error => {
          Utils.printValueWithHeaderToConsole("Error", error);
        });
    }
  };

  onCloseClick(): void {
    this.dialogRef.close();
  }
}
