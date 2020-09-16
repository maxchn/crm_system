import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpService } from '../services/http.service';
import { Utils } from '../core/utils';
import { Task } from '../models/taskModel';
import { DataService } from '../services/data.service';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-task-details',
  templateUrl: './task-details.component.html',
  styleUrls: ['./task-details.component.css'],
  providers: [HttpService, DataService]
})
export class TaskDetailsComponent implements OnInit {
  commentForm: FormGroup;
  id: number;
  model: Task = new Task();
  runButtonText: String = 'Начать выполнение';
  taskComments: Array<any> = [];
  attachedFiles: Array<any> = [];
  reopenTaskAccess: boolean = false;

  constructor(private activateRoute: ActivatedRoute,
    fb: FormBuilder,
    private _httpService: HttpService,
    private _dataService: DataService,
    private _snackBar: MatSnackBar,
    private _router: Router) {
    this.id = activateRoute.snapshot.params['id'];

    this.commentForm = fb.group({
      hideRequired: false,
      floatLabel: 'auto',
    });
  }

  ngOnInit() {

    this.commentForm = new FormGroup({
      text: new FormControl('',
        [
          Validators.required,
          Validators.maxLength(512)
        ])
    });

    this._httpService.getTaskDetailsInfoById(this.id).subscribe(data => {
      this.model = data["data"].task;

      if (data["data"].taskAttachedFiles) {
        for (let i = 0; i < data["data"].taskAttachedFiles.length; i++) {
          this.attachedFiles.push(data["data"].taskAttachedFiles[i].attachedFile);
        }
      }

      this.runButtonText = data["data"].task.isExecution ? "Закончить выполнение" : "Начать выполнение";
    },
      error => {
        Utils.printValueWithHeaderToConsole("Error", error);
      });

    this._httpService.loadAllTaskComments(this.id).subscribe(data => {
      if (data["data"] && data["data"].comments) {
        this.taskComments = data["data"].comments;
      }
    },
      error => {
        Utils.printValueWithHeaderToConsole("loadAllTaskComments Error", error);
      });

    this._httpService.getAccessOnReopenTask(this.id).subscribe(data => {
      this.reopenTaskAccess = data["data"].getAccessOnReopenTask;
    }, error => {
      Utils.printValueWithHeaderToConsole("[getAccessOnReopenTask][error]", error);
      this.reopenTaskAccess = false;
    });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.commentForm.controls[controlName].hasError(errorName);
  }

  goToEdit() {
    this._router.navigate([`/task/edit/${this.id}`]);
  }

  clickExecution() {
    this._httpService.checkExecution(this.id, !this.model.isExecution).subscribe(data => {
      if (data["data"].checkExecution.status) {
        this.model.isExecution = data["data"].checkExecution.isExecution;
        this.runButtonText = data["data"].checkExecution.isExecution ? "Закончить выполнение" : "Начать выполнение";
      }
    }, error => {
      Utils.printValueWithHeaderToConsole("Error", error);
    });
  }

  markCompleted() {
    this._httpService.markTaskCompletion(this.id, this._dataService.getUserId()).subscribe(data => {
      this.model.finalPerformerId = this._dataService.getUserId();

      this._httpService.getAccessOnReopenTask(this.id).subscribe(data => {
        this.reopenTaskAccess = data["data"].getAccessOnReopenTask;
      }, error => {
        Utils.printValueWithHeaderToConsole("[getAccessOnReopenTask][error]", error);
        this.reopenTaskAccess = false;
      });
    }, error => {
      Utils.printValueWithHeaderToConsole("Error", error);
    });
  }

  addComment() {
    if (this.commentForm.valid) {
      let textComment = this.commentForm.value.text;

      this._httpService.addTaskComment(textComment, this.id, this._dataService.getUserId())
        .subscribe(data => {
          this.commentForm.patchValue({
            text: ''
          });
          this.taskComments.push(data["data"].createTaskComment);
        }, error => {
          Utils.printValueWithHeaderToConsole("addTaskComment error", error);
        });
    }
  }

  deleteComment(comment) {
    this._httpService.deleteComment(comment.taskCommentId).subscribe(data => {
      if (data["data"]) {
        if (data["data"].deleteTaskComment.status) {
          let index = this.taskComments.findIndex(c => c.taskCommentId == data["data"].deleteTaskComment.value);

          if (index >= 0) {
            this.taskComments.splice(index, 1);
          }

          this._snackBar.open("Комментарий был успешно удален", null, {
            duration: 3000,
          });
        }
        else {
          this._snackBar.open(data["data"].deleteTaskComment.message, null, {
            duration: 3000,
          });
        }
      }
      else {
        this._snackBar.open("При удалении комментария произошла ошибка!!!", null, {
          duration: 3000,
        });
      }
    }, error => {
      Utils.printValueWithHeaderToConsole("deleteComment Error", error);

      this._snackBar.open("При удалении комментария произошла ошибка!!!", null, {
        duration: 3000,
      });
    });
  }

  formatDate(date: string): string {
    return Utils.formatDate(date);
  }

  downloadAttachedFile(attachedFile) {

    this._httpService.downloaAttachedFile(attachedFile.attachedFileId)
      .subscribe(data => {
        var newBlob = new Blob([data], { type: data.type });

        // IE doesn't allow using a blob object directly as link href
        // instead it is necessary to use msSaveOrOpenBlob
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
          window.navigator.msSaveOrOpenBlob(newBlob);
          return;
        }

        // For other browsers: 
        // Create a link pointing to the ObjectURL containing the blob.
        const urlData = window.URL.createObjectURL(newBlob);

        var link = document.createElement('a');
        link.href = urlData;

        link.download = attachedFile.name;

        // this is necessary as link.click() does not work on the latest firefox
        link.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true, view: window }));

        setTimeout(function () {
          // For Firefox it is necessary to delay revoking the ObjectURL
          window.URL.revokeObjectURL(urlData);
          link.remove();
        }, 100);
      }, error => {
        Utils.printValueWithHeaderToConsole("[downloaAttacheddFile][error]", error);
      });
  }

  reopenTask() {
    this._httpService.reopenTask(this.id).subscribe(data => {
      this.reopenTaskAccess = !data["data"].reopenTask;
      this.model.finalPerformerId = null;
    }, error => {
      Utils.printValueWithHeaderToConsole("[reopenTask][error]", error);
    });
  }

  formatTime(value: string): string {
    return parseFloat(value).toFixed(1);
  }
}
