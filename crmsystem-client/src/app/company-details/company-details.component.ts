import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { EditCompanyDataDialogComponent } from '../edit-company-data-dialog/edit-company-data-dialog.component';
import { HttpService } from '../services/http.service';
import { Utils } from '../core/utils';
import { CompanyModel } from '../models/companyModel';
import { DataService } from '../services/data.service';

@Component({
  selector: 'app-company-details',
  templateUrl: './company-details.component.html',
  styleUrls: ['./company-details.component.css'],
  providers: [HttpService]
})

export class CompanyDetailsComponent implements OnInit {
  model: CompanyModel = new CompanyModel();
  isEnableEditCompanyButton: boolean = false;

  constructor(public dialog: MatDialog,
    private _dataService: DataService,
    private _httpService: HttpService) { }

  ngOnInit() {
    this._httpService.loadCompanyInfo(this._dataService.getCompanyId()).subscribe(data => {
      this.model.id = data['data'].company.companyId;
      this.model.name = data['data'].company.name;
      this.model.url = data['data'].company.urlName;
      this.model.owner = `${data['data'].company.owner.lastName} ${data['data'].company.owner.firstName} ${data['data'].company.owner.patronymic}`;
    },
      error => {
        Utils.printValueWithHeaderToConsole("[loadCompanyInfo][error]", error);
      });

    this._httpService.loadCompanyPermission(this._dataService.getCompanyId()).subscribe(data => {
      this.isEnableEditCompanyButton = data["data"].getPermissionOnUpdatingCompanyData;
    }, error => {
      Utils.printValueWithHeaderToConsole("[loadCompanyPermission][error]", error);
    });
  }

  openEditCompanyDialog(): void {
    const dialogRef = this.dialog.open(EditCompanyDataDialogComponent, {
      width: '500px',
      data: { id: this.model.id, name: this.model.name, url: this.model.url }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (result.status) {
          this.model.name = result.name;
          this.model.url = result.url;
        }
      }
    });
  }
}