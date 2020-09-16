import { Component, OnInit, Inject } from '@angular/core';
import { HttpService } from '../services/http.service';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { RenameFileDialogData } from '../models/renameFileDialogData';
import { Utils } from '../core/utils';
import { FileModel } from '../models/fileModel';

@Component({
  selector: 'app-rename-file-dialog',
  templateUrl: './rename-file-dialog.component.html',
  styleUrls: ['./rename-file-dialog.component.css'],
  providers: [HttpService]
})

export class RenameFileDialogComponent implements OnInit {
  oldName: string;
  oldPath: string;
  companyId: number;
  renameFileForm: FormGroup;
  fileNameControl: FormControl = new FormControl(this.oldName,
    [
      Validators.required,
      Validators.maxLength(120),
      Validators.pattern('[^/\:\*?"<>\|]+')
    ]);
  files: Array<FileModel> = [];

  constructor(fb: FormBuilder,
    public dialogRef: MatDialogRef<RenameFileDialogComponent>,
    private httpService: HttpService,
    @Inject(MAT_DIALOG_DATA) public data: RenameFileDialogData) {

    this.oldName = data.oldName;
    this.oldPath = data.oldPath;
    this.companyId = data.companyId;
    this.files = data.files;

    this.renameFileForm = fb.group({
      hideRequired: false,
      floatLabel: 'auto',
    });
  }

  ngOnInit() {
    this.renameFileForm = new FormGroup({
      fileName: this.fileNameControl
    });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.renameFileForm.controls[controlName].hasError(errorName);
  }

  renameFile(): void {

    if (this.renameFileForm.valid) {
      let newName = this.renameFileForm.value.fileName;

      // Проверка на дубликат
      let index = this.files.findIndex(f => f.name.toLowerCase() === newName.toLowerCase());

      this.fileNameControl.setErrors({ dublicate: index >= 0 ? true : false });
      if (index >= 0)
        return;

      this.httpService.renameFile(this.oldPath, newName, this.companyId).subscribe(data => {
        this.dialogRef.close(data);
      },
        error => {
          Utils.printValueWithHeaderToConsole("Error", error);
        });
    }
  };

  onCancelClick(): void {
    this.dialogRef.close();
  }
}
