import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PalindromeListComponent } from './palindrome-list.component';

describe('PalindromeListComponent', () => {
  let component: PalindromeListComponent;
  let fixture: ComponentFixture<PalindromeListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PalindromeListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PalindromeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
