import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditCompanyDataDialogComponent } from './edit-company-data-dialog.component';

describe('EditCompanyDataDialogComponent', () => {
  let component: EditCompanyDataDialogComponent;
  let fixture: ComponentFixture<EditCompanyDataDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditCompanyDataDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditCompanyDataDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
