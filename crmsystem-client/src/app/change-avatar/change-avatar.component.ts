import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpService } from '../services/http.service';
import { Utils } from '../core/utils';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { DataService } from '../services/data.service';

@Component({
  selector: 'app-change-avatar',
  templateUrl: './change-avatar.component.html',
  styleUrls: ['./change-avatar.component.css'],
  providers: [HttpService, DataService]
})

export class ChangeAvatarComponent implements OnInit {
  public imagePath;
  imgURL: any = "assets/images/default-avatar.png";
  form: FormGroup;
  file: any;
  public errorMessage: string;
  progressValue: number;

  constructor(private _snackBar: MatSnackBar,
    private httpService: HttpService,
    private dataService: DataService,
    private fb: FormBuilder) {

    this.form = fb.group({
      avatar: [],
      hideRequired: false,
      floatLabel: 'auto'
    });
  }

  ngOnInit() {
    let id: string = this.dataService.getUserId();

    this.httpService.downloadAvatar(id).subscribe(data => {
      let reader = new FileReader();
      reader.addEventListener("load", () => {
        this.imgURL = reader.result;
      }, false);

      if (data) {
        reader.readAsDataURL(data);
      }
    },
      error => {
        
      });
  }

  preview(event) {

    if (event.target.files.length === 0)
      return;

    const file = event.target.files[0];
    this.file = file;

    var mimeType = event.target.files[0].type;
    if (mimeType.match(/image\/*/) == null) {
      this.errorMessage = 'Поддерживаются только изображения!!!';
      return;
    }

    this.errorMessage = '';

    var reader = new FileReader();
    this.imagePath = event.target.files[0];
    reader.readAsDataURL(event.target.files[0]);

    reader.onload = (_event) => {
      this.imgURL = reader.result;

      this._snackBar.open("Предзагрузка аватара прошла успешно", null, {
        duration: 3000,
      });
    }
  }

  public changeAvatar = () => {
    this.httpService.uploadAvatar(this.file)
      .subscribe(data => {
        if (data && data.status === 'progress')
          this.progressValue = Number(data.message);

        if (!data) {
          this._snackBar.open("Аватар был успешно обновлен", null, {
            duration: 3000,
          });
        }
      },
        error => {
          Utils.printValueWithHeaderToConsole("Error", error);

          this._snackBar.open(`При обновлении аватара произошла ошибка!!! Подробнее: ${error.error}`, null, {
            duration: 3000,
          });
        });
  }
}
