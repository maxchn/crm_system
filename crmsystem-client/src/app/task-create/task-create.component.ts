import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { HttpService } from '../services/http.service';
import { ENTER } from '@angular/cdk/keycodes';
import { MatAutocompleteSelectedEvent, MatAutocomplete } from '@angular/material/autocomplete';
import { MatChipInputEvent } from '@angular/material/chips';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { DateAdapter } from '@angular/material/core';
import { Utils } from '../core/utils';
import { Employee } from '../models/employeeModel';
import { Tag } from '../models/tagModel';
import { Task } from '../models/taskModel';
import { TaskEmployee } from '../models/taskEmployeeModel';
import { CompanyModel } from '../models/companyModel';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, ActivatedRoute } from '@angular/router';
import { EmployeeTypeFilter } from '../models/employeeTypeFilter';
import { DataService } from '../services/data.service';
import { AttachedFile } from '../models/attachedFile';
import { FlatTreeControl } from '@angular/cdk/tree';
import { DynamicFlatNode } from '../models/dynamicFlatNode';
import { DynamicDataSource } from '../data-source/dynamicDataSource';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';

@Component({
  selector: 'app-task-create',
  templateUrl: './task-create.component.html',
  styleUrls: ['./task-create.component.css'],
  providers:
    [
      HttpService,
      DataService
    ]
})
export class TaskCreateComponent implements OnInit {
  form: FormGroup;
  id: number = 0;

  minDate = new Date();
  maxDate = new Date(2020, 0, 1);

  actionButtonText: String = 'Поставить задачу';

  visible = true;
  selectable = true;
  removable = true;
  addOnBlur = true;
  separatorKeysCodes: number[] = [ENTER];

  responsibleForExecutionCtrl = new FormControl();
  coExecutorCtrl = new FormControl();
  observerCtrl = new FormControl();
  tagCtrl = new FormControl();

  filteredResponsibleForExecution: Observable<Employee[]>;
  filteredCoExecutor: Observable<Employee[]>;
  filteredObserver: Observable<Employee[]>;
  filteredTags: Observable<Tag[]>;

  selectedResponsiblesArray: Employee[] = [];
  selectedCoExecutorArray: Employee[] = [];
  selectedObserverArray: Employee[] = [];

  allTags: Tag[] = [];
  allEmployees: Employee[] = [];

  attachedFiles: AttachedFile[] = [];
  selectedFilesFromCloud: DynamicFlatNode[] = [];

  @ViewChild('bodyEditor', null) bodyEditor: any;
  public Editor = ClassicEditor;
  taskBody: string = "";
  isEmptyBody: boolean = false;

  treeControl: FlatTreeControl<DynamicFlatNode>;
  dataSource: DynamicDataSource;
  getLevel = (node: DynamicFlatNode) => {
    let width = (node.level * this.threePaddingIndent) + 200;
    this.treeWidth = `${width}px`;
    return node.level;
  };
  isExpandable = (node: DynamicFlatNode) => node.expandable;
  hasChild = (_: number, _nodeData: DynamicFlatNode) => _nodeData.expandable;

  treeWidth: string;
  threePaddingIndent: number = 15;

  @ViewChild('responsibleForExecutionInput', null) responsibleForExecutionInput: ElementRef<HTMLInputElement>;
  @ViewChild('coExecutorInput', null) coExecutorInput: ElementRef<HTMLInputElement>;
  @ViewChild('observerInput', null) observerInput: ElementRef<HTMLInputElement>;
  @ViewChild('tagInput', null) tagInput: ElementRef<HTMLInputElement>;

  @ViewChild('auto1', null) r_matAutocomplete: MatAutocomplete;
  @ViewChild('auto2', null) c_matAutocomplete: MatAutocomplete;
  @ViewChild('auto3', null) o_matAutocomplete: MatAutocomplete;

  deadLineControl: FormControl = new FormControl('',
    [
      this.dateIsNotEmpty
    ]);

  cloudSizeInfo = {
    size: 0,
    maxSize: 0
  };

  constructor(fb: FormBuilder,
    private _httpService: HttpService,
    private _dataService: DataService,
    private _adapter: DateAdapter<any>,
    private _snackBar: MatSnackBar,
    private _router: Router,
    private _activateRoute: ActivatedRoute) {

    this.id = _activateRoute.snapshot.params['id'];

    if (this.id)
      this.actionButtonText = 'Сохранить изменения';

    this.form = fb.group({
      hideRequired: false,
      floatLabel: 'auto',
    });

    this._adapter.setLocale('ru');

    this.filteredResponsibleForExecution = this.responsibleForExecutionCtrl.valueChanges.pipe(
      startWith(null),
      map((employee: string | null) => employee ? this._filter(employee, EmployeeTypeFilter.Responsible) : []));

    this.filteredCoExecutor = this.coExecutorCtrl.valueChanges.pipe(
      startWith(null),
      map((employee: string | null) => employee ? this._filter(employee, EmployeeTypeFilter.CoExecutor) : []));

    this.filteredObserver = this.observerCtrl.valueChanges.pipe(
      startWith(null),
      map((employee: string | null) => employee ? this._filter(employee, EmployeeTypeFilter.Observer) : []));

    this.treeControl = new FlatTreeControl<DynamicFlatNode>(this.getLevel, this.isExpandable);
    this.dataSource = new DynamicDataSource(this.treeControl, this._httpService, this._dataService);
    this.dataSource.fileType = 3;
  }

  ngOnInit() {
    this.dataSource.data = [new DynamicFlatNode("\\", 0, true, false, "\\")];

    let id = this._dataService.getCompanyId();
    this._httpService.loadEmployees(id).subscribe(data => {
      this.allEmployees = data["data"].employees;
    },
      error => {
        Utils.printValueWithHeaderToConsole("Error", error);
      });

    this.form = new FormGroup({
      name: new FormControl('',
        [
          Validators.required,
          Validators.maxLength(120)
        ]),
      deadlineDate: this.deadLineControl,
      isImportant: new FormControl(''),
      responsibleForExecution: this.responsibleForExecutionCtrl,
      coExecutor: this.coExecutorCtrl,
      observer: this.observerCtrl,
      tag: this.tagCtrl,
    });

    if (this.id) {
      this._httpService.getTaskDetailsInfoById(this.id).subscribe(data => {

        let detailsData = data["data"].task;

        this.form.patchValue({
          name: detailsData.name,
          isImportant: detailsData.isImportant
        });

        this.bodyEditor.editorInstance.setData(detailsData.body);

        this.deadLineControl.patchValue(new Date(detailsData.deadline));

        for (let i = 0; i < detailsData.responsiblesForExecution.length; i++) {
          this.selectedResponsiblesArray.push(detailsData.responsiblesForExecution[i].user);
        }

        for (let i = 0; i < detailsData.coExecutors.length; i++) {
          this.selectedCoExecutorArray.push(detailsData.coExecutors[i].user);
        }

        for (let i = 0; i < detailsData.observers.length; i++) {
          this.selectedObserverArray.push(detailsData.observers[i].user);
        }

        for (let i = 0; i < detailsData.taskTags.length; i++) {
          this.allTags.push(detailsData.taskTags[i].tag);
        }

        this.selectedCoExecutor = detailsData.coExecutors;

        let attachedFiles = data["data"].taskAttachedFiles;
        for (let i = 0; i < attachedFiles.length; i++) {
          let attachedFile = new AttachedFile();
          attachedFile.taskAttachedFileId = attachedFiles[i].taskAttachedFileId;
          attachedFile.name = attachedFiles[i].attachedFile.name;
          attachedFile.path = attachedFiles[i].attachedFile.path;
          attachedFile.id = attachedFiles[i].attachedFile.attachedFileId;
          attachedFile.file = null;

          this.attachedFiles.push(attachedFile);
        }
      },
        error => {
          Utils.printValueWithHeaderToConsole("Error", error);
        });
    }

    this._httpService.getCloudSize(this._dataService.getCompanyId())
      .subscribe(data => {
        this.cloudSizeInfo = data;
      },
        error => {
          Utils.printValueWithHeaderToConsole("[getCloudSize][error]", error);
        });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.form.controls[controlName].hasError(errorName);
  }

  dateIsNotEmpty(control: FormControl) {
    let value = control.value;

    if (value) {
      return null;
    }

    return { "isEmpty": true };
  }

  responsibleForExecutionValidator() {
    return (control: AbstractControl): { [key: string]: any } | null => {
      return this.selectedResponsiblesArray.length > 0 ? null : { 'required': true };
    };
  }

  addResponsibleForExecution(event: MatChipInputEvent): void {
    if (!this.r_matAutocomplete.isOpen) {
      this.responsibleForExecutionCtrl.setValue(null);
    }
  }

  addCoExecutor(event: MatChipInputEvent): void {
    if (!this.c_matAutocomplete.isOpen) {
      this.coExecutorCtrl.setValue(null);
    }
  }

  addObserver(event: MatChipInputEvent): void {
    if (!this.o_matAutocomplete.isOpen) {
      this.observerCtrl.setValue(null);
    }
  }

  addTag(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    // Add our tag
    if ((value || '').trim()) {
      this.allTags.push({ tagId: 0, name: value.trim() });
    }

    // Reset the input value
    if (input) {
      input.value = '';
    }

    this.tagCtrl.setValue(null);
  }

  removeResponsibleForExecution(employee: Employee): void {
    const index = this.selectedResponsiblesArray.indexOf(employee);

    if (index >= 0) {
      this.selectedResponsiblesArray.splice(index, 1);
    }
  }

  removeCoExecutor(employee: Employee): void {
    const index = this.selectedCoExecutorArray.indexOf(employee);

    if (index >= 0) {
      this.selectedCoExecutorArray.splice(index, 1);
    }
  }

  removeObserver(employee: Employee): void {
    const index = this.selectedObserverArray.indexOf(employee);

    if (index >= 0) {
      this.selectedObserverArray.splice(index, 1);
    }
  }

  removeTag(tag: Tag): void {
    const index = this.allTags.indexOf(tag);

    if (index >= 0) {
      this.allTags.splice(index, 1);
    }
  }

  selectedResponsibleForExecution(event: MatAutocompleteSelectedEvent): void {
    this.selectedResponsiblesArray.push(this.allEmployees.find(x => x.id === event.option.value));
    this.responsibleForExecutionInput.nativeElement.value = '';
    this.responsibleForExecutionCtrl.setValue(null);
  }

  selectedCoExecutor(event: MatAutocompleteSelectedEvent): void {
    this.selectedCoExecutorArray.push(this.allEmployees.find(x => x.id === event.option.value));
    this.coExecutorInput.nativeElement.value = '';
    this.coExecutorCtrl.setValue(null);
  }

  selectedObserver(event: MatAutocompleteSelectedEvent): void {
    this.selectedObserverArray.push(this.allEmployees.find(x => x.id === event.option.value));
    this.observerInput.nativeElement.value = '';
    this.observerCtrl.setValue(null);
  }

  private _filter(value: string, type: EmployeeTypeFilter): Employee[] {
    if (!value.trim())
      return [];

    const filterValue = value.toLowerCase();

    let employees = this.allEmployees.filter(employee => employee.id.toLowerCase().indexOf(filterValue) === 0 ||
      `${employee.lastName.toLowerCase()} ${employee.firstName.toLowerCase()} ${employee.patronymic.toLowerCase()}`.toLowerCase().indexOf(filterValue) === 0);

    switch (+type) {
      case EmployeeTypeFilter.Observer:
        employees = employees.filter(employee => !this.selectedResponsiblesArray.find(e => e.id.indexOf(employee.id) === 0) &&
          !this.selectedCoExecutorArray.find(e => e.id.indexOf(employee.id) === 0) &&
          !this.selectedObserverArray.find(e => e.id.indexOf(employee.id) === 0));
        break;
      case EmployeeTypeFilter.Responsible:
        employees = employees.filter(employee => !this.selectedObserverArray.find(e => e.id.indexOf(employee.id) === 0) &&
          !this.selectedCoExecutorArray.find(e => e.id.indexOf(employee.id) === 0) &&
          !this.selectedResponsiblesArray.find(e => e.id.indexOf(employee.id) === 0));
        break;
      case EmployeeTypeFilter.CoExecutor:
        employees = employees.filter(employee => !this.selectedObserverArray.find(e => e.id.indexOf(employee.id) === 0) &&
          !this.selectedResponsiblesArray.find(e => e.id.indexOf(employee.id) === 0) &&
          !this.selectedCoExecutorArray.find(e => e.id.indexOf(employee.id) === 0));
        break;
    }

    return employees;
  }

  public changeTask = () => {
    if (this.bodyEditor.editorInstance.getData().length == 0) {
      this.isEmptyBody = true;

      return;
    }

    if (this.form.valid && this.selectedResponsiblesArray.length > 0) {

      let filesSize = 0;
      for (let i = 0; i < this.attachedFiles.length; i++) {
        if (this.attachedFiles[i].fromCloud == false && this.attachedFiles[i].file != null) {
          filesSize += this.attachedFiles[i].file.size;
        }
      }

      if (this.cloudSizeInfo.size + filesSize > this.cloudSizeInfo.maxSize) {
        this._snackBar.open(`Размер прикрепленных файлов (не учитываются файлы выбранные с облачного хранилища) превышает размер свободного места в облачном хранилище!!!`, null, {
          duration: 10000,
        });
        return;
      }

      let task = new Task();

      if (this.id)
        task.taskId = this.id;

      task.name = this.form.value.name;
      task.body = this.bodyEditor.editorInstance.getData();
      task.deadline = this.form.value.deadlineDate
      task.author = new Employee();
      task.author.id = this._dataService.getUserId();
      task.company = new CompanyModel();
      task.company.id = this._dataService.getCompanyId();
      task.isImportant = this.form.value.isImportant;

      let responsibleForExecutions = new Array<TaskEmployee>();
      for (let i = 0; i < this.selectedResponsiblesArray.length; i++) {
        responsibleForExecutions.push({ id: this.selectedResponsiblesArray[i].id, taskId: 0 });
      }
      task.responsibleForExecution = responsibleForExecutions;

      let coExecutors = new Array<TaskEmployee>();
      for (let i = 0; i < this.selectedCoExecutorArray.length; i++) {
        coExecutors.push({ id: this.selectedCoExecutorArray[i].id, taskId: 0 });
      }
      task.coExecutors = coExecutors;

      let observers = new Array<TaskEmployee>();
      for (let i = 0; i < this.selectedObserverArray.length; i++) {
        observers.push({ id: this.selectedObserverArray[i].id, taskId: 0 });
      }
      task.observers = observers;

      let taskTags = new Array<any>();
      for (let i = 0; i < this.allTags.length; i++) {
        taskTags.push({ tag: this.allTags[i] })
      }
      task.tags = taskTags;

      let attachedFiles = [];
      for (let i = 0; i < this.attachedFiles.length; i++) {
        if (this.attachedFiles[i].taskAttachedFileId != -1) {
          attachedFiles.push({
            "taskAttachedFileId": this.attachedFiles[i].taskAttachedFileId,
            "attachedFileId": this.attachedFiles[i].id,
            "taskId": 0,
            "attachedFile": {
              "attachedFileId": this.attachedFiles[i].id,
              "name": this.attachedFiles[i].name,
              "path": this.attachedFiles[i].path
            },
          });
        }
        else {
          if (this.attachedFiles[i].fromCloud === true) {
            attachedFiles.push({
              "taskAttachedFileId": 0,
              "attachedFileId": 0,
              "taskId": 0,
              "attachedFile": {
                "attachedFileId": 0,
                "name": this.attachedFiles[i].name,
                "path": this.attachedFiles[i].path
              },
            });
          }
        }
      }

      task.attachedFiles = attachedFiles;
      if (this.id) {
        this._httpService.updateTask(task).subscribe(async data => {
          if (data["data"].updateTask.status) {

            for (let i = 0; i < this.attachedFiles.length; i++) {
              if (this.attachedFiles[i].fromCloud == false && this.attachedFiles[i].file != null) {
                try {
                  await this._httpService.addTaskAttachedFile(this.attachedFiles[i].file,
                    this.id, this._dataService.getCompanyId());
                }
                catch (err) {
                  this._snackBar.open(`При загрузки ${this.attachedFiles[i].name} произошла ошибка!!!`, null, {
                    duration: 3000,
                  });
                }
              }
            }

            this._snackBar.open("Задача была успешно обновлена", null, {
              duration: 3000,
            });

            this._router.navigate(['/task/index']);
          }
        },
          error => {
            Utils.printValueWithHeaderToConsole("Error:", error);

            this._snackBar.open(`При обновлении задачи произошла ошибка!!! Подробнее: ${error.message}`, null, {
              duration: 3000,
            });
          }
        );
      }
      else {
        this._httpService.createNewTask(task, this._dataService.getCompanyId()).subscribe(async data => {
          if (data["data"].createTask.status) {
            if (this.attachedFiles.length > 0) {
              for (let i = 0; i < this.attachedFiles.length; i++) {
                if (this.attachedFiles[i].fromCloud == false && this.attachedFiles[i].file != null) {
                  try {
                    await this._httpService.addTaskAttachedFile(this.attachedFiles[i].file,
                      data["data"].createTask.value, this._dataService.getCompanyId());
                  }
                  catch (err) {
                    this._snackBar.open(`При загрузки ${this.attachedFiles[i].name} произошла ошибка!!!`, null, {
                      duration: 3000,
                    });
                  }
                }
              }

              this._snackBar.open("Задача была успешно создана", null, {
                duration: 3000,
              });

              this._router.navigate(['/task/index']);
            }
            else {
              this._router.navigate(['/task/index']);
            }
          }
        },
          error => {
            Utils.printValueWithHeaderToConsole("Error:", error);

            this._snackBar.open(`При создании задачи произошла ошибка!!! Подробнее: ${error.message}`, null, {
              duration: 3000,
            });
          }
        );
      }
    }
  }

  preview(event) {
    if (event.target.files.length === 0)
      return;

    for (let i = 0; i < event.target.files.length; i++) {
      let attachedFile = new AttachedFile();
      attachedFile.taskAttachedFileId = -1;
      attachedFile.name = event.target.files[i].name;
      attachedFile.file = event.target.files[i];
      attachedFile.fromCloud = false;
      attachedFile.progress = 0;

      this.attachedFiles.push(attachedFile);
    }
  }

  deleteAttachedFiles(selectedItems: Array<any>) {
    for (let i = 0; i < selectedItems.length; i++) {
      let index = this.attachedFiles.indexOf(selectedItems[i].value);

      if (index >= 0)
        this.attachedFiles.splice(index, 1);
    }
  }

  selectFileItemSelectionToggle(isChecked: boolean, file: DynamicFlatNode) {
    if (isChecked) {
      this.selectedFilesFromCloud.push(file);
    }
    else {
      let index = this.selectedFilesFromCloud.findIndex(f => f.path == file.path);

      if (index >= 0) {
        this.selectedFilesFromCloud.splice(index, 1);
      }
    }
  }

  addSelectedFilesFromCloud() {
    for (let i = 0; i < this.selectedFilesFromCloud.length; i++) {
      let index = this.attachedFiles.findIndex(f => f.fromCloud == true && f.path == this.selectedFilesFromCloud[i].path);

      if (index == -1) {
        let attachedFile = new AttachedFile();
        attachedFile.taskAttachedFileId = -1;
        attachedFile.name = this.selectedFilesFromCloud[i].item;
        attachedFile.fromCloud = true;
        attachedFile.path = this.selectedFilesFromCloud[i].path;
        attachedFile.file = null;

        this.attachedFiles.push(attachedFile);
      }
    }

    this.selectedFilesFromCloud.length = 0;
  }

  nodeSelected(node: DynamicFlatNode): boolean {
    let index = this.selectedFilesFromCloud.findIndex(f => f.path == node.path);

    return index >= 0 ? true : false;
  }
}
