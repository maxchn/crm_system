import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Utils } from '../core/utils';
import { Employee } from '../models/employeeModel';
import { HttpService } from '../services/http.service';

@Component({
  selector: 'app-employee-details',
  templateUrl: './employee-details.component.html',
  styleUrls: ['./employee-details.component.css'],
  providers: [HttpService]
})

export class EmployeeDetailsComponent implements OnInit {
  id: string;
  model: Employee = new Employee();
  imgURL: any = "assets/images/default-avatar.png";

  constructor(private _activateRoute: ActivatedRoute,
    private httpService: HttpService) {

    this.id = _activateRoute.snapshot.params['id'];
  }

  ngOnInit() {
    this.httpService.loadEmployeeInfo(this.id).subscribe(data => {
      this.model = data["data"].user;
    },
      error => {
        Utils.printValueWithHeaderToConsole("[loadEmployeeInfo][error]", error);
      });

    this.httpService.downloadAvatar(this.id).subscribe(data => {
      let reader = new FileReader();
      reader.addEventListener("load", () => {
        this.imgURL = reader.result;
      }, false);

      if (data) {
        reader.readAsDataURL(data);
      }
    },
      error => {
        Utils.printValueWithHeaderToConsole("[downloadAvatar][error]", error);
      });
  }

  formatDate(date: string): string {
    return Utils.formatDate(date);
  }
}