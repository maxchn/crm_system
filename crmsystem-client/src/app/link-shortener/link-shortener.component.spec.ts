import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LinkShortenerComponent } from './link-shortener.component';

describe('LinkShortenerComponent', () => {
  let component: LinkShortenerComponent;
  let fixture: ComponentFixture<LinkShortenerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LinkShortenerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LinkShortenerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
