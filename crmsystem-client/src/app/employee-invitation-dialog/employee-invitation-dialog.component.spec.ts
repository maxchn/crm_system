import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeInvitationDialogComponent } from './employee-invitation-dialog.component';

describe('EmployeeInvitationDialogComponent', () => {
  let component: EmployeeInvitationDialogComponent;
  let fixture: ComponentFixture<EmployeeInvitationDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EmployeeInvitationDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeeInvitationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
