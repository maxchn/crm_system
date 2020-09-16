import { Component, OnInit, ViewChild } from '@angular/core';

import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import listPlugin from '@fullcalendar/list';
import interactionPlugin from '@fullcalendar/interaction';

import { Utils } from '../core/utils';
import { MatDialog } from '@angular/material/dialog';
import { EventCreateDialogComponent } from '../event-create-dialog/event-create-dialog.component';
import { HttpService } from '../services/http.service';
import { DataService } from '../services/data.service';
import { EventEditDialogComponent } from '../event-edit-dialog/event-edit-dialog.component';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css'],
  providers: [HttpService, DataService]
})
export class CalendarComponent implements OnInit {

  calendarPlugins = [dayGridPlugin, timeGridPlugin, listPlugin, interactionPlugin];
  calendarEvents = [];

  buttonText = {
    today: 'Cегодня',
    month: 'Месяц',
    week: 'Неделя',
    day: 'День',
    list: 'Список'
  };

  buttonsSettings = {
    left: 'prev,next,today',
    center: 'title',
    right: 'prevYear,nextYear,dayGridMonth,timeGridWeek,timeGridDay'
  }

  constructor(public dialog: MatDialog,
    private _httpService: HttpService,
    private _dataService: DataService,
    private _snackBar: MatSnackBar) {

  }

  ngOnInit() {
    this._httpService.loadAllCalendarEvent(this._dataService.getUserId())
      .subscribe(data => {
        try {
          let events = this.calendarEvents;
          data["data"].calendarEvents.forEach(event => {
            events.push({ id: event.calendarEventId, start: event.start, end: event.end, title: event.text });
          });
        }
        catch (err) {
          Utils.printValueWithHeaderToConsole("Error", err);
        }
      }, error => {
        Utils.printValueWithHeaderToConsole("Error", error);
      });
  }

  datesSelect(event) {
    this.showCreateEventDialog(event.startStr, event.endStr);
  }

  private showCreateEventDialog(startDate, endDate) {
    const dialogRef = this.dialog.open(EventCreateDialogComponent, {
      width: '500px',
      data: { startDate: startDate, endDate: endDate }
    });

    dialogRef.afterClosed().subscribe(result => {

      if (result != null) {
        this.calendarEvents.push({ id: result.calendarEventId, start: result.start, end: result.end, title: result.text });
      }
    });
  }

  eventClick(elem) {
    let event = this.calendarEvents.find(e => e.id == elem.event.id);

    if (event != undefined) {
      const dialogRef = this.dialog.open(EventEditDialogComponent, {
        width: '500px',
        data: event
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result != null && result != undefined) {
          if (result.updateId != undefined && result.updateTitle != undefined) {
            let event = this.calendarEvents.find(e => e.id == result.updateId);

            if (event != undefined) {
              event.title = result.updateTitle;
            }
          }
          else if (result.removeId != undefined) {
            let index = this.calendarEvents.findIndex(e => e.id == result.removeId);

            if (index >= 0) {
              this.calendarEvents.splice(index, 1);
            }
          }
        }
      });
    }
  }

  async eventResize(info) {
    if (confirm("Изменить время события?")) {
      let result = await this._httpService.changeCalendarEventDataTime(info.event.id, info.event.start, info.event.end);

      if (result == null || result == undefined || !result) {
        info.revert();
      }
      else {
        this._snackBar.open(`Время события "${info.event.title}" было успешно изменено`, null, {
          duration: 3000,
        });
      }
    }
    else {
      info.revert();
    }
  }

  async eventDrop(eventDropInfo) {
    if (confirm("Изменить время события?")) {
      let result = await this._httpService.changeCalendarEventDataTime(eventDropInfo.event.id, eventDropInfo.event.start, eventDropInfo.event.end);

      if (result == null || result == undefined || !result) {
        eventDropInfo.revert();
      }
      else {
        this._snackBar.open(`Дата и время события "${eventDropInfo.event.title}" были успешно изменены`, null, {
          duration: 3000,
        });
      }
    }
    else {
      eventDropInfo.revert();
    }
  }
}