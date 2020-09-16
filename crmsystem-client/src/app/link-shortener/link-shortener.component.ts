import { Component, OnInit, ViewChild } from '@angular/core';
import { HttpService } from '../services/http.service';
import { DataService } from '../services/data.service';
import { MatTableDataSource, MatSnackBar, MatPaginator, MatPaginatorIntl, MatDialog } from '@angular/material';
import { ShortLink } from '../models/shortLink';
import { Utils } from '../core/utils';
import { MatPaginatorIntlCro } from '../custom/paginator';
import { ShortLinkCreateDialogComponent } from '../short-link-create-dialog/short-link-create-dialog.component';

@Component({
  selector: 'app-link-shortener',
  templateUrl: './link-shortener.component.html',
  styleUrls: ['./link-shortener.component.css'],
  providers: [HttpService, DataService, { provide: MatPaginatorIntl, useClass: MatPaginatorIntlCro }]
})
export class LinkShortenerComponent implements OnInit {

  allDisplayedColumns: string[] = ['fullLink', 'shortLink', 'actions'];

  userLinks = new MatTableDataSource<ShortLink>();
  companyLinks = new MatTableDataSource<ShortLink>();

  userLinksLength: number = 0;
  companyLinksLength: number = 0;

  @ViewChild(MatPaginator, null) paginator: MatPaginator;

  constructor(private httpService: HttpService,
    private dataService: DataService,
    private _snackBar: MatSnackBar,
    public dialog: MatDialog) { }

  ngOnInit() {
    this.userLinks.paginator = this.paginator;
    this.companyLinks.paginator = this.paginator;

    this.httpService.loadAllUserShortLinks(this.dataService.getUserId())
      .subscribe(data => {
        this.userLinks.data = data["data"].userShortLinks;
        this.userLinksLength = data["data"].userShortLinks.length;
      }, error => {
        Utils.printValueWithHeaderToConsole("loadAllUserShortLinks error", error);
        this.userLinksLength = 0;
      });

    this.httpService.loadAllCompanyShortLinks(this.dataService.getCompanyId())
      .subscribe(data => {
        this.companyLinks.data = data["data"].companyShortLinks;
        this.companyLinksLength = data["data"].companyShortLinks.length;
      }, error => {
        Utils.printValueWithHeaderToConsole("loadAllCompanyShortLinks error", error);
        this.companyLinksLength = 0;
      });
  }

  showCreateShortLink() {
    const dialogRef = this.dialog.open(ShortLinkCreateDialogComponent, {
      width: '500px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result != null && result != undefined) {
        if (result.newLink.ownerId != null) {
          const data = this.userLinks.data;
          data.push(result.newLink);

          this.userLinks.data = data;
          this.userLinksLength = data.length;
        }
        else {
          const data = this.companyLinks.data;
          data.push(result.newLink);

          this.companyLinks.data = data;
          this.companyLinksLength = data.length;
        }
      }
    });
  }

  deleteLink(link: number) {
    this.httpService.removeShortLink(link).subscribe(data => {
      if (data["data"].deleteShortLink.status) {
        this._snackBar.open("Ссылка была успешно удалена", null, {
          duration: 3000,
        });

        const userData = this.userLinks.data;
        let index = userData.findIndex(l => l.shortLinkId == data["data"].deleteShortLink.value);

        if (index >= 0) {
          userData.splice(index, 1);
        }

        this.userLinks.data = userData;
        this.userLinksLength = userData.length;

        const companyData = this.companyLinks.data;
        index = companyData.findIndex(l => l.shortLinkId == data["data"].deleteShortLink.value);

        if (index >= 0) {
          companyData.splice(index, 1);
        }

        this.companyLinks.data = companyData;
        this.companyLinksLength = companyData.length;
      }
      else {
        this._snackBar.open(data["data"].deleteShortLink.message, null, {
          duration: 3000,
        });
      }
    }, error => {
      Utils.printValueWithHeaderToConsole("Error", error);

      this._snackBar.open(`При удалении ссылки произошла ошибка!!! Подробнее: ${error.message}`, null, {
        duration: 3000,
      });
    });
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
}