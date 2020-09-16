import { Component, OnInit, ViewChild, ElementRef, Input, ViewChildren } from '@angular/core';
import { PathItem } from '../models/pathItem';
import { Utils } from '../core/utils';
import { FileModel } from '../models/fileModel';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { HttpService } from '../services/http.service';
import { CreateFolderDialogComponent } from '../create-folder-dialog/create-folder-dialog.component';
import { RenameFileDialogComponent } from '../rename-file-dialog/rename-file-dialog.component';
import { UploadFilesDialogComponent } from '../upload-files-dialog/upload-files-dialog.component';
import { DataService } from '../services/data.service';
import { FormControl } from '@angular/forms';
import { FlatTreeControl } from '@angular/cdk/tree';
import { DynamicFlatNode } from '../models/dynamicFlatNode';
import { DynamicDataSource } from '../data-source/dynamicDataSource';

@Component({
  selector: 'app-cloud',
  templateUrl: './cloud.component.html',
  styleUrls: ['./cloud.component.css'],
  providers: [HttpService, DataService]
})

export class CloudComponent implements OnInit {
  currentPath: string = "\\";
  breadcrumbItems: Array<PathItem> = [];
  files: Array<FileModel> = [];
  selectedFiles: Array<FileModel> = [];
  publicLink = new FormControl();
  dropZoneClass: string = "col-md-10 mt-2 files-boundary drop-zone-no-active";

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
  threePaddingIndent: number = 10;
  curCloudSize = 0;
  cloudSizeInfo = {
    size: 0,
    maxSize: 0
  };

  constructor(private _snackBar: MatSnackBar,
    public dialog: MatDialog,
    private _httpService: HttpService,
    private _dataService: DataService) {

    this.treeControl = new FlatTreeControl<DynamicFlatNode>(this.getLevel, this.isExpandable);
    this.dataSource = new DynamicDataSource(this.treeControl, this._httpService, this._dataService);
    this.dataSource.fileType = 1;
  }

  ngOnInit() {
    this.calculatePath({ name: "\\", path: "\\" });
    this.dataSource.data = [new DynamicFlatNode("\\", 0, true, false, "\\")];

    this.loadCloudInfo();
  }

  loadCloudInfo() {
    this._httpService.getCloudSize(this._dataService.getCompanyId())
      .subscribe(data => {
        this.cloudSizeInfo = data;
        this.curCloudSize = Math.round((data.size * 100) / data.maxSize);
      },
        error => {
          Utils.printValueWithHeaderToConsole("[getCloudSize][error]", error);
        });
  }

  goToFolder(item: PathItem) {
    this.currentPath = item.path;
    this.calculatePath(item);
    this.selectedFiles.length = 0;
  }

  calculatePath(item: PathItem) {
    let breadcrumbItems = this.breadcrumbItems;
    breadcrumbItems.length = 0;
    breadcrumbItems.push({ name: "\\", path: "\\" });

    let pathArr = item.path.match(/\\([\wа-яёА-ЯЁ\s\(\)0-9]+)/gm);

    if (pathArr != null) {
      let offset = 0;

      pathArr.forEach(function (value) {
        let lenght = item.path.substring(offset).indexOf(value) + value.length;
        offset += lenght;

        let name = value.replace('\\', '');
        let path = item.path.substring(0, offset);

        breadcrumbItems.push({ name: name, path: path });
      });
    }

    let files = this.files;
    files.length = 0;

    this._httpService.getFilesList(item.path, this._dataService.getCompanyId(), 3).subscribe(data => {
      data.forEach(function (value) {
        files.push({ fileType: value.type, name: value.name, path: value.path, publicLink: value.publicLink });
      });

      this.sortFiles();
    },
      error => {
        Utils.printValueWithHeaderToConsole("Error Files", error);
      });
  }

  checkClick(isChecked: boolean, file: FileModel) {
    if (isChecked) {
      this.selectedFiles.push(file);
    }
    else {
      const index = this.selectedFiles.indexOf(file);

      if (index >= 0) {
        this.selectedFiles.splice(index, 1);
      }
    }
  }

  dbClick(file: FileModel) {
    if (file.fileType == 1) {      
      this.goToFolder({ name: file.name, path: file.path });
    }
  }

  showCreateNewFolderDialog() {
    const dialogRef = this.dialog.open(CreateFolderDialogComponent, {
      width: '500px',
      data: {
        currentPath: this.currentPath,
        companyId: this._dataService.getCompanyId(),
        files: this.files
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (result.status) {
          this.files.push({
            fileType: result.file.fileType,
            name: result.file.name,
            path: result.file.path,
            publicLink: null
          });

          this.sortFiles();
        }
      }
    });
  }

  showUploadFilesDialog() {
    const dialogRef = this.dialog.open(UploadFilesDialogComponent, {
      width: '600px',
      data: { path: this.currentPath, files: null, cloudSizeInfo: this.cloudSizeInfo }
    });

    dialogRef.afterClosed().subscribe(result => {
      let files = this.files;
      if (result && result.length > 0) {

        this.loadCloudInfo();

        result.forEach(function (value) {
          if (value.status)
            files.push({ fileType: value.fileType, name: value.name, path: value.path, publicLink: null });
        });
      }
    });
  }

  showRenameFileDialog() {
    const dialogRef = this.dialog.open(RenameFileDialogComponent, {
      width: '500px',
      data: {
        oldName: this.selectedFiles[0].name,
        oldPath: this.selectedFiles[0].path,
        companyId: this._dataService.getCompanyId(),
        files: this.files
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (result.status) {
          let decodeOldPath = decodeURI(result.oldPath);
          let file = this.files.find(f => f.path.indexOf(decodeOldPath) === 0);

          if (file) {
            file.name = result.newName;
            file.path = result.newPath;
          }
        }
      }
    });
  }

  deleteFiles() {
    let paths = new Array<string>();
    this.selectedFiles.forEach(function (value) {
      paths.push(value.path);
    });

    let files = this.files;

    this._httpService.deleteFiles(paths, this._dataService.getCompanyId()).subscribe(data => {
      let context = this;
      data.forEach(function (value) {
        if (value.isDeleted) {
          let index = files.findIndex(f => f.path.indexOf(value.path) === 0);

          if (index >= 0) {
            files.splice(index, 1);
          }
        }
        else {
          context._snackBar.open(value.message, null, {
            duration: 3000,
          });
        }
      });

      this.loadCloudInfo();
      this.selectedFiles.length = 0;
    }, error => {
      Utils.printValueWithHeaderToConsole("Error", error);
    });
  }

  private sortFiles() {
    this.files = this.files.sort((o1, o2) => {
      if (o1.fileType > o2.fileType) {
        return -1;
      }

      if (o1.fileType < o2.fileType) {
        return 1;
      }

      return 0;
    });
  }

  onDragStart(event, file) {
    event.dataTransfer.setData("path", file.path);
  }

  onAllowDrop(event) {
    event.preventDefault();
  }

  onDrop(event, file) {

    event.preventDefault();
    var filePath = event.dataTransfer.getData("path");

    this._httpService.moveFile(this._dataService.getCompanyId(), filePath, file)
      .subscribe(data => {
        let index = this.files.findIndex(f => f.path.indexOf(data) === 0);

        if (index >= 0) {
          this.files.splice(index, 1);
        }
      }, error => {
        Utils.printValueWithHeaderToConsole("Move Cloud error", error);

        this._snackBar.open("При перемещении элемента произошла ошибка!!!", null, {
          duration: 3000,
        });
      });
  }

  downloadFiles() {
    if (this.selectedFiles.length > 0) {
      if (this.selectedFiles.length > 1) {
        // Download archive
        this._httpService.downloadFiles(this.selectedFiles, this._dataService.getCompanyId())
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

            if (this.selectedFiles.length == 1) {
              link.download = this.selectedFiles[0].name;
            }
            else {
              link.download = `archive_${new Date().toLocaleDateString()}.zip`;
            }

            // this is necessary as link.click() does not work on the latest firefox
            link.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true, view: window }));

            setTimeout(function () {
              // For Firefox it is necessary to delay revoking the ObjectURL
              window.URL.revokeObjectURL(urlData);
              link.remove();
            }, 100);
          }, error => {
            Utils.printValueWithHeaderToConsole("downloadFiles Error", error);
          });
      }
      else {
        // Download File
        this._httpService.downloadFile(this.selectedFiles[0].path, this._dataService.getCompanyId())
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

            if (this.selectedFiles.length == 1) {
              link.download = this.selectedFiles[0].name;
            }
            else {
              link.download = `archive_${new Date().toLocaleDateString()}.zip`;
            }

            // this is necessary as link.click() does not work on the latest firefox
            link.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true, view: window }));

            setTimeout(function () {
              // For Firefox it is necessary to delay revoking the ObjectURL
              window.URL.revokeObjectURL(urlData);
              link.remove();
            }, 100);
          }, error => {
            Utils.printValueWithHeaderToConsole("Download File Error", error);
          });
      }
    }
  }

  createPublickLink() {
    if (this.selectedFiles.length > 0 &&
      this.selectedFiles.length < 2) {
      this._httpService.createFilePublickLink(this.selectedFiles[0].path, this._dataService.getCompanyId())
        .subscribe(data => {

          if (data.status == true) {
            this._snackBar.open("Публичная ссылка была успешно создана", null, {
              duration: 3000,
            });

            let path = decodeURI(data.path);
            let index = this.files.findIndex(f => f.path.indexOf(path) === 0);

            if (index >= 0) {
              this.files[index].publicLink = data.link;
            }
          }
          else {
            this._snackBar.open("При создании публичной ссылки произошла ошибка!!!", null, {
              duration: 3000,
            });
          }
        }, error => {
          Utils.printValueWithHeaderToConsole("createPublickLink Error", error);

          this._snackBar.open("При создании публичной ссылки произошла ошибка!!!", null, {
            duration: 3000,
          });
        });
    }
  }

  removePublickLink() {
    if (this.selectedFiles.length > 0 &&
      this.selectedFiles.length < 2) {
      this._httpService.removeFilePublicLink(this.selectedFiles[0].publicLink)
        .subscribe(data => {
          if (data.status == true) {
            this._snackBar.open("Публичная ссылка была успешно удалена", null, {
              duration: 3000,
            });

            let index = this.files.findIndex(f => f.publicLink != null && f.publicLink.indexOf(data.link) === 0);

            if (index >= 0) {
              this.selectedFiles[index].publicLink = null;
            }
          }
          else {
            this._snackBar.open("При удалении публичной ссылки произошла ошибка!!!", null, {
              duration: 3000,
            });
          }
        }, error => {
          Utils.printValueWithHeaderToConsole("Error", error);
          this._snackBar.open("При удалении публичной ссылки произошла ошибка!!!", null, {
            duration: 3000,
          });
        });
    }
  }

  isShowPublicLinkBlock(): boolean {
    if (this.selectedFiles.length > 0 &&
      this.selectedFiles.length < 2) {
      return this.selectedFiles[0].publicLink == null ? false : true;
    }
    else {
      return false;
    }
  }

  isFile(): boolean {
    if (this.selectedFiles.length > 0 &&
      this.selectedFiles.length < 2) {
      return this.selectedFiles[0].fileType !== 1;
    }
    else {
      return false;
    }
  }

  getPublicLink(): string {
    if (this.selectedFiles.length > 0 &&
      this.selectedFiles.length < 2) {
      return this.selectedFiles[0].publicLink;
    }

    return "";
  }

  copyToClipboard(text: any) {
    let listener = (e: ClipboardEvent) => {

      let clipboard = e.clipboardData || window["clipboardData"];
      clipboard.setData("text", text);
      e.preventDefault();

    };

    document.addEventListener("copy", listener, false)
    let result = document.execCommand("copy");
    document.removeEventListener("copy", listener, false);

    if (result) {
      this._snackBar.open('Ссылка была скопирована в буфер обмена', null, {
        duration: 3000,
      });
    }
  }

  handleDrop(event) {
    const dialogRef = this.dialog.open(UploadFilesDialogComponent, {
      width: '600px',
      data: { path: this.currentPath, files: event, cloudSizeInfo: this.cloudSizeInfo }
    });

    dialogRef.afterClosed().subscribe(result => {
      let files = this.files;
      if (result && result.length > 0) {

        this.loadCloudInfo();

        result.forEach(function (value) {
          if (value.status)
            files.push({ fileType: value.fileType, name: value.name, path: value.path, publicLink: null });
        });
      }
    });
  }

  dropZoneState($event: boolean) {
    this.dropZoneClass = $event ? "col-md-10 mt-2 files-boundary drop-zone-active" : "col-md-10 mt-2 files-boundary drop-zone-no-active";
  }

  getStringSize(size): string {

    let units = [
      "Байт",
      "КБ",
      "МБ",
      "ГБ",
      "ТБ",
      "ПБ",
      "ЭБ",
      "ЗБ",
      "ИБ",
    ];
    let index = 0;

    while (size > 1024) {
      size = size / 1024;
      index++;
    }

    return `${size.toFixed(2)} ${units[index]}`;
  }
}