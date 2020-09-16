import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatSettingsDialogComponent } from './chat-settings-dialog.component';

describe('ChatSettingsDialogComponent', () => {
  let component: ChatSettingsDialogComponent;
  let fixture: ComponentFixture<ChatSettingsDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatSettingsDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatSettingsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
