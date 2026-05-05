import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SetupWizard } from './setup-wizard';

describe('SetupWizard', () => {
  let component: SetupWizard;
  let fixture: ComponentFixture<SetupWizard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SetupWizard],
    }).compileComponents();

    fixture = TestBed.createComponent(SetupWizard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
