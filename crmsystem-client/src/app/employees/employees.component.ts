import { Component, OnInit, ViewChild } from '@angular/core';
import { SelectionModel } from '@angular/cdk/collections';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatDialog } from '@angular/material/dialog';
import { EmployeeInvitationDialogComponent } from '../employee-invitation-dialog/employee-invitation-dialog.component';
import { HttpService } from '../services/http.service';
import { Utils } from '../core/utils';
import { Employee } from '../models/employeeModel';
import { MatPaginatorIntl } from '@angular/material';
import { MatPaginatorIntlCro } from '../custom/paginator';
import { DataService } from '../services/data.service';
import { EditEmployeeDialogComponent } from '../edit-employee-dialog/edit-employee-dialog.component';

@Component({
  selector: 'app-employees',
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.css'],
  providers: [HttpService, DataService, { provide: MatPaginatorIntl, useClass: MatPaginatorIntlCro }]
})

export class EmployeesComponent implements OnInit {
  displayedColumns: string[] = ['select', 'lastName', 'firstName', 'patronymic', 'department', 'position', 'email', 'actions'];
  dataSource = new MatTableDataSource<Employee>();
  selection = new SelectionModel<Employee>(true, []);
  resultsLength: number;
  allowPermission: boolean = false;

  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;

  constructor(public dialog: MatDialog,
    private httpService: HttpService,
    private dataService: DataService) { }

  ngOnInit() {
    this.dataSource.paginator = this.paginator;

    let id = this.dataService.getCompanyId();
    this.httpService.loadEmployees(id).subscribe(data => {
      this.resultsLength = data["data"].employees.length;
      this.dataSource.data = data["data"].employees;
    },
      error => {
        Utils.printValueWithHeaderToConsole("Error", error);
      });

    this.httpService.loadPermissionAsOwner(this.dataService.getCompanyId()).subscribe(data => {
      this.allowPermission = data["data"].getPermissionAsOwner;
    }, error => {
      Utils.printValueWithHeaderToConsole('[loadPermissionAsOwner][error]', error);
    });
  }

  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  checkboxLabel(row?: Employee): string {
    if (!row) {
      return `${this.isAllSelected() ? 'select' : 'deselect'} all`;
    }
    return `${this.selection.isSelected(row) ? 'deselect' : 'select'} row ${row.id + 1}`;
  }

  openEmployeeInvitationDialog(): void {
    const dialogRef = this.dialog.open(EmployeeInvitationDialogComponent, {
      width: '550px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result != null) {
        let data = this.dataSource.data;
        data.push(result);
        this.dataSource.data = data;
      }
    });
  }

  generateDetailsLink(employee: Employee): string {
    return `/employee/details/${employee.id}`;
  }

  showEditEmployeeDialog(id) {
    const dialogRef = this.dialog.open(EditEmployeeDialogComponent, {
      width: '500px',
      data: id
    });

    dialogRef.afterClosed().subscribe(result => {
      Utils.printValueWithHeaderToConsole("[showEditEmployeeDialog][result]", result);
    });
  }
}