import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExtendedRegistrationComponent } from './extended-registration.component';

describe('ExtendedRegistrationComponent', () => {
  let component: ExtendedRegistrationComponent;
  let fixture: ComponentFixture<ExtendedRegistrationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExtendedRegistrationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExtendedRegistrationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
