import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrivateNotificationDialogComponent } from './private-notification-dialog.component';

describe('PrivateNotificationDialogComponent', () => {
  let component: PrivateNotificationDialogComponent;
  let fixture: ComponentFixture<PrivateNotificationDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrivateNotificationDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrivateNotificationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
