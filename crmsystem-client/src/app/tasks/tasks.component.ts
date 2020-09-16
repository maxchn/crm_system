import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { HttpService } from '../services/http.service';
import { Utils } from '../core/utils';
import { MatSnackBar } from '@angular/material';
import { Task } from '../models/taskModel';
import { DataService } from '../services/data.service';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { MatPaginatorIntlCro } from '../custom/paginator';


@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css'],
  providers: [HttpService, DataService, { provide: MatPaginatorIntl, useClass: MatPaginatorIntlCro }]
})

export class TasksComponent implements OnInit {
  allDisplayedColumns: string[] = ['name', 'createdDate', 'deadline', 'author', 'actions'];
  asAuthordisplayedColumns: string[] = ['name', 'createdDate', 'deadline', 'author'];

  dataSourceResponsible = new MatTableDataSource<Task>();
  dataSourceCoExecutor = new MatTableDataSource<Task>();
  dataSourceObserver = new MatTableDataSource<Task>();
  dataSourceAuthor = new MatTableDataSource<Task>();
  dataSourceOverdue = new MatTableDataSource<Task>();
  dataSourceCompleted = new MatTableDataSource<Task>();

  responsibleResultsLength: number;
  coExecutorResultsLength: number;
  observerResultsLength: number;
  authorResultsLength: number;
  overdueResultsLength: number;
  completedResultsLength: number;

  @ViewChild(MatPaginator, null) paginator: MatPaginator;

  todo = [];
  doing = [];
  closed = [];

  dropToDo(event: CdkDragDrop<Task[]>) {
    this.moveTask(event);

    this._httpService.checkExecution(event.container.data[event.currentIndex].taskId, false).subscribe(data => {
    }, error => {
      Utils.printValueWithHeaderToConsole("Error", error);
    });
  }

  dropDoing(event: CdkDragDrop<Task[]>) {
    this.moveTask(event);

    this._httpService.checkExecution(event.container.data[event.currentIndex].taskId, true).subscribe(data => {
    }, error => {
      Utils.printValueWithHeaderToConsole("Error", error);
    });
  }

  dropClosed(event: CdkDragDrop<Task[]>) {
    this.moveTask(event);

    this._httpService.markTaskCompletion(event.container.data[event.currentIndex].taskId, this._dataService.getUserId()).subscribe(data => {

    }, error => {
      Utils.printValueWithHeaderToConsole("Error", error);
    });
  }

  private moveTask(event: CdkDragDrop<Task[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex);
    }
  }

  constructor(private _httpService: HttpService,
    private _dataService: DataService,
    private _snackBar: MatSnackBar) { }

  ngOnInit() {
    this.dataSourceResponsible.paginator = this.paginator;

    // load tasks lists
    this.loadTasks("AS_RESPONSIBLE");
    this.loadTasks("AS_CO_EXECUTOR");
    this.loadTasks("AS_OBSERVER");
    this.loadTasks("AS_AUTHOR");
    this.loadTasks("OVERDUE");
    this.loadTasks("COMPLETED");
  }

  private loadTasks(taskType: string) {
    this._httpService.loadAllTasksAsResponsible(this._dataService.getUserId(), this._dataService.getCompanyId(), taskType)
      .subscribe(data => {

        switch (taskType) {
          case "AS_RESPONSIBLE":
            this.dataSourceResponsible.data = data["data"].tasks;
            this.responsibleResultsLength = data["data"].tasks.length;

            this.dataSourceResponsible.data.forEach((task) => {
              // Если задача сейчас не выполняется то добавляем ее в список 
              // не выполняемых задач
              if (task.isExecution === false) {

                this.todo.push(task);
              }
              // иначе в список выполняемых задач
              else {
                this.doing.push(task);
              }
            });
            break;
          case "AS_CO_EXECUTOR":
            this.dataSourceCoExecutor.data = data["data"].tasks;
            this.coExecutorResultsLength = data["data"].tasks.length;

            this.dataSourceCoExecutor.data.forEach((task) => {
              // Если задача сейчас не выполняется то добавляем ее в список 
              // не выполняемых задач
              if (!task.isExecution) {
                this.todo.push(task);
              }
              // иначе в список выполняемых задач
              else {
                this.doing.push(task);
              }
            });
            break;
          case "AS_OBSERVER":
            this.dataSourceObserver.data = data["data"].tasks;
            this.observerResultsLength = data["data"].tasks.length;
            break;
          case "AS_AUTHOR":
            this.dataSourceAuthor.data = data["data"].tasks;
            this.authorResultsLength = data["data"].tasks.length;
            break;
          case "OVERDUE":
            this.dataSourceOverdue.data = data["data"].tasks;
            this.overdueResultsLength = data["data"].tasks.length;

            this.dataSourceOverdue.data.forEach((task) => {
              // Если задача сейчас не выполняется то добавляем ее в список 
              // не выполняемых задач
              if (!task.isExecution) {
                this.todo.push(task);
              }
              // иначе в список выполняемых задач
              else {
                this.doing.push(task);
              }
            });
            break;
          case "COMPLETED":
            this.dataSourceCompleted.data = data["data"].tasks;
            this.completedResultsLength = data["data"].tasks.length;

            this.dataSourceCompleted.data.forEach((task) => {
              // Если определен финальный исполнитель значит задача была закрыта,
              // то добавляем задачу в список закрытых
              if (task.finalPerformerId !== null) {
                this.closed.push(task);
              }
            });
            break;
        }
      },
        error => {
          Utils.printValueWithHeaderToConsole(`Error AsResponsible ${taskType}`, error);
        });
  }

  generateDetailsLinkUrl(task: Task): string {
    return `/task/details/${task.taskId}`;
  }

  generateEditLinkUrl(task: Task): string {
    return `/task/edit/${task.taskId}`;
  }

  removeTask(task: Task) {
    this._httpService.removeTask(task.taskId).subscribe(data => {
      if (data["data"].removeTask.status) {
        this._snackBar.open("Задача была успешно удалена", null, {
          duration: 3000,
        });

        // FIXME: Удалить задачу из списка
      }
      else {
        this._snackBar.open(`При удалении задачи произошла ошибка! Подробнее: ${data["data"].removeTask.message}`, null, {
          duration: 3000,
        });
      }
    }, error => {
      Utils.printValueWithHeaderToConsole("Error", error);

      this._snackBar.open(`При удалении задачи произошла ошибка! Подробнее: ${error.message}`, null, {
        duration: 3000,
      });
    });
  }

  formatDate(date: string): string {
    return Utils.formatDate(date);
  }
}