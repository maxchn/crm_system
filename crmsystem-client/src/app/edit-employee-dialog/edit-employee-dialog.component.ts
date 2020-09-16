import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Utils } from '../core/utils';

@Component({
  selector: 'app-edit-employee-dialog',
  templateUrl: './edit-employee-dialog.component.html',
  styleUrls: ['./edit-employee-dialog.component.css']
})
export class EditEmployeeDialogComponent implements OnInit {

  employeeId: string = null;
  isChangeProfile: boolean = false;

  constructor(public dialogRef: MatDialogRef<EditEmployeeDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: string) {

    this.employeeId = data;
  }

  ngOnInit() {

  }

  onCloseClick(): void {
    this.dialogRef.close(this.isChangeProfile);
  }

  changeProfile(isSuccessfully: boolean) {
    this.isChangeProfile = isSuccessfully;

    if (this.isChangeProfile) {
      this.dialogRef.close(this.isChangeProfile);
    }
  }
}