import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShortLinkCreateDialogComponent } from './short-link-create-dialog.component';

describe('CreateShortLinkDialogComponent', () => {
  let component: ShortLinkCreateDialogComponent;
  let fixture: ComponentFixture<ShortLinkCreateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShortLinkCreateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShortLinkCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
