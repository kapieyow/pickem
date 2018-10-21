import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TestDriverComponent } from './test-driver.component';

describe('TestDriverComponent', () => {
  let component: TestDriverComponent;
  let fixture: ComponentFixture<TestDriverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TestDriverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TestDriverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
